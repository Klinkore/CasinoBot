using BotData.Entity;
using Microsoft.EntityFrameworkCore;

public class BotContext : DbContext
{
    public DbSet<BotUser> Users => Set<BotUser>();
    public BotContext() => Database.EnsureCreated();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=telegram.db");
    }
}