using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SearchService.Infrastructure.Models;
using SearchService.Shared.Models;

namespace SearchService.Infrastructure.Helpers.Seeding;

public class ElasticSeeder(
    ElasticConfiguration elasticConfiguration,
    HttpClient httpClient,
    ILogger<ElasticSeeder> logger)
    : IElasticSeeder
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };
    
    public async Task SeedAsync()
    {
        try
        {
            logger.LogInformation("Starting Elasticsearch seeding...");

            // Create indices
            await CreateAccommodationIndexAsync();
            await CreateDestinationIndexAsync();

            // Seed data
            await SeedAccommodationsAsync();
            await SeedDestinationsAsync();

            logger.LogInformation("Seeding completed successfully!");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during seeding");
            throw;
        }
    }
    
     private async Task CreateAccommodationIndexAsync()
    {
        logger.LogInformation("Creating accommodations index...");

        var mappingJson = """
        {
          "settings": {
            "index.max_ngram_diff": 20,
            "analysis": {
              "analyzer": {
                "destination_analyzer": {
                  "tokenizer": "standard",
                  "filter": ["lowercase", "trim"]
                },
                "ngram_analyzer": {
                  "tokenizer": "ngram_tokenizer",
                  "filter": ["lowercase", "trim"]
                }
              },
              "tokenizer": {
                "ngram_tokenizer": {
                  "type": "ngram",
                  "min_gram": 3,
                  "max_gram": 20,
                  "token_chars": ["letter", "digit"]
                }
              }
            }
          },
          "mappings": {
            "properties": {
              "id": { "type": "keyword" },
              "name": {
                "type": "text",
                "fields": {
                  "keyword": { "type": "keyword" },
                  "ngram": { "type": "text", "analyzer": "ngram_analyzer", "search_analyzer": "standard" }
                }
              },
              "name_suggest": {
                "type": "completion",
                "analyzer": "simple",
                "preserve_separators": true,
                "preserve_position_increments": true,
                "max_input_length": 50
              },
              "destinationId": { "type": "keyword" },
              "destinationName": {
                "type": "text",
                "fields": {
                  "ngram": { "type": "text", "analyzer": "ngram_analyzer", "search_analyzer": "standard" }
                }
              },
              "fullDestination": { "type": "text", "analyzer": "destination_analyzer" },
              "accommodationType": { "type": "keyword" },
              "coordinate": { "type": "geo_point" }
            }
          }
        }
        """;

        await CreateIndexAsync(elasticConfiguration.AccommodationIndex, mappingJson);
    }

    private async Task CreateDestinationIndexAsync()
    {
        logger.LogInformation("Creating destinations index...");

        var mappingJson = """
        {
          "mappings": {
            "properties": {
              "id": { "type": "keyword" },
              "name": { "type": "text" },
              "name_suggest": {
                "type": "completion",
                "analyzer": "simple",
                "preserve_separators": true,
                "preserve_position_increments": true,
                "max_input_length": 100
              },
              "fullName": { "type": "text" },
              "centerCoordinate": { "type": "geo_point" },
              "accommodationCount": { "type": "integer" }
            }
          }
        }
        """;

        await CreateIndexAsync(elasticConfiguration.DestinationIndex, mappingJson);
    }

    private async Task CreateIndexAsync(string indexName, string mappingJson)
    {
        var url = $"{elasticConfiguration.ElasticUrl}/{indexName}";

        // Delete index if exists
        var deleteResponse = await httpClient.DeleteAsync(url);
        if (deleteResponse.IsSuccessStatusCode)
        {
            logger.LogInformation("Deleted existing index: {IndexName}", indexName);
        }

        // Create new index
        var content = new StringContent(mappingJson, Encoding.UTF8, "application/json");
        var response = await httpClient.PutAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
            logger.LogInformation("Created index: {IndexName}", indexName);
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            logger.LogError("Failed to create index {IndexName}: {Error}", indexName, errorContent);
            throw new InvalidOperationException($"Failed to create index {indexName}: {errorContent}");
        }
    }

    private async Task SeedAccommodationsAsync()
    {
        logger.LogInformation("Seeding accommodations...");

        var accommodations = new[]
        {
            new Accommodation
            {
                Id = "a1f3c9d0-1e2b-4a5c-b678-0a1b2c3d4e5f",
                Name = "Royal Jakarta Hotel",
                NameSuggest = new NameSuggest
                {
                    Input = ["Royal", "Jakarta", "Hotel", "Royal Jakarta", "Royal Hotel", "Jakarta Hotel", "Royal Jakarta Hotel"
                    ]
                },
                DestinationId = "d1",
                DestinationName = "Jakarta",
                FullDestination = "Jakarta, DKI Jakarta, Indonesia",
                AccommodationType = "Hotel",
                Coordinate = new GeoPoint { Lat = -6.2088, Lon = 106.8456 }
            },
            new Accommodation
            {
                Id = "b2e4d0f1-2f3c-4b5d-c789-1b2c3d4e5f6a",
                Name = "Bandung Mountain Resort",
                NameSuggest = new NameSuggest
                {
                    Input = ["Bandung", "Mountain", "Resort", "Bandung Mountain", "Bandung Resort", "Mountain Resort", "Bandung Mountain Resort"
                    ]
                },
                DestinationId = "d2",
                DestinationName = "Bandung",
                FullDestination = "Bandung, West Java, Indonesia",
                AccommodationType = "Resort",
                Coordinate = new GeoPoint { Lat = -6.9175, Lon = 107.6191 }
            },
            new Accommodation
            {
                Id = "d4a6b2c3-4b5c-4d7e-e901-3d4e5f6a7b8c",
                Name = "Yogyakarta Boutique Villa",
                NameSuggest = new NameSuggest
                {
                    Input = ["Yogyakarta", "Boutique", "Villa", "Yogyakarta Boutique", "Yogyakarta Villa", "Boutique Villa", "Yogyakarta Boutique Villa"
                    ]
                },
                DestinationId = "d4",
                DestinationName = "Yogyakarta",
                FullDestination = "Yogyakarta, Special Region of Yogyakarta, Indonesia",
                AccommodationType = "Villa",
                Coordinate = new GeoPoint { Lat = -7.7956, Lon = 110.3695 }
            },
            new Accommodation
            {
                Id = "d0a2b8c9-0b1c-4d3e-4567-9d0e1f2a3b4c",
                Name = "Yogyakarta Heritage Villa",
                NameSuggest = new NameSuggest
                {
                    Input = ["Yogyakarta", "Heritage", "Villa", "Yogyakarta Heritage", "Yogyakarta Villa", "Heritage Villa", "Yogyakarta Heritage Villa"
                    ]
                },
                DestinationId = "d4",
                DestinationName = "Yogyakarta",
                FullDestination = "Yogyakarta, Special Region of Yogyakarta, Indonesia",
                AccommodationType = "Villa",
                Coordinate = new GeoPoint { Lat = -7.7972, Lon = 110.3700 }
            }
        };

        await BulkIndexAsync(elasticConfiguration.AccommodationIndex, accommodations);
        logger.LogInformation("Accommodations seeded successfully");
    }

    private async Task SeedDestinationsAsync()
    {
        logger.LogInformation("Seeding destinations...");

        var destinations = new[]
        {
            new Destination
            {
                Id = "d1",
                Name = "Jakarta",
                NameSuggest = new NameSuggest { Input = ["Jakarta"] },
                FullName = "Jakarta, DKI Jakarta, Indonesia",
                CenterCoordinate = new GeoPoint { Lat = -6.2088, Lon = 106.8456 },
                AccommodationCount = 1
            },
            new Destination
            {
                Id = "d2",
                Name = "Bandung",
                NameSuggest = new NameSuggest { Input = ["Bandung"] },
                FullName = "Bandung, West Java, Indonesia",
                CenterCoordinate = new GeoPoint { Lat = -6.9175, Lon = 107.6191 },
                AccommodationCount = 2
            },
            new Destination
            {
                Id = "d3",
                Name = "Surabaya",
                NameSuggest = new NameSuggest { Input = ["Surabaya"] },
                FullName = "Surabaya, East Java, Indonesia",
                CenterCoordinate = new GeoPoint { Lat = -7.2575, Lon = 112.7521 },
                AccommodationCount = 2
            },
            new Destination
            {
                Id = "d4",
                Name = "Yogyakarta",
                NameSuggest = new NameSuggest { Input = ["Yogyakarta"] },
                FullName = "Yogyakarta, Special Region of Yogyakarta, Indonesia",
                CenterCoordinate = new GeoPoint { Lat = -7.7956, Lon = 110.3695 },
                AccommodationCount = 2
            },
            new Destination
            {
                Id = "d5",
                Name = "Malang",
                NameSuggest = new NameSuggest { Input = ["Malang"] },
                FullName = "Malang, East Java, Indonesia",
                CenterCoordinate = new GeoPoint { Lat = -7.9825, Lon = 112.6303 },
                AccommodationCount = 1
            },
            new Destination
            {
                Id = "d6",
                Name = "Semarang",
                NameSuggest = new NameSuggest { Input = ["Semarang"] },
                FullName = "Semarang, Central Java, Indonesia",
                CenterCoordinate = new GeoPoint { Lat = -6.9667, Lon = 110.4167 },
                AccommodationCount = 1
            },
            new Destination
            {
                Id = "d7",
                Name = "Bogor",
                NameSuggest = new NameSuggest { Input = ["Bogor"] },
                FullName = "Bogor, West Java, Indonesia",
                CenterCoordinate = new GeoPoint { Lat = -6.5950, Lon = 106.8167 },
                AccommodationCount = 1
            },
            new Destination
            {
                Id = "d8",
                Name = "Depok",
                NameSuggest = new NameSuggest { Input = ["Depok"] },
                FullName = "Depok, West Java, Indonesia",
                CenterCoordinate = new GeoPoint { Lat = -6.4025, Lon = 106.7949 },
                AccommodationCount = 1
            },
            new Destination
            {
                Id = "d9",
                Name = "Tangerang",
                NameSuggest = new NameSuggest { Input = ["Tangerang"] },
                FullName = "Tangerang, Banten, Indonesia",
                CenterCoordinate = new GeoPoint { Lat = -6.1783, Lon = 106.6319 },
                AccommodationCount = 1
            },
            new Destination
            {
                Id = "d10",
                Name = "Solo",
                NameSuggest = new NameSuggest { Input = ["Solo"] },
                FullName = "Solo, Central Java, Indonesia",
                CenterCoordinate = new GeoPoint { Lat = -7.5754, Lon = 110.8240 },
                AccommodationCount = 1
            }
        };

        await BulkIndexAsync(elasticConfiguration.DestinationIndex, destinations);
        logger.LogInformation("Destinations seeded successfully");
    }

    private async Task BulkIndexAsync<T>(string indexName, T[] documents)
    {
        var bulkBody = new StringBuilder();

        foreach (var doc in documents)
        {
            // Index action
            var indexAction = new { index = new { _index = indexName } };
            bulkBody.AppendLine(JsonSerializer.Serialize(indexAction, _jsonOptions));
            
            // Document
            bulkBody.AppendLine(JsonSerializer.Serialize(doc, _jsonOptions));
        }

        var content = new StringContent(bulkBody.ToString(), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync($"{elasticConfiguration.ElasticUrl}/_bulk", content);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            logger.LogError("Bulk indexing failed for {IndexName}: {Error}", indexName, errorContent);
            throw new InvalidOperationException($"Bulk indexing failed for {indexName}: {errorContent}");
        }

        logger.LogInformation("Bulk indexed {Count} documents to {IndexName}", documents.Length, indexName);
    }
}