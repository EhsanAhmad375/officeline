using officeline.DTO;
using officeline.Models;
using officeline.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using officeline.ErrorExceptions;   
using officeline.repo;
using Microsoft.AspNetCore.JsonPatch;
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

    private readonly ICompanyRepo _companyRepo;
    
    public CompanyServices(AppDbContext context, ICompanyRepo companyRepo)
    {
        _context = context;
        _companyRepo = companyRepo;
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
        // var companiesList = await _context.Companies.Select(c=> new CompaniesListDTO
        // {
        //     CompanyId = c.CompanyId,
        //     Name = c.Name,
        //     Email = c.Email 
        // }).ToListAsync();
        var companies = await _companyRepo.GetAllCompaniesAsync();

        var companiesList =companies.Select(c=> new CompaniesListDTO
        {
            CompanyId=c.CompanyId,
            Name=c.Name??"",
            Email=c.Email
        }).ToList();
        

        return companiesList; 
    }



    public async Task<UpdateCompanyDTO> UpdateCompanyAsync(int companyId, UpdateCompanyDTO updateCompanyDTO)
    {
        var company = await _companyRepo.UpdateCompanyAsync(companyId, updateCompanyDTO);

        if (company == null)
        {
            throw new ApiException("message","Company not found");
        }
        return updateCompanyDTO;
    }


    public async Task<bool> deleteCompany(int companyId)
{
    // 1. Repository ka method use karein (await ke sath)
    var success = await _companyRepo.DeleteCompanyAsync(companyId);

    if (!success)
    {
        throw new ApiException("message", "Company not found");
    }

    return true;
} 
}
