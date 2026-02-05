using Microsoft.EntityFrameworkCore;

namespace officeline.DTO
{
    public class ProductCreateDTO
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string SubCategory { get; set; } = null!;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int CompanyId { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class ProductUpdateDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? SubCategory { get; set; }
        public decimal? Price { get; set; }
        public int? Stock { get; set; }
    }

    public class ProductListDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Category { get; set; } = null!;
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }

    public class ProductDetailDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? SubCategory { get; set; }
        public int Stock { get; set; }
        public int CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class companyProductDTO
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = null!;
        public IEnumerable<ProductListDTO> Products { get; set; } = null!;
    }
}