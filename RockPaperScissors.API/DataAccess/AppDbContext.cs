using api.Domain;
using Microsoft.EntityFrameworkCore;

namespace api.DataAccess;

public class AppDbContext: DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChatMessage>()
            .HasOne(cm => cm.Game)
            .WithMany(g => g.Messages);

        modelBuilder.Entity<Game>()
            .HasOne(g => g.Creator)
            .WithMany(u => u.Games);

        modelBuilder.Entity<Game>()
            .HasOne(g => g.SecondPlayer);
        
        
        base.OnModelCreating(modelBuilder);
    }
}