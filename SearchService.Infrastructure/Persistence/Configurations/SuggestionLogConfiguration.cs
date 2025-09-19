using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SearchService.Domain.Entities;

namespace SearchService.Infrastructure.Persistence.Configurations;

public class SuggestionLogConfiguration : IEntityTypeConfiguration<SuggestionLogEntity>
{
    public void Configure(EntityTypeBuilder<SuggestionLogEntity> builder)
    {
        builder.ToTable("SuggestionLogs");
        
        builder.HasKey(x => x.LogId);
        
        builder.Property(x => x.SessionId)
            .IsRequired();
        
        builder.Property(x => x.Timestamp)
            .IsRequired();
        
        builder.Property(x => x.Query)
            .IsRequired()
            .HasMaxLength(1000);
        
        builder.Property(x => x.AccommodationSuggestionCount).IsRequired();
        builder.Property(x => x.DestinationSuggestionCount).IsRequired();
        
        // Indexes for analytics
        builder.HasIndex(x => x.Timestamp).HasDatabaseName("IX_SuggestionLogs_Timestamp");
        builder.HasIndex(x => x.Query).HasDatabaseName("IX_SuggestionLogs_Query");
    }
}