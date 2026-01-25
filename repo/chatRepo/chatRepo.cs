using officeline.Data;
using officeline.DTO;
using officeline.Models;
using Microsoft.EntityFrameworkCore;
namespace officeline.repo
{
    public interface IChatRepo
    {
        Task<ChatsModel> SendMessageAsync(ChatsModel chat);
        Task<List<GetChatMessageDTO>> GetChatMessagesAsync(int userId1, int userId2);
        Task<bool> MarkAsReadAsync(int messageId);
        Task SaveChangesAsync();
        
    }

    public class ChatsRepo :IChatRepo
    {
        private readonly AppDbContext _context;
        public ChatsRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<ChatsModel> SendMessageAsync(ChatsModel chat)
        {
            await _context.Chats.AddAsync(chat);
            await SaveChangesAsync();
            return chat;
        }

        public async Task<List<GetChatMessageDTO>> GetChatMessagesAsync(int userId1, int userId2)
        {
            var chat= await _context.Chats.Where(c=>c.SenderId==userId1 && c.ReceiverId==userId2 
            || c.SenderId==userId2 && c.ReceiverId==userId1).OrderBy(c=>c.Timestamp).ToListAsync();
            var chatDTOs= chat.Select(c=> new GetChatMessageDTO
            {
                Id=c.Id,
                SenderId=c.SenderId,
                ReceiverId=c.ReceiverId,
                Message=c.Message,
                Timestamp=c.Timestamp,
                IsRead=c.IsRead,
                IsMine=c.SenderId==userId1
            }).ToList();
            return chatDTOs;
            }

        public async Task<bool> MarkAsReadAsync(int messageId)
        {
            var message = await _context.Chats.FindAsync(messageId);
            if (message == null)
            {
                return false;
            }
            message.IsRead = true;
            await SaveChangesAsync();
            return true;
        }













        }
}
