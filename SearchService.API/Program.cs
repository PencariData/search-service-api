using SearchService.API.Middlewares;
using SearchService.Application;
using SearchService.Infrastructure;
using SearchService.Infrastructure.ServiceCollectionExtensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(); 

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructure();
builder.Services.AddElasticSearch(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();