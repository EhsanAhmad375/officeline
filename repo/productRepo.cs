using Microsoft.EntityFrameworkCore;
using officeline.DTO;
using officeline.Data;
using officeline.Models;
namespace officeline.repo
{
    public interface IProductRepo
    {
        Task<IEnumerable<ProductListDTO>> GetAllProductsAsync();
        Task<ProductDetailDTO?> GetProductByIdAsync(int id);
        Task<ProductDetailDTO> CreateProductAsync(ProductCreateDTO productCreateDTO, int companyId, int userId);
        Task<bool> UpdateProductAsync(int id, ProductUpdateDTO productUpdateDTO);
        Task<bool> DeleteProductAsync(int id);
    }

    public class ProductRepo : IProductRepo
    {
        private readonly AppDbContext _context;

        public ProductRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductListDTO>> GetAllProductsAsync()
        {
            return await _context.Products
                .Select(p => new ProductListDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Stock=p.Stock,
                    Category=p.Category
                })
                .ToListAsync();
        }

        public async Task<ProductDetailDTO?> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Where(p => p.Id == id)
                .Select(p => new ProductDetailDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Stock = p.Stock,
                    CompanyName = p.Company.Name,
                    CompanyId = p.CompanyId,
                })
                .FirstOrDefaultAsync();
        }
    
    
public async Task<ProductDetailDTO> CreateProductAsync(ProductCreateDTO dto, int companyId, int userId)
{
    
    var product = new ProductModel 
    {
        Name = dto.Name,
        Description = dto.Description,
        Price = dto.Price,
        Stock = dto.Stock,
        Category = dto.Category,
        SubCategory = dto.SubCategory,
        CompanyId = companyId,
        UserId = userId,
        CreatedAt = DateTime.UtcNow
    };

    _context.Products.Add(product);
    await _context.SaveChangesAsync();

    
    return new ProductDetailDTO
    {
        Id = product.Id,
        Name = product.Name,
        Description = product.Description,
        Category = product.Category,
        SubCategory = product.SubCategory,
        Price = product.Price,
        Stock = product.Stock,
        CreatedAt = product.CreatedAt
    };
}

        public async Task<bool> UpdateProductAsync(int id, ProductUpdateDTO dto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            
            if (dto.Name != null) product.Name = dto.Name;
            if (dto.Description != null) product.Description = dto.Description;
            if (dto.Price.HasValue) product.Price = dto.Price.Value;
            if (dto.Stock.HasValue) product.Stock = dto.Stock.Value;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}