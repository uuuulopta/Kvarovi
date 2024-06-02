namespace Kvarovi.Contexts;

using Entities;
using EntityConfigs;
using Microsoft.EntityFrameworkCore;

public class MySqlContext : DbContext
{
   public DbSet<Announcement> Announcements { get; set; } = null!;
   public DbSet<AnnouncementType> AnnouncementTypes { get; set; } = null!;
   public DbSet<Keyword> Keywords { get; set; } = null!;
   public DbSet<User> Users { get; set; } = null!;
   public DbSet<KeywordUser> KeywordUsers { get; set; } = null!;

   public DbSet<Notification> Notifications { get; set; } = null!;
   public DbSet<ApiKey> ApiKeys { get; set; } = null!;
   
    
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
        new NotificationConfiguration().Configure(modelBuilder.Entity<Notification>());
        modelBuilder.Entity<ApiKey>().HasKey(apk => apk.ApiKeyId);
        modelBuilder.Entity<ApiKey>().Property(apk => apk.Value).IsRequired();
    }
    
}
