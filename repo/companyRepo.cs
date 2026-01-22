using Microsoft.OpenApi.Models;
using officeline.Models;
using officeline.Data;
using Microsoft.EntityFrameworkCore;
using officeline.DTO;
namespace officeline.repo
{
        public interface ICompanyRepo
        {
            Task<List<CompanyModel>> GetAllCompaniesAsync();
            Task<CompanyModel> GetCompanyByIdAsync(int id);
            Task<CompanyModel> AddCompanyAsync(CompanyModel company);
            Task<CompanyModel> UpdateCompanyAsync(int id, UpdateCompanyDTO company);
            Task<bool> DeleteCompanyAsync(int id);

            Task SaveChangesAsync();
        }


        public class CompanyRepo : ICompanyRepo
    {
        public readonly AppDbContext _context;
        public CompanyRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

                                                
                                        
        public async Task<List<CompanyModel>> GetAllCompaniesAsync()
        {
            return await _context.Companies.ToListAsync();
        }

        public async Task<CompanyModel?> GetCompanyByIdAsync(int id)
                {
                return await _context.Companies.FindAsync(id);
                }
        public async Task<CompanyModel> AddCompanyAsync(CompanyModel company)
        {
            var result= await _context.Companies.AddAsync(company);

            return result.Entity;
        }

        public async Task<CompanyModel> UpdateCompanyAsync(int id,UpdateCompanyDTO company)
        {
            var existingCompany =await GetCompanyByIdAsync(id);
            if (existingCompany == null)
            {
                return null;
            }

            existingCompany.Name = company.Name;
            existingCompany.Email = company.Email;
            existingCompany.CompleteAddress = company.CompleteAddress;
            existingCompany.PhoneNumber = company.PhoneNumber;
            existingCompany.Address = company.Address;
            existingCompany.City = company.City;
            existingCompany.State = company.State;
            existingCompany.ZipCode = company.ZipCode;
            existingCompany.Country = company.Country;

            await SaveChangesAsync();
            return existingCompany;
        }

        public async Task<bool> DeleteCompanyAsync(int id)
        {
            var company = await GetCompanyByIdAsync(id);
            if (company == null)
            {
                return false;
            }

            _context.Companies.Remove(company);
            await SaveChangesAsync();
            return true;
        }

        
    }
        
    
}