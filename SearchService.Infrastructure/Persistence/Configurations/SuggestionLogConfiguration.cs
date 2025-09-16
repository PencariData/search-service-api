using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SearchService.Domain.Entities;

namespace SearchService.Infrastructure.Persistence.Configurations;

public class SuggestionLogConfiguration : IEntityTypeConfiguration<SuggestionLogEntity>
{
    public void Configure(EntityTypeBuilder<SuggestionLogEntity> builder)
    {
        builder.ToTable("SuggestionSession");
        builder.HasKey(x => x.SuggestionId);
        
        // SessionInfo → flatten into SuggestionSession table
        builder.OwnsOne(x => x.Session, session =>
        {
            session.Property(s => s.Timestamp).HasColumnName("Timestamp").IsRequired();
            session.Property(s => s.Query).HasColumnName("QueryText").IsRequired();
            session.Property(s => s.AccommodationSuggestionCount).HasColumnName("AccommodationSuggestionCount");
            session.Property(s => s.DestinationSuggestionCount).HasColumnName("DestinationSuggestionCount");
        });

        // PerformanceInfo → separate table
        builder.OwnsOne(x => x.Performance, perf =>
        {
            perf.ToTable("SuggestionPerformance");
            perf.WithOwner().HasForeignKey("SuggestionId");
            perf.Property(p => p.IsFromCache).HasColumnName("IsFromCache").IsRequired();
            perf.Property(p => p.ElapsedMs).HasColumnName("ElapsedMs").IsRequired();
        });

        // InteractionInfo → separate table
        builder.OwnsOne(x => x.Interaction, interaction =>
        {
            interaction.ToTable("SuggestionInteraction");
            interaction.WithOwner().HasForeignKey("SuggestionId");
            interaction.Property(i => i.ClickedResultId).HasColumnName("ClickedResultId");
            interaction.Property(i => i.ClickRank).HasColumnName("ClickRank");
        });
    }
}