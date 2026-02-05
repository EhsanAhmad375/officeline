using Microsoft.EntityFrameworkCore;
using officeline.Models; // Yahan 'officeline' kar diya

namespace officeline.Data; // Yahan 'officeline' kar diya

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<CompanyModel> Companies { get; set; }
    public DbSet<UsersModel>  Users{get;set;}
    public DbSet<ProductModel> Products { get; set; }
    public DbSet<OrderModel> Orders { get; set; }
    public DbSet<ChatsModel> Chats { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
     base.OnModelCreating(modelBuilder);

     modelBuilder.Entity<ChatsModel>().
     HasOne(c=>c.Sender).WithMany().
     HasForeignKey(c=>c.SenderId).
     OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<ChatsModel>().
        HasOne(c => c.Receiver).WithMany().
        HasForeignKey(c => c.ReceiverId).
        OnDelete(DeleteBehavior.SetNull);



    }


}