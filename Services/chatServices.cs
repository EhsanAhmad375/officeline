using officeline.Data;
using officeline.DTO;
using officeline.Models;
using officeline.repo;
using officeline.ErrorExceptions;
using Microsoft.EntityFrameworkCore;
namespace officeline.Services
{
    public interface IChatService
    {
        public Task<GetChatMessageDTO> SendMessageAsync(SendMessageDTO sendMessageDTO,int SenderId);
        public Task<List<GetChatMessageDTO>> GetChatMessagesAsync(int userId1, int userId2);
        public Task<bool> MarkAsReadAsync(int messageId);
    }

    public class ChatService : IChatService
    {
        public readonly IChatRepo _chatRepo;
        public readonly IUserRepo _userRepo;
        public ChatService(IChatRepo chatRepo,IUserRepo userRepo)
        {
            _chatRepo = chatRepo;
            _userRepo = userRepo;
        }

        public async Task<GetChatMessageDTO> SendMessageAsync(SendMessageDTO sendMessageDTO, int SenderId)
        {
            var reveiver= await _userRepo.GetUserByIdAsync(sendMessageDTO.ReceiverId);
            if (reveiver == null)
            {
                 throw new ApiException("message","Receiver not found");
            }
            if(sendMessageDTO.ReceiverId==SenderId)
            {
                 throw new ApiException("message","Cannot send message to yourself");
            }


            var chat =new ChatsModel
            {
                SenderId=SenderId,
                ReceiverId=sendMessageDTO.ReceiverId,
                Message=sendMessageDTO.Message,
                Timestamp=DateTime.UtcNow,
                IsRead=false
            };
            var savedChat= await _chatRepo.SendMessageAsync(chat);
            var chatDTO=new GetChatMessageDTO{
                Id=savedChat.Id,
                SenderId=savedChat.SenderId,
                ReceiverId=savedChat.ReceiverId,
                Message=savedChat.Message,
                Timestamp=savedChat.Timestamp,
                IsRead=savedChat.IsRead,
                IsMine=true
            };
            return chatDTO;
        }

        public async Task<List<GetChatMessageDTO>> GetChatMessagesAsync(int userId1, int userId2)
        {
            var chatMessages = await _chatRepo.GetChatMessagesAsync(userId1, userId2);
            if (chatMessages == null || chatMessages.Count == 0)
            {
                throw new ApiException("messages", "No messages found between the users.");
            }
            return chatMessages;
        }

        public async Task<bool> MarkAsReadAsync(int messageId)
        {
            return await _chatRepo.MarkAsReadAsync(messageId);
        }
    }
}