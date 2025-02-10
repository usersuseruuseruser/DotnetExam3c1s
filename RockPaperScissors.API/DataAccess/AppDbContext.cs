using api.Domain;
using Microsoft.EntityFrameworkCore;

namespace api.DataAccess;

public class AppDbContext: DbContext
{
    public DbSet<User> Users { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}