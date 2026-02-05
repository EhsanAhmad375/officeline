using Microsoft.EntityFrameworkCore;
using officeline.DTO;
using officeline.Data;
using officeline.Models;

namespace officeline.repo
{
    public interface IOrderRepo
    {
        Task<OrderDetailDTO> CreateOrderAsync(OrderCreateDTO orderCreateDTO, int userId);
        Task<IEnumerable<OrderListDTO>> GetAllOrdersAsync();
        Task<OrderDetailDTO?> GetOrderByIdAsync(int id);
        Task<IEnumerable<OrderListDTO>> GetOrdersByUserIdAsync(int userId);
        Task<IEnumerable<OrderListDTO>> GetOrdersByProductIdAsync(int productId);
        Task<IEnumerable<OrderListDTO>> GetOrdersByCompanyIdAsync(int companyId);
    }

    public class OrderRepo : IOrderRepo
    {
        private readonly AppDbContext _context;

        public OrderRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<OrderDetailDTO> CreateOrderAsync(OrderCreateDTO orderCreateDTO, int userId)
        {
            // Fetch the actual ProductModel entity (not DTO) to get all properties
            var productEntity = await _context.Products
                .Include(p => p.Company)
                .FirstOrDefaultAsync(p => p.Id == orderCreateDTO.ProductId);

            if (productEntity == null) throw new Exception("Product not found");

            var isStockedProduct= productEntity.Stock>= orderCreateDTO.Quantity;
            if (!isStockedProduct) throw new Exception("Not enough stock for the product");

            int targetCompanyId = productEntity.CompanyId;

            // Calculate Order Number based on Company
            int lastOrderNumber = await _context.Orders
                .Where(o => o.Product.CompanyId == targetCompanyId)
                .MaxAsync(o => (int?)o.OrderNumber) ?? 0;

            int newOrderNumber = lastOrderNumber + 1;

            // Calculate TotalPrice: Product Price * Quantity
            decimal totalPrice = productEntity.Price * orderCreateDTO.Quantity;

            var newOrder = new OrderModel
            {
                OrderNumber = newOrderNumber,
                ProductId = orderCreateDTO.ProductId,
                Quantity = orderCreateDTO.Quantity,
                TotalPrice = totalPrice,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            // Fetch with full navigation chain for the Detail DTO
            var savedOrder = await _context.Orders
                .Include(o => o.Product)
                    .ThenInclude(p => p.Company)
                .FirstAsync(o => o.Id == newOrder.Id);

            return MapToDetailDTO(savedOrder);
        }

        public async Task<IEnumerable<OrderListDTO>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Select(o => MapToListDTO(o))
                .ToListAsync();
        }

        public async Task<OrderDetailDTO?> GetOrderByIdAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Product)
                    .ThenInclude(p => p.Company)
                .FirstOrDefaultAsync(o => o.Id == id);

            return order == null ? null : MapToDetailDTO(order);
        }

        public async Task<IEnumerable<OrderListDTO>> GetOrdersByUserIdAsync(int userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .Select(o => MapToListDTO(o))
                .ToListAsync();
        }

        public async Task<IEnumerable<OrderListDTO>> GetOrdersByProductIdAsync(int productId)
        {
            return await _context.Orders
                .Where(o => o.ProductId == productId)
                .Select(o => MapToListDTO(o))
                .ToListAsync();
        }

        public async Task<IEnumerable<OrderListDTO>> GetOrdersByCompanyIdAsync(int companyId)
        {
            return await _context.Orders
                .Include(o => o.Product)
                .Where(o => o.Product.CompanyId == companyId)
                .Select(o => MapToListDTO(o))
                .ToListAsync();
        }

        // --- Helper Mappings ---

        private static OrderListDTO MapToListDTO(OrderModel o) => new OrderListDTO
        {
            Id = o.Id,
            OrderNumber = o.OrderNumber ?? 0,
            ProductId = o.ProductId,
            Quantity = o.Quantity,
            TotalPrice = o.TotalPrice,
            PaymentStatus = o.PaymentStatus,
            UserId = o.UserId,
            Status = o.Status
        };

        private static OrderDetailDTO MapToDetailDTO(OrderModel o) => new OrderDetailDTO
        {
            Id = o.Id,
            OrderNumber = o.OrderNumber ?? 0,
            Quantity = o.Quantity,
            TotalPrice = o.TotalPrice,
            PaymentStatus = o.PaymentStatus,
            UserId = o.UserId,
            Status = o.Status,
            CreatedAt = o.CreatedAt,
            UpdatedAt = o.UpdatedAt,
            Product = new ProductDetailDTO 
            { 
                Id = o.Product.Id, 
                Name = o.Product.Name, 
                Price = o.Product.Price,
                // These must exist in ProductDetailDTO to fix your error:
                CompanyId = o.Product.CompanyId,
                CompanyName = o.Product.Company?.Name 
            },
            Company = new CompanyDetailsDTO
            {
                Id = o.Product.Company?.CompanyId ?? 0, 
                Name = o.Product.Company?.Name ?? "Unknown"
            }
        };
    }
}