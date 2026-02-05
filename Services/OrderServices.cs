using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using officeline.DTO;
using officeline.Models;
using officeline.repo;
using officeline.ErrorExceptions;
 
 namespace officeline.Services
{
    public interface IOrderService
    {
        Task<OrderDetailDTO> CreateOrderAsync(OrderCreateDTO orderCreateDTO, int userId);
        Task<IEnumerable<OrderListDTO>> GetAllOrdersAsync();
        Task<OrderDetailDTO?> GetOrderByIdAsync(int id);
        Task<IEnumerable<OrderListDTO>> GetOrdersByUserIdAsync(int userId);
        Task<IEnumerable<OrderListDTO>> GetOrdersByProductIdAsync(int productId);
        Task<IEnumerable<OrderListDTO>> GetOrdersByCompanyIdAsync(int companyId);
    }
    public class OrderService : IOrderService
    {
        private readonly IOrderRepo _orderRepo;

        public OrderService(IOrderRepo orderRepo)
        {
            _orderRepo = orderRepo;
        }

        public async Task<OrderDetailDTO> CreateOrderAsync(OrderCreateDTO orderCreateDTO, int userId)
        {
            var order = await _orderRepo.CreateOrderAsync(orderCreateDTO, userId);
            Console.WriteLine($"Order created with ID: {order.Id}");
            return order;
        }

        public async Task<IEnumerable<OrderListDTO>> GetAllOrdersAsync()
        {
            var orders = await _orderRepo.GetAllOrdersAsync();
            if(!orders.Any())
            {
                throw new ApiException("orders","No orders found.");
            }
            return orders;
        }

        public async Task<OrderDetailDTO?> GetOrderByIdAsync(int id)
        {
            var order = await _orderRepo.GetOrderByIdAsync(id);
            if(order == null)
            {
                throw new ApiException("orders", $"Order with ID {id} not found.");
            }
            return order;
        }

        public async Task<IEnumerable<OrderListDTO>> GetOrdersByUserIdAsync(int userId)
        {
            var orders = await _orderRepo.GetOrdersByUserIdAsync(userId);
            if(!orders.Any())
            {
                throw new ApiException("orders", $"No orders found for user ID {userId}.");
            }
            return orders;
        }

        public async Task<IEnumerable<OrderListDTO>> GetOrdersByProductIdAsync(int productId)
        {
            var orders = await _orderRepo.GetOrdersByProductIdAsync(productId);
            if(!orders.Any())
            {
                throw new ApiException("orders", $"No orders found for product ID {productId}.");
            }
            return orders;
        }

        public async Task<IEnumerable<OrderListDTO>> GetOrdersByCompanyIdAsync(int companyId)
        {
            var orders = await _orderRepo.GetOrdersByCompanyIdAsync(companyId);
            if(!orders.Any())
            {
                throw new ApiException("orders", $"No orders found for company ID {companyId}.");
            }
            return orders;
        }
    }
    
}