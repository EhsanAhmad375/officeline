using officeline.Models;
using System.ComponentModel.DataAnnotations;

namespace officeline.DTO
{
    public class SendMessageDTO
{
    [Required]
    public int ReceiverId { get; set; }

    [Required]
    public string Message { get; set; }
}

public class GetChatMessageDTO
{
    public int Id { get; set; }
    public int? SenderId { get; set; }
    public int? ReceiverId { get; set; }
    public string Message { get; set; }
    public DateTime Timestamp { get; set; }
    public bool IsRead { get; set; }
    
    // Ye janne ke liye ke message "Me" ne bheja hai ya "Dost" ne
    public bool IsMine { get; set; } 
}
}