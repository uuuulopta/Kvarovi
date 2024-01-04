namespace Kvarovi.Contexts;

using Entities;
using EntityConfigs;
using Microsoft.EntityFrameworkCore;

public class MySqlContext : DbContext
{
    public MySqlContext(DbContextOptions<MySqlContext> options): base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLowerCaseNamingConvention();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new UserConfiguration().Configure(modelBuilder.Entity<User>());
        new AnnouncementTypeConfiguration().Configure(modelBuilder.Entity<AnnouncementType>());
        new AnnouncementConfiguration().Configure(modelBuilder.Entity<Announcement>());
        new KeywordConfiguration().Configure(modelBuilder.Entity<Keyword>());
    }
    
}
