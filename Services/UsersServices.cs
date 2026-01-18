using officeline.Models;
using officeline.Data; 
using officeline.DTO;
using officeline.Services;
using Microsoft.EntityFrameworkCore;
using officeline.ErrorExceptions;
using BCrypt.Net;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt; // JwtSecurityTokenHandler ke liye
using Microsoft.Extensions.Configuration;
using System.Text;
using Microsoft.AspNetCore.Mvc;
namespace officeline.Services;


    public interface IUsers
    {
        Task<CreateUserDTO> CreateUserAsync(CreateUserDTO createUserDTO, string creatorRole, int creatorCompanyId);
        Task<LoginDetailsDTO> UserLoginAsync(LoginUserDTO loginUserDTO);

        Task<List<GetAllUsersDTO>> GetAllUsersAsync(string role, int companyId);

        Task<GetUserDetailDTO> GetUserDetailAsync();
        Task<GetUserDetailDTO> UpdateUserProfile(updateUserProfileDTO updateUser);

        Task<bool> DeleteUser(int userId);
        
    }



public class Users : IUsers
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public Users(AppDbContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _context=context;
        _configuration=configuration;
        _httpContextAccessor=httpContextAccessor;
    }

    public async Task<CreateUserDTO> CreateUserAsync(CreateUserDTO createUserDTO, string creatorRole, int creatorCompanyId)
    {
        if (creatorRole.ToLower() == "admin")
        {
            if (creatorCompanyId != createUserDTO.CompanyId)
            {
            throw new ApiException("CompanyId","you cannot create user of other company");    
            }
            
        }
        var emailExist=await _context.Users.AnyAsync(u=> u.email==createUserDTO.email);
        if (emailExist)
        {
            throw new ApiException("email",$"{createUserDTO.email} is already used");
        }
        var isCompany=await _context.Companies.FindAsync(createUserDTO.CompanyId);
        if (isCompany==null)
        {
            throw new ApiException("Company",$"Company not Found");
        }
        string salt=BCrypt.Net.BCrypt.GenerateSalt(12);
        string hashPassword=BCrypt.Net.BCrypt.HashPassword(createUserDTO.password);


        var maxUserNumber=await _context.Users.Where(u=>u.CompanyId == createUserDTO.CompanyId)
        .Select(u=>(int?)u.userNumber).MaxAsync()??0;

        int nextUserNumber=maxUserNumber+1;

        var user =new UsersModel{ 
          fName=createUserDTO.fName,
          lName=createUserDTO.lName,
          email=createUserDTO.email,
          CompanyId=createUserDTO.CompanyId,
          userNumber=nextUserNumber,
          password=hashPassword  
        };
        await _context.AddAsync(user);
        await _context.SaveChangesAsync();
        createUserDTO.password=null;
        return createUserDTO;
    }


    public async Task<LoginDetailsDTO> UserLoginAsync(LoginUserDTO loginUserDTO)
    {
        var user = await _context.Users
        .FirstOrDefaultAsync(u => u.email == loginUserDTO.email);

    if (user == null)
    {
        throw new ApiException("email", "Invalid Email Address");
    }
        bool isPasswordValid=BCrypt.Net.BCrypt.Verify(loginUserDTO.password,user.password);
        if (!isPasswordValid)
        {
            throw new ApiException("password","Invalid Password");
        }
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier,user.userId.ToString()),
            new Claim(ClaimTypes.Email,user.email),
            new Claim(ClaimTypes.Role,user.role),
            new Claim("CompanyId",user.CompanyId.ToString())
        };

        var key= new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
        issuer: _configuration["Jwt:Issuer"],
        audience: _configuration["Jwt:Audience"],
        claims: claims,
        expires: DateTime.Now.AddDays(1),
        signingCredentials: creds
    );
    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
    var userDetail=new LoginDetailsDTO
    {
    userId=user.userId,
    fName=user.fName,
    lName=user.lName,
    role=user.role,
    CompanyId=user.CompanyId,
    token=tokenString

    };
    return userDetail;   
    }


    
    
    public async Task<List<GetAllUsersDTO>> GetAllUsersAsync(string role,int CompanyId)
    {
        var query=_context.Users.AsQueryable();
        if (role == "admin")
        {
        query=query.Where(u=>u.CompanyId==CompanyId);    
        }

        var users=await query.Select(u=> new GetAllUsersDTO
        {
            userId=u.userId,
            fName=u.fName,
            lName=u.lName,
            role=u.role,
            CompanyId=u.CompanyId,
        }).ToListAsync();
        // var users=await _context.Users.Select(u => new GetAllUsersDTO
        // {
        //     userId=u.userId,
        //     fName=u.fName,
        //     lName=u.fName,
        //     role=u.role,
        //     CompanyId=u.CompanyId
        // }).ToListAsync();
        return users;
    }




    public async Task<GetUserDetailDTO> GetUserDetailAsync()
    {
        var userIdClaimed=_httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaimed))
        {
            throw new ApiException("user","UnAuthorized");
        }

        int userId=int.Parse(userIdClaimed);

        var userDetail=await _context.Users.Where(u=> u.userId==userId).
        Select(u=> new GetUserDetailDTO
        {
            userId=u.userId,
            userNumber=u.userNumber,
            fName=u.fName,
            lName=u.lName,
            email=u.email,
            role=u.role,
            PhoneNumber=u.PhoneNumber,
            dob=u.dob,
            CompanyId=u.CompanyId
        }).FirstOrDefaultAsync();

        if (userDetail == null)
        {
            throw new ApiException("User", "User details not found.");
        }
        return userDetail;
    }



    public async Task<GetUserDetailDTO> UpdateUserProfile(updateUserProfileDTO updateUser)
    {
        var userIdClaimed=_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaimed))
        {
            throw new ApiException("user","Unauthorized access");
        }

        int userId=int.Parse(userIdClaimed);

        var user=await _context.Users.FindAsync(userId);
        if (user == null)
        {
            throw new ApiException("user","user not found");
        }

            user.fName = updateUser.fName;
            user.lName = updateUser.lName;
            user.dob=updateUser.dob;
            user.PhoneNumber=updateUser.PhoneNumber;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return new GetUserDetailDTO{
                userId = user.userId,
                fName = user.fName,
                lName = user.lName,
                email = user.email,
                role = user.role,
                CompanyId = user.CompanyId
    };
    }



    public async Task<bool> DeleteUser(int UserId)
    {
        var user=await _context.Users.FindAsync(UserId);
        if (user == null)
        {
            throw new ApiException("user","user not found");
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return true;
    }
}
