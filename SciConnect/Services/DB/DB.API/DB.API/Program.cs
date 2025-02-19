using DB.API.Extensions;
using DB.Application;
using DB.Infrastructure;
using DB.Infrastructure.Persistance;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var configuration = builder.Configuration;

// Registrujte Infrastructure i Application servise
builder.Services.AddInfrastructureServices(configuration);
builder.Services.AddApplicationServices();

builder.MigrateDatabase<SqlServerContext>((context, services) =>
{
    var logger = services.GetRequiredService<ILogger<SqlServerContextSeed>>();
    SqlServerContextSeed.SeedAsync(context, logger).Wait();
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
