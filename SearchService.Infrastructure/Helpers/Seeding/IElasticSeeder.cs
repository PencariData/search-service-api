namespace SearchService.Infrastructure.Helpers.Seeding;

public interface IElasticSeeder
{
    Task SeedAsync();
}