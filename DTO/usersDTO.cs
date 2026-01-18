using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace officeline.DTO
{
    public class CreateUserDTO
    {
        [Required(ErrorMessage ="First Name is Required")]
        public string fName{get;set;}

        [Required(ErrorMessage ="Last Name is Required")]
        public string lName{get;set;}

        [EmailAddress]
        [Required(ErrorMessage ="Email is Required")]
        public string email{get;set;}

        [Required(ErrorMessage ="Select Your Company")]
        public int CompanyId{get;set;}

        [Required(ErrorMessage ="Enter Password")]
        public string password{get;set;}
        
    }

    public class LoginUserDTO
    {
        [Required(ErrorMessage ="Email Address Required")]
        [EmailAddress]
        public string email{get;set;}

        [Required(ErrorMessage ="Password Required")]
        public string password{get;set;}
    }

    public class LoginDetailsDTO
    {
         [Key]
        public int userId {get; set;}
        public string? fName{get;set;}
        public string? lName{get;set;}
        public string? role{get;set;}
        public int? CompanyId {get;set;}

        public string token{get;set;}
    }
    public class GetAllUsersDTO
    {
        public int userId {get; set;}
        public string? fName{get;set;}
        public string? lName{get;set;}
        public string? role{get;set;}
        public int? CompanyId {get;set;}
    }

    public class GetUserDetailDTO
    {
        public int userId {get; set;}
        public int? userNumber {get;set;}
        public string? fName{get;set;}
        public string? lName{get;set;}
        public string? role{get;set;}
        public string? email{get;set;}
        public string? PhoneNumber{get;set;}
        public string? dob{get;set;}
        public int? CompanyId {get;set;}
    }

    public class updateUserProfileDTO
    {
        
        
        public string? fName{get;set;}
        public string? lName{get;set;}
        public string? PhoneNumber{get;set;}
        public string? dob{get;set;}
        

    
    }



    
}