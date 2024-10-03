using Microsoft.EntityFrameworkCore;
using Models;

namespace TimelineService;

public class TSDBContext : DbContext
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Post> Posts { get; set; }
    
    public TSDBContext(DbContextOptions<TSDBContext> options) : base(options) { }
}
