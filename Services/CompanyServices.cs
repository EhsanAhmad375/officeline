using officeline.DTO;
using officeline.Models;
using officeline.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using officeline.ErrorExceptions;   
namespace officeline.Services;

public interface ICompanyServices
{
    Task<CompanyModel> CreateCompanyAsync(CreateCompanyDTO companyDto);

    Task<List<CompaniesListDTO>> GetAllCompaniesAsync();

    Task<UpdateCompanyDTO> UpdateCompanyAsync(int companyId, UpdateCompanyDTO updateCompanyDTO);
    
    Task<bool> deleteCompany(int companyId);
 
}





public class CompanyServices : ICompanyServices
{
    private readonly AppDbContext _context;
    
    public CompanyServices(AppDbContext context)
    {
        _context = context;
    }




    public async Task<CompanyModel> CreateCompanyAsync(CreateCompanyDTO createCompanyDTO)
    {
        bool isExistingEmail = await _context.Companies.AnyAsync(c=>c.Email == createCompanyDTO.Email);
        if (isExistingEmail)
        {
            throw new ApiException("Email", $"The email {createCompanyDTO.Email} is already in use");

        }
        if(createCompanyDTO.PhoneNumber == null && createCompanyDTO.PhoneNumber.Length<11)
        {
            throw new ApiException("PhoneNumber", "Invalid phone number");
        }
        var company = new CompanyModel
        {
            Name = createCompanyDTO.Name,
            Email = createCompanyDTO.Email,
            CompleteAddress = createCompanyDTO.CompleteAddress,
            PhoneNumber = createCompanyDTO.PhoneNumber
        };
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();
        return company;
    }



    public async Task<List<CompaniesListDTO>> GetAllCompaniesAsync()
    {
        var companiesList = await _context.Companies.Select(c=> new CompaniesListDTO
        {
            CompanyId = c.CompanyId,
            Name = c.Name,
            Email = c.Email 
        }).ToListAsync();

        return companiesList; 
    }



    public async Task<UpdateCompanyDTO> UpdateCompanyAsync(int companyId, UpdateCompanyDTO updateCompanyDTO)
    {
        var company = await _context.Companies.FindAsync(companyId);
        if(company == null)
        {
            throw new ApiException("CompanyId", "Company not found");
        }
        company.Name = updateCompanyDTO.Name;
        company.Email = updateCompanyDTO.Email;
        company.Address = updateCompanyDTO.Address;
        company.City = updateCompanyDTO.City;
        company.State = updateCompanyDTO.State;
        company.CompleteAddress = updateCompanyDTO.CompleteAddress;
        company.ZipCode = updateCompanyDTO.ZipCode;
        company.Country = updateCompanyDTO.Country;
        company.PhoneNumber = updateCompanyDTO.PhoneNumber; 
        await _context.SaveChangesAsync();
        return updateCompanyDTO;
    }


    public async Task<bool> deleteCompany(int companyId)
    {
        var company=await _context.Companies.FindAsync(companyId);
        if (company == null)
        {
            throw new ApiException("message","Company not found");
        }

        _context.Companies.Remove(company);
        await _context.SaveChangesAsync();
        return true;
        
        
    } 
}
