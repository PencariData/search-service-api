using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SearchService.Domain.Entities;

namespace SearchService.Infrastructure.Persistence.Configurations;

public class ClickLogConfiguration : IEntityTypeConfiguration<ClickLogEntity>
{
    public void Configure(EntityTypeBuilder<ClickLogEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.DocumentId).IsRequired();
    }
}