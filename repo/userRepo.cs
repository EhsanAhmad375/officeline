using Microsoft.AspNetCore;
using officeline.Data;
using officeline.DTO;
using officeline.Models;
using Microsoft.EntityFrameworkCore;

namespace officeline.repo
{
    public interface IUserRepo
    {
        Task<List<UsersModel>> GetAllUsersAsync();
        Task<UsersModel> GetUserByIdAsync(int id);
        Task<UsersModel> GetUserByEmailAsync(string email);
        Task<int> GetMaxUserNumerOfACompanyAsync(int companyId);
        Task<UsersModel> AddUserAsync(UsersModel user);
        Task UpdateUserAsync(UsersModel user);
        Task<bool> DeleteUserAsync(int id);

        Task SaveChangesAsync();
    }

    public class UserRepo : IUserRepo
    {
        public readonly AppDbContext _context;
        public UserRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<List<UsersModel>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }
        public async Task<UsersModel?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<UsersModel?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.email == email);
        }

        public async Task<int> GetMaxUserNumerOfACompanyAsync(int companyId)
        {
            int maxUserNumber = await _context.Users.Where(u=> u.CompanyId == companyId).
            Select(u=> u.userNumber).MaxAsync()??0;
            
            return maxUserNumber;
        }


        public async Task<UsersModel> AddUserAsync(UsersModel user){
            var result= await _context.Users.AddAsync(user);

            return result.Entity;
        }
        public async Task UpdateUserAsync(UsersModel user)
        {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        }





        public async Task<bool> DeleteUserAsync(int id)
        {
            var user=await GetUserByIdAsync(id);
            if(user==null)
            {
                return false;
            }
            _context.Users.Remove( user);;
            await SaveChangesAsync();
            return true;
        }

    }
}