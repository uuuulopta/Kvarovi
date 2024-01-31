namespace Kvarovi.EntityConfigs;

using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AnnouncementConfiguration : IEntityTypeConfiguration<Announcement>
{

    public void Configure(EntityTypeBuilder<Announcement> builder)
    {
        builder.HasKey(a => a.AnnouncementId);
        builder.Property(a => a.Text).IsRequired();
        builder.Property(a => a.Title).IsRequired();
        // builder.Property(a => a.Timestamp).IsRowVersion();                  
        builder.HasOne(a => a.AnnouncementType);
    }
}
