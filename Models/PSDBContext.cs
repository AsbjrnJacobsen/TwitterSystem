using Microsoft.EntityFrameworkCore;

namespace Models;

public class PSDBContext : DbContext
{
    public DbSet<Post> Posts { get; set; }
    
    public PSDBContext(DbContextOptions<PSDBContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Posts Table
        modelBuilder.Entity<Post>(entity =>
        {
            //Primary Key
            entity.HasKey(p => p.postID);
            entity.Property(p => p.postID).ValueGeneratedOnAdd();
            
            //Columns
            entity.Property(p => p.timestamp);
            entity.Property(p => p.repliedToPostID);
            entity.Property(p => p.username);
            entity.Property(p => p.content);
        });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            "Server=post-db;Initial Catalog=PSDB;User ID=sa;Password=pepsi1234!;TrustServerCertificate=True;");
    }
}