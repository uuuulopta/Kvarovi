namespace Kvarovi.EntityConfigs;

using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public enum AnnouncementTypeEnum
{
   vodovod = 1,
   eps = 2,
}
public class AnnouncementTypeConfiguration : IEntityTypeConfiguration<AnnouncementType>
{

    public void Configure(EntityTypeBuilder<AnnouncementType> builder)
    {
        builder.HasKey(a => a.AnnouncementTypeId);
        builder.Property(a => a.AnnouncementTypeName).IsRequired();
        var vodovod = new AnnouncementType() { AnnouncementTypeName = "vodovod",AnnouncementTypeId = 1};
        var eps = new AnnouncementType() { AnnouncementTypeName = "eps",AnnouncementTypeId = 2};
        builder.HasData(vodovod, eps);
    }
}
