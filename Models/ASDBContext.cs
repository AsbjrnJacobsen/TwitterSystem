using Microsoft.EntityFrameworkCore;

namespace Models;

public class ASDBContext : DbContext
{
    public virtual DbSet<Account> Accounts { get; set; }
    // Empty constructor for testing
    public ASDBContext() { }
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
        string connectionString = Environment.GetEnvironmentVariable("ASDBConnectionString");
        optionsBuilder.UseSqlServer(connectionString);
    }
}