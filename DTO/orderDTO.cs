using Microsoft.AspNetCore.Mvc;
using officeline.DTO;
namespace officeline.DTO
{
    public class OrderCreateDTO
    {
        
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }


    public class OrderDetailDTO
    {
        public int Id { get; set; }
        public int OrderNumber { get; set; }
        public ProductDetailDTO Product { get; set; } = null!;
        public CompanyDetailsDTO Company { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string PaymentStatus { get; set; } = null!;
        public int UserId { get; set; }
        public String Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }





    public class OrderListDTO
    {
        public int Id { get; set; }
        public int OrderNumber { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string PaymentStatus { get; set; } = null!;
        public int UserId { get; set; }
        public String Status { get; set; } = null!;
    }

    public class UserOrdersDTO
    {
        public int UserId { get; set; }
        public IEnumerable<OrderListDTO> Orders { get; set; } = null!;
    }

    public class ProductOrdersDTO
    {
        public int ProductId { get; set; }
        public IEnumerable<OrderListDTO> Orders { get; set; } = null!;
    }

    public class CompanyOrdersDTO
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = null!;
        public IEnumerable<OrderListDTO> Orders { get; set; } = null!;
    }

    public class OrderStatusUpdateDTO
    {
        public String Status { get; set; } = null!;
    }

    public class PaymentStatusUpdateDTO
    {
        public string PaymentStatus { get; set; } = null!;
    }

    public class OrderFilterDTO
    {
        public int? UserId { get; set; }
        public int? OrderNumber { get; set; }
        public string? productName { get; set; }
        public int? ProductId { get; set; }
        public int? CompanyId { get; set; }
        public String? Status { get; set; }
        public string? PaymentStatus { get; set; }
    }

    public class OrderSummaryDTO
    {
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int PendingOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int CancelledOrders { get; set; }
    }

    public class CompanyOrderSummaryDTO
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = null!;
        public OrderSummaryDTO Summary { get; set; } = null!;
    }

    public class TotalRevenueMonthlyYearlyDTO
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal TotalRevenue { get; set; }
    }
    
}