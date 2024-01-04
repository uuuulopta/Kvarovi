namespace Kvarovi.EntityConfigs;

using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AnnouncementTypeConfiguration : IEntityTypeConfiguration<AnnouncementType>
{

    public void Configure(EntityTypeBuilder<AnnouncementType> builder)
    {
        builder.HasKey(a => a.AnnouncementTypeId);
        
        builder.Property(a => a.AnnouncementTypeName).IsRequired();
    }
}
