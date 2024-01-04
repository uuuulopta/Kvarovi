namespace Kvarovi.EntityConfigs;

using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AnnouncementConfiguration : IEntityTypeConfiguration<Announcement>
{

    public void Configure(EntityTypeBuilder<Announcement> builder)
    {
        builder.HasKey(a => a.AnnouncementId);
        builder.Property(a => a.Date).IsRequired();
        builder.Property(a => a.Text).IsRequired();
        builder.Property(a => a.Title).IsRequired();
        builder.HasOne(a => a.AnnouncementType);
        builder.HasMany(a => a.Keywords).WithMany(k => k.Announcements);
    }
}
