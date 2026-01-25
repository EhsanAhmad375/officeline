using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using officeline.Models;
namespace officeline.Models
{
    [Table("chats")]
    public class ChatsModel
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Sender")]
        public int? SenderId { get; set; }
        public UsersModel? Sender { get; set; }

        [ForeignKey("Receiver")]
        public int? ReceiverId { get; set; }
        public UsersModel? Receiver { get; set; }

        public string Message { get; set; }

        public DateTime Timestamp { get; set; }

        public bool IsRead { get; set; }
    }
}