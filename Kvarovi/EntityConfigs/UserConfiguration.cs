namespace Kvarovi.EntityConfigs;

using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    
 public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.UserId);
        builder.HasIndex(u => u.DeviceToken).IsUnique();
        builder.HasMany(u => u.Keywords).WithMany(k => k.Users)
            .UsingEntity<KeywordUser>();
        builder.HasMany(u => u.Announcements).WithMany(a => a.Users);
        builder.HasMany(u => u.ApiKeys).WithOne(apk => apk.User);
    }}
