using System.ComponentModel.DataAnnotations.Schema;

namespace officeline.Models
{
    public class ProductModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string SubCategory { get; set; } = null!;
        public decimal Price { get; set; }
        public int Stock { get; set; }

        // Company Mapping
        public int CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public CompanyModel Company { get; set; } = null!;
        
        // User Mapping (Fixing the ghost columns)
        [Column("userId")] // Database ke actual column se map karega
        public int UserId { get; set; }

        [ForeignKey("UserId")] // Oopar wali property (UserId) ko as a FK use karega
        public UsersModel users { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}