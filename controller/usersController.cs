using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using officeline.Data;
using officeline.Services;
using officeline.Models;
using officeline.DTO;
using officeline.ErrorExceptions;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
namespace officeline.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
        public class UserController : ControllerBase
    {
        private readonly IUsers _users;

        public UserController(IUsers users)
        {
            _users=users;
        }


        // [Authorize(Roles="superadmin,admin")]
        [HttpPost("create")]
        public async Task<ActionResult<UsersModel>> CreateUsers([FromBody] CreateUserDTO createUserDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new {success=false, message="User not created",error=ModelState });
                
            }
            try
            {
                var creatorRole=User.FindFirst(ClaimTypes.Role)?.Value;
                var creatorCompanyClaim=User.FindFirst("CompanyId")?.Value;
                int creatorCompanyId=int.Parse(creatorCompanyClaim??"0");
                await _users.CreateUserAsync(createUserDTO,creatorRole,creatorCompanyId); 
                return Ok(new {success=false, message="user Created Successfuly" });
            }catch(ApiException ex)
            {
                return BadRequest(new {success=false,error=new Dictionary<string,string> {{ex.FieldName,ex.Message}}});
            }catch(Exception ex)
            {
                return BadRequest(new {success=false,message="server error",error=ex.Message});
            }
            
            
        }

        [HttpPost("login")]
        public async Task<ActionResult<UsersModel>> userLogin([FromBody] LoginUserDTO loginUser)
        {
            if (!ModelState.IsValid)
            {
                BadRequest(new {success=false, error=ModelState});
            }

            try
            {
                var user=await _users.UserLoginAsync(loginUser);
                return Ok(new {success=true, message="user Created Successfuly",data=user});
            }catch(ApiException ex)
            {
                return BadRequest(new {success=false,error=new Dictionary<string,string>{{ex.FieldName,ex.Message}}});
            }catch(Exception ex)
            {
                return BadRequest(new {success=false,message="server error",error=ex.Message});
            }



            
        }

        [Authorize(Roles="superadmin,admin")]
        [HttpGet("users")]
        public async Task<ActionResult> getAllUsersList()
        {
            try
            {
                var userRole=User.FindFirst(ClaimTypes.Role)?.Value;
                var companyIdClaim=User.FindFirst("CompanyId")?.Value;

                int companyId=string.IsNullOrEmpty(companyIdClaim)?0:int.Parse(companyIdClaim);
                var usersList=await _users.GetAllUsersAsync(userRole,companyId);
                return Ok(new {success=true,message="All userse retrived success fully",data=usersList});
            }catch(Exception ex)
            {
                return BadRequest(new {success=true,message=ex.Message});
                
            }
        }
        [HttpGet("profile")]
        public async Task<ActionResult> getUserDetail()
        {
            try
            {
                var userDetail=await _users.GetUserDetailAsync();
                return Ok(new {success=true,message="Retrive user detail successfully",data=userDetail});
            }catch(ApiException ex)
            {
                return BadRequest(new {success=false,message="Retrive user detail successfully" ,
                erros= new Dictionary<string,string>{{ex.FieldName,ex.Message}}});
            }catch(Exception ex)
            {
                return BadRequest( new {success=false, erros=ex.Message});
            }

        }

        [HttpPatch("update")]
        public async Task<ActionResult> updateUser([FromForm] updateUserProfileDTO updateUser)
        {
            try
            {
                var user=await _users.UpdateUserProfile(updateUser);
                return Ok(new {success=true,message="user updated successfully",data=user});
            }catch(ApiException ex)
            {
                return BadRequest(new {success=false, message="Failed! user profile",
                error=new Dictionary<string,string>{{ex.FieldName,ex.Message}}});
            }catch(Exception ex)
            {
                return BadRequest(new {success=false, message="Failed! user profile", 
                error=ex.Message});
            }
            
        }

        [Authorize(Roles="superadmin,admin")]
        [HttpDelete("delete/{userId}")]
        public async Task<ActionResult> deleteUser([FromRoute] int userId)
        {
            try
            {
                await _users.DeleteUser(userId);
                return Ok(new {success=true,message="User Deleted Successully "});

            }
            catch (ApiException ex)
            {
                return BadRequest(new {success=false,error=new Dictionary<string,string> {{ex.FieldName,ex.Message}}});
            }catch(Exception ex)
            {
                return BadRequest(new {success=false, error=ex.Message});
            }
        }


    }


}