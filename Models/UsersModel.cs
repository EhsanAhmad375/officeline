using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
namespace officeline.Models
{
    public class UsersModel
    {
        [Key]
        public int userId {get; set;}
        public string? fName{get;set;}
        public string? lName{get;set;}
        public string? role{get;set;}
        [EmailAddress] public string? email{get;set;}
        [Phone] public string? PhoneNumber{get;set;}
        public string? password{get;set;}
        public string? dob{get;set;}
        public CompanyModel? Company {get;set;}
        public int? CompanyId {get;set;}
        public int? userNumber {get;set;}
        public string? profile_pic{get;set;}

    }
}