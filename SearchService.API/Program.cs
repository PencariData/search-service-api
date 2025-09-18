using SearchService.API.Middlewares;
using SearchService.Application;
using SearchService.Infrastructure;
using SearchService.Infrastructure.Helpers.Seeding;
using SearchService.Shared;
using SearchService.Shared.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Dependency Injection
builder.Services.AddShared(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Cache
builder.Services.AddMemoryCache();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowNuxtApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Check for CLI seed argument
if (args.Contains("--seed"))
{
    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<IElasticSeeder>();
    await seeder.SeedAsync();

    Console.WriteLine("Seeding complete. Exiting...");
    return; // exit app after seeding
}

// Use CORS middleware (important: before UseRouting)
app.UseCors("AllowNuxtApp");

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestInfoMiddleware>();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();