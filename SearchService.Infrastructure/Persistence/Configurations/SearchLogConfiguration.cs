using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SearchService.Domain.Entities;

namespace SearchService.Infrastructure.Persistence.Configurations;

public class SearchLogConfiguration : IEntityTypeConfiguration<SearchLogEntity>
{
    public void Configure(EntityTypeBuilder<SearchLogEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.Timestamp);
        builder.HasIndex(x => x.SearchQuery);
        builder.HasIndex(x => x.SearchType);

        builder.Property(x => x.SearchQuery)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.SearchType)
            .HasMaxLength(50)
            .IsRequired();
    }
}