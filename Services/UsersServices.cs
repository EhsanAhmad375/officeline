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
using System.IdentityModel.Tokens.Jwt; 
using officeline.repo;
using Microsoft.Extensions.Configuration;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Formats.Asn1;
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
    private readonly IUserRepo _userRepo;
    private readonly ICompanyRepo _companyRepo;
    public Users(AppDbContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IUserRepo userRepo, ICompanyRepo companyRepo )
    {
        _context=context;
        _configuration=configuration;
        _httpContextAccessor=httpContextAccessor;
        _userRepo=userRepo;  
        _companyRepo=companyRepo;
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
        var emailExist=await _userRepo.GetUserByEmailAsync(createUserDTO.email) != null;
        if (emailExist)
        {
            throw new ApiException("email",$"{createUserDTO.email} is already used");
        }
        var isCompany=await _companyRepo.GetCompanyByIdAsync(createUserDTO.CompanyId);
        if (isCompany==null)
        {
            throw new ApiException("Company",$"Company not Found");
        }
        string salt=BCrypt.Net.BCrypt.GenerateSalt(12);
        string hashPassword=BCrypt.Net.BCrypt.HashPassword(createUserDTO.password);


        var maxUserNumber=await _userRepo.GetMaxUserNumerOfACompanyAsync(createUserDTO.CompanyId);

        int nextUserNumber=maxUserNumber+1;

        var user =new UsersModel{ 
          fName=createUserDTO.fName,
          lName=createUserDTO.lName,
          email=createUserDTO.email,
          CompanyId=createUserDTO.CompanyId,
          userNumber=nextUserNumber,
          role="employee", 
          password=hashPassword  
        };
        await _userRepo.AddUserAsync(user);
        await _userRepo.SaveChangesAsync();
        createUserDTO.password=null;
        return createUserDTO;
    }


    public async Task<LoginDetailsDTO> UserLoginAsync(LoginUserDTO loginUserDTO)
    {
        // var user = await _context.Users.FirstOrDefaultAsync(u => u.email == loginUserDTO.email);
        var user= await _userRepo.GetUserByEmailAsync(loginUserDTO.email);

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
        var user=await _userRepo.GetAllUsersAsync();

        var query=user.AsQueryable();

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
        var request = _httpContextAccessor.HttpContext?.Request;
        var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";

        var userDetail=await _userRepo.GetUserByIdAsync(userId);
        if (userDetail == null)
        {
            throw new ApiException("User", "User details not found.");
        }
        
        return new GetUserDetailDTO
        {
            userId=userDetail.userId,
            userNumber=userDetail.userNumber,
            fName=userDetail.fName,
            lName=userDetail.lName,
            email=userDetail.email,
            role=userDetail.role,
            PhoneNumber=userDetail.PhoneNumber,
            dob=userDetail.dob,
            CompanyId=userDetail.CompanyId,
            profilepic=!string.IsNullOrEmpty(userDetail.profile_pic) 
                 ? $"{baseUrl}/uploads/profiles/{userDetail.profile_pic}" 
                 : $"{baseUrl}/uploads/profiles/default_avatar.png"
        };
    }


public async Task<GetUserDetailDTO> UpdateUserProfile(updateUserProfileDTO updateUser)
{
    var userIdClaimed = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(userIdClaimed)) throw new ApiException("user", "Unauthorized access");
    
    int userId = int.Parse(userIdClaimed);

    var user = await _userRepo.GetUserByIdAsync(userId);
    if (user == null) throw new ApiException("user", "User not found");

    if (updateUser.profilepic != null && updateUser.profilepic.Length > 0)
    {
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profiles");
        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(updateUser.profilepic.FileName);
        var filePath = Path.Combine(uploadsFolder, fileName);

        
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await updateUser.profilepic.CopyToAsync(stream);
        }

        if (!string.IsNullOrEmpty(user.profile_pic) && user.profile_pic != "default_avatar.png")
        {
            var oldPath = Path.Combine(uploadsFolder, user.profile_pic);
            if (File.Exists(oldPath)) File.Delete(oldPath);
        }

        user.profile_pic = fileName; 
    }

    user.fName = updateUser.fName;
    user.lName = updateUser.lName;
    user.dob = updateUser.dob;
    user.PhoneNumber = updateUser.PhoneNumber;

    await _userRepo.UpdateUserAsync(user);

    var request = _httpContextAccessor.HttpContext?.Request;
    var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";

    return new GetUserDetailDTO
    {
        userId = user.userId,
        fName = user.fName,
        lName = user.lName,
        email = user.email,
        role = user.role,
        CompanyId = user.CompanyId,
        profilepic = !string.IsNullOrEmpty(user.profile_pic) 
                 ? $"{baseUrl}/uploads/profiles/{user.profile_pic}" 
                 : $"{baseUrl}/uploads/profiles/default_avatar.png"
    };
}

    public async Task<bool> DeleteUser(int UserId)
    {
        var user=await _userRepo.DeleteUserAsync(UserId);
        if (user == false)
        {
            throw new ApiException("user","user not found");
        }

        return true;
    }


}
 