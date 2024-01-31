namespace Kvarovi.EntityConfigs;

using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class KeywordUserConfiguration: IEntityTypeConfiguration<KeywordUser>
{

    public void Configure(EntityTypeBuilder<KeywordUser> builder)
    {
        builder.HasKey(k => k.KeywordUserId);
        builder.HasIndex(k => k.KeywordId);
        builder.HasIndex(k => k.UserId);
    }
}
