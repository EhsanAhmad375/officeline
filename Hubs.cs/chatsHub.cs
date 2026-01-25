using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using officeline.ErrorExceptions;
using officeline.Services;
using Microsoft.AspNetCore.Authorization;
using officeline.DTO;
namespace officeline.Hubs
{
    [Authorize]
    public class ChatsHub : Hub
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IChatService _chatService;
        public ChatsHub(IHttpContextAccessor httpContextAccessor, IChatService chatService)
        {
            _httpContextAccessor = httpContextAccessor;
            _chatService = chatService;
        }

  
        public async Task SendPrivateMessage(SendMessageDTO sendMessageDTO)
        {
                var SenderId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                var savedMessage=await _chatService.SendMessageAsync(sendMessageDTO,SenderId);
                await Clients.User(sendMessageDTO.ReceiverId.ToString()).
                            SendAsync("ReceivePrivateMessage", savedMessage);
        }



        public async Task TypingIndicator(int ReceiverId)
        {
            var SenderId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            await Clients.User(ReceiverId.ToString()).
                        SendAsync("ReceiveTypingIndicator", SenderId);
        }

                
    }
}
