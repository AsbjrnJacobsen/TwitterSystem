using Microsoft.EntityFrameworkCore;

namespace Models;

public class ASDBContext : DbContext
{
    public DbSet<Account> Accounts { get; set; }
    
    public ASDBContext(DbContextOptions<ASDBContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Account Table
        modelBuilder.Entity<Account>(entity =>
        {
            //Primary Key
            entity.HasKey(p => p.ID);
            entity.Property(p => p.ID).ValueGeneratedOnAdd();
            
            //Columns
            entity.Property(p => p.Username);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.Property(p => p.Password);
            entity.Property(p => p.Email);
            entity.Property(p => p.Bio);
        });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            "Server=account-db;Initial Catalog=ASDB;User ID=sa;Password=pepsi1234!;TrustServerCertificate=True;");
    }
}