using Microsoft.AspNetCore.Mvc;
using officeline.DTO;
using officeline.Services;
using System.Threading.Tasks;
using officeline.ErrorExceptions;

namespace officeline.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductServices _productServices;

        public ProductController(IProductServices productServices)
        {
            _productServices = productServices;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts(){
            try
            {
            var products = await _productServices.GetAllProductsAsync();
            return Ok(products);    
            }catch(ApiException ex)
            {
                return BadRequest(new {success=false, error=new Dictionary<string,string>{{ex.FieldName, ex.Message}}});
            }catch(Exception ex)
            {
                return StatusCode(500, new {success=false, error="An unexpected server error occurred."});
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var product = await _productServices.GetProductByIdAsync(id);
                return Ok(product);
            }catch(ApiException ex)
            {
                return NotFound(new {success=false, error=new Dictionary<string,string>{{ex.FieldName, ex.Message}}});
            }catch(Exception ex)
            {
                return StatusCode(500, new {success=false, error="An unexpected server error occurred."});
            }
        }
        [HttpPost("company/{companyId}/user/{userId}")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductCreateDTO productCreateDTO, [FromRoute] int companyId, [FromRoute] int userId)
        {   
            try
            {
                var createdProduct = await _productServices.CreateProductAsync(productCreateDTO, companyId, userId);
                return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.Id }, createdProduct);
            }catch(ApiException ex)
            {
                return BadRequest(new {success=false, error=new Dictionary<string,string>{{ex.FieldName, ex.Message}}});
            } catch (Exception ex) {
            return StatusCode(500, new { success = false, error = ex.Message, inner = ex.InnerException?.Message });
            }
        }
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductUpdateDTO productUpdateDTO)
        {
            try
            {
                var isUpdated = await _productServices.UpdateProductAsync(id, productUpdateDTO);
                if (isUpdated)
                {
                    return NoContent();
                }
                return NotFound(new {success=false, error="Product not found."});
            }catch(ApiException ex)
            {
                return BadRequest(new {success=false, error=new Dictionary<string,string>{{ex.FieldName, ex.Message}}});
            }catch(Exception ex)
            {
                return StatusCode(500, new {success=false, error="An unexpected server error occurred."});
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var isDeleted = await _productServices.DeleteProductAsync(id);
                if (isDeleted)
                {
                    return NoContent();
                }
                return NotFound(new {success=false, error="Product not found."});
            }catch(ApiException ex)
            {
                return BadRequest(new {success=false, error=new Dictionary<string,string>{{ex.FieldName, ex.Message}}});
            }catch(Exception ex)
            {
                return StatusCode(500, new {success=false, error="An unexpected server error occurred."});
            }
        }
    }
}