using officeline.DTO;
using officeline.Models;
using Microsoft.EntityFrameworkCore;
using officeline.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using officeline.ErrorExceptions;
using Microsoft.AspNetCore.Authorization;
namespace officeline.Controllers
{
    

    [ApiController]
    [Route("api/[controller]")]
    public class ChatsController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IHttpContextAccessor _httpContextAccessot;
        public ChatsController(IChatService chatService, IHttpContextAccessor httpContextAccessor)
        {
            _chatService = chatService;
            _httpContextAccessot = httpContextAccessor;
        }

        [Authorize]
        [HttpPost("send")]
        public async Task<ActionResult<GetChatMessageDTO>> SendMessage([FromBody] SendMessageDTO sendMessageDTO)
        {
            try{
            var SenderId=int.Parse(_httpContextAccessot.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var sendMessage=await _chatService.SendMessageAsync(sendMessageDTO,SenderId);
            return StatusCode(201, new{sucess=true,sendMessage});
            }catch(ApiException ex)
            {
                return StatusCode(400,new{sucess=false,error=new Dictionary<string,string>{{ex.FieldName,ex.Message}}});
            }
            catch(Exception ex)
            {
                return StatusCode(500,new{sucess=false,error="Internal server error"});
            } 
        }
        [Authorize]
        [HttpGet("between/{userId}")]
        public async Task<ActionResult<List<GetChatMessageDTO>>> GetChatMessages([FromRoute]int userId)
        {
            try
            {
               var currentUserId = int.Parse( _httpContextAccessot.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                var chatMessages = await _chatService.GetChatMessagesAsync(currentUserId, userId);
                return StatusCode(200, new { success = true, chatMessages });
            }
            catch (ApiException ex)
            {
                return StatusCode(400, new { success = false, error = new Dictionary<string, string> { { ex.FieldName, ex.Message } } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = "Internal server error" });
            }
            
        }

    }
}