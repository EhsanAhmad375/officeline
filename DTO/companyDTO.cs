using Microsoft.AspNetCore;
using System.ComponentModel.DataAnnotations;

namespace officeline.DTO
{
    public class CreateCompanyDTO
    {
    
        [Required(ErrorMessage = "Company name is required.")]
        [StringLength(100)] public string Name { get; set; }

        [Required(ErrorMessage = "Company Email is required.")]
        [EmailAddress] public string Email { get; set; }

        [Required(ErrorMessage = "Company address is required.")]
        [StringLength(50)] public string CompleteAddress { get; set; }
        [Phone] public string PhoneNumber { get; set; }
        
    }
    public class CompaniesListDTO
    {
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }


    public class CompanyDetailsDTO
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string CompleteAddress { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class UpdateCompanyDTO
    {
        
        [Required][StringLength(100)] public string Name { get; set; }
        [EmailAddress] public string Email { get; set; }
        [StringLength(200)] public string Address { get; set; }
        [StringLength(100)] public string City { get; set; }
        [StringLength(50)] public string State { get; set; }
        [StringLength(50)] public string CompleteAddress { get; set; }
        [StringLength(20)] public string ZipCode { get; set; }
        [StringLength(100)] public string Country { get; set; }
        [Phone] public string PhoneNumber { get; set; }
    }
}