using Microsoft.EntityFrameworkCore;
using officeline.Models; // Yahan 'officeline' kar diya

namespace officeline.Data; // Yahan 'officeline' kar diya

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<CompanyModel> Companies { get; set; }
    public DbSet<UsersModel>  Users{get;set;}
}