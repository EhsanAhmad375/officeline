using System.ComponentModel.DataAnnotations;

namespace officeline.Models // Yahan 'officeline' kar diya
{
    public class CompanyModel
    {
        [Key]
         public int CompanyId { get; set; }
        [Required][StringLength(100)] public string? Name { get; set; }
        [StringLength(200)] public string? Address { get; set; }
        [StringLength(100)] public string? City { get; set; } 
        [StringLength(50)] public string? State { get; set; }
        [StringLength(50)] public string? CompleteAddress { get; set; }
        [StringLength(20)] public string? ZipCode { get; set; }
        [StringLength(100)] public string? Country { get; set; }
        [Phone] public string? PhoneNumber { get; set; }
        [EmailAddress] public string Email { get; set; }
    }
}