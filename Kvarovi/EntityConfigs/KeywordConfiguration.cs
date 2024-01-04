namespace Kvarovi.EntityConfigs;

using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class KeywordConfiguration: IEntityTypeConfiguration<Keyword>
{

    public void Configure(EntityTypeBuilder<Keyword> builder)
    {
        builder.HasKey(k => k.KeywordId);
        builder.HasIndex(k => k.Word).IsUnique();
        // hasMany configurations have been done in announcment & user.
    }
}
