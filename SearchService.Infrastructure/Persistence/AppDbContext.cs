using Microsoft.EntityFrameworkCore;
using SearchService.Domain.Entities;

namespace SearchService.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<SearchLogEntity> SearchLogs { get; set; }
    public DbSet<ClickLogEntity>  ClickLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}