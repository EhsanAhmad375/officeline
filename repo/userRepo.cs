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
        Task<UsersModel> AddUserAsync(UsersModel user);
        Task<UsersModel> UpdateUserProfileAsync(int id, updateUserProfileDTO user);
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


        public async Task<UsersModel> AddUserAsync(UsersModel user){
            var result= await _context.Users.AddAsync(user);

            return result.Entity;
        }
        public async Task<UsersModel> UpdateUserProfileAsync(int id,updateUserProfileDTO user)
        {
            var existingUser =await GetUserByIdAsync(id);
            if (existingUser == null)
            {
                return null;
            }

            existingUser.fName = user.fName;
            existingUser.lName = user.lName;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.dob = user.dob;
            existingUser.profile_pic = user.profilepic.ToString();

            return existingUser;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user=GetUserByIdAsync(id);
            if(user==null)
            {
                return false;
            }
            _context.Users.Remove(await user);;
            await SaveChangesAsync();
            return true;
        }

    }
}