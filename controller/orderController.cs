using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using officeline.Data;   // 'officeline'
using officeline.Models; // 'officeline'
using officeline.DTO;    // 'officeline'
using officeline.Services;
using officeline.ErrorExceptions;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization; // 'officeline'
namespace officeline.Controllers // 'officeline'
{
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [Authorize()]
        [HttpPost("create")]
        public async Task<ActionResult<OrderDetailDTO>> CreateOrder([FromBody] OrderCreateDTO orderCreateDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid input data", errors = ModelState });
            }
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("UserId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId) || userId <= 0)
                {
                    return Unauthorized(new { success = false, message = "Invalid or missing user claim" });
                }

                var result = await _orderService.CreateOrderAsync(orderCreateDTO, userId);
                return StatusCode(201, new { success = true, message = "Order created successfully", data = result });
            }
            catch (ApiException ex)
            {
                return StatusCode(401, new { success = false, errors = new Dictionary<string, string> { { ex.FieldName, ex.Message } } });
            }
            catch (DbUpdateException dbEx)
            {
                var inner = dbEx.GetBaseException()?.Message ?? dbEx.Message;
                if (inner.Contains("FK_Orders_Users_UserId") || inner.Contains("foreign key") || inner.Contains("UserId"))
                {
                    return BadRequest(new { success = false, message = "Invalid user or related entity not found" });
                }
                return StatusCode(500, new { success = false, message = "Database error creating order" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to create order", error = ex.Message });
            }
        }
        [Authorize()]
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<OrderListDTO>>> GetAllOrders()
        {
            try
            {
                var result = await _orderService.GetAllOrdersAsync();
                return Ok(new { success = true, data = result });
            }
            catch (ApiException ex)
            {
                return StatusCode(401, new { success = false, errors = new Dictionary<string, string> { { ex.FieldName, ex.Message } } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to retrieve orders", error = ex.Message });
            }
        }
        [Authorize()]
        [HttpGet("by-user/{userId}")]
        public async Task<ActionResult<IEnumerable<OrderListDTO>>> GetOrdersByUserId(int userId)
        {
            try
            {
                var result = await _orderService.GetOrdersByUserIdAsync(userId);
                return Ok(new { success = true, data = result });
            }
            catch (ApiException ex)
            {
                return StatusCode(401, new { success = false, errors = new Dictionary<string, string> { { ex.FieldName, ex.Message } } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to retrieve orders", error = ex.Message });
            }
        }
        [Authorize()]
        [HttpGet("by-product/{productId}")]
        public async Task<ActionResult<IEnumerable<OrderListDTO>>> GetOrdersByProductId(int productId)
        {
            try
            {
                var result = await _orderService.GetOrdersByProductIdAsync(productId);
                return Ok(new { success = true, data = result });
            }
            catch (ApiException ex)
            {
                return StatusCode(401, new { success = false, errors = new Dictionary<string, string> { { ex.FieldName, ex.Message } } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to retrieve orders", error = ex.Message });
            }
        }
        [Authorize()]
        [HttpGet("by-company/{companyId}")]
        public async Task<ActionResult<IEnumerable<OrderListDTO>>> GetOrdersByCompanyId(int companyId)
        {
            try
            {
                var result = await _orderService.GetOrdersByCompanyIdAsync(companyId);
                return Ok(new { success = true, data = result });
            }
            catch (ApiException ex)
            {
                return StatusCode(401, new { success = false, errors = new Dictionary<string, string> { { ex.FieldName, ex.Message } } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to retrieve orders", error = ex.Message });
            }
        }
        [Authorize()]
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDetailDTO>> GetOrderById(int id)
        {
            try
            {
                var result = await _orderService.GetOrderByIdAsync(id);
                return Ok(new { success = true, data = result });

            }
            catch (ApiException ex)
            {
                return StatusCode(401, new { success = false, errors = new Dictionary<string, string> { { ex.FieldName, ex.Message } } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to retrieve order", error = ex.Message });
            }
        }
        
    }
}