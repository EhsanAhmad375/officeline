using Microsoft.EntityFrameworkCore;
namespace officeline.Models
{
    public class OrderModel
    {
        public int Id { get; set; }
        public ProductModel Product { get; set; } = null!;
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string PaymentStatus { get; set; } = "Unpaid";
        public UsersModel User { get; set; } = null!;
        public int UserId { get; set; }
        public String Status { get; set; } = "Pending";
        public int? OrderNumber { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}