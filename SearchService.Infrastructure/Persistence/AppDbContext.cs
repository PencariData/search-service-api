using Microsoft.EntityFrameworkCore;
using SearchService.Domain.Entities;
using SearchService.Infrastructure.Entities;

namespace SearchService.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<SearchEventEntity> SearchEvents { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SearchEventEntity>(builder =>
        {
            builder.ToTable("search_events");
            builder.HasKey(x => x.EventId);

            builder.Property(x => x.EventType).HasColumnName("event_type");
            builder.Property(x => x.Payload).HasColumnName("payload").HasColumnType("jsonb");
            builder.Property(x => x.SessionId).HasColumnName("session_id");
            builder.Property(x => x.SearchId).HasColumnName("search_id");
            builder.Property(x => x.OccurredAt).HasColumnName("occurred_at");
        });
    }
}