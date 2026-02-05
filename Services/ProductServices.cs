using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using officeline.DTO;
using officeline.Models;
using officeline.Data;
using officeline.repo;
using officeline.ErrorExceptions;
namespace officeline.Services
{
    public interface IProductServices
    {
        Task<IEnumerable<ProductListDTO>> GetAllProductsAsync();
        Task<ProductDetailDTO?> GetProductByIdAsync(int id);
        Task<ProductDetailDTO> CreateProductAsync(ProductCreateDTO productCreateDTO, int companyId, int userId);
        Task<bool> UpdateProductAsync(int id, ProductUpdateDTO productUpdateDTO);
        Task<bool> DeleteProductAsync(int id);
    }
    public class ProductServices : IProductServices{
        
            private readonly IProductRepo _productRepo;
            private readonly ICompanyRepo _companyRepo;
            
            public ProductServices(IProductRepo productRepo, ICompanyRepo companyRepo)
            {
                _productRepo = productRepo;
                _companyRepo = companyRepo;
                
            }
            public async Task<IEnumerable<ProductListDTO>> GetAllProductsAsync()
            {
                var products = await _productRepo.GetAllProductsAsync();
                if (products == null || !products.Any())
                {
                    throw new ApiException("Product", "No products found.");
                }
                return products;
            }
            public async Task<ProductDetailDTO?> GetProductByIdAsync(int id)
            {
                var product = await _productRepo.GetProductByIdAsync(id);
                 if (product == null)
                {
                    throw new ApiException("Product", $"Product with ID {id} not found.");
                }
                return product;
            }
            public async Task<ProductDetailDTO> CreateProductAsync(ProductCreateDTO productCreateDTO, int companyId, int userId)
            {

                var company = await _companyRepo.GetCompanyByIdAsync(companyId);
                if (company == null)
                {
                    throw new ApiException("Company", $"Company with ID {companyId} not found.");
                }

                return await _productRepo.CreateProductAsync(productCreateDTO, companyId, userId);  
            }
            public async Task<bool> UpdateProductAsync(int id, ProductUpdateDTO productUpdateDTO)
            {
                var existingProduct = await _productRepo.GetProductByIdAsync(id);
                if (existingProduct == null)
                {
                    throw new ApiException("Product", $"Product with ID {id} not found.");
                }
                return await _productRepo.UpdateProductAsync(id, productUpdateDTO);
            }
            public async Task<bool> DeleteProductAsync(int id){
                var existingProduct = await _productRepo.GetProductByIdAsync(id);
                if (existingProduct == null)
            {
                    throw new ApiException("Product", $"Product with ID {id} not found.");
            }

                return await _productRepo.DeleteProductAsync(id);
                }

        }
    }
