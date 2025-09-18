using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SearchService.Domain.Entities;

namespace SearchService.Infrastructure.Persistence.Configurations;

public class SearchLogConfiguration : IEntityTypeConfiguration<SearchLogEntity>
{
    public void Configure(EntityTypeBuilder<SearchLogEntity> builder)
    {
        builder.ToTable("SearchLogs");

        builder.HasKey(x => x.LogId);

        builder.Property(x => x.SessionId)
            .IsRequired();

        builder.Property(x => x.SearchId)
            .IsRequired();

        builder.Property(x => x.Timestamp)
            .IsRequired();

        builder.Property(x => x.Query)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.Page).IsRequired();
        builder.Property(x => x.ResultCount).IsRequired();
        builder.Property(x => x.ElapsedMs).IsRequired();

        builder.Property(x => x.UserAgent).HasMaxLength(500);
        builder.Property(x => x.IpAddress).HasMaxLength(45); // IPv6 ready
        builder.Property(x => x.Referer).HasMaxLength(2000);

        // Indexes for analytics
        builder.HasIndex(x => x.Timestamp).HasDatabaseName("IX_SearchLogs_Timestamp");
        builder.HasIndex(x => x.Query).HasDatabaseName("IX_SearchLogs_Query");
        builder.HasIndex(x => x.IpAddress).HasDatabaseName("IX_SearchLogs_IpAddress");

        // Composite index for session + search tracking
        builder.HasIndex(x => new { x.SessionId, x.SearchId })
            .HasDatabaseName("IX_SearchLogs_Session_Search");
    }
}