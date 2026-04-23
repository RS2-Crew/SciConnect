using AnalyticsService.Consumers;
using AnalyticsService.Extensions;
using AnalyticsService.Services;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "http://localhost:53216")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.ConfigurePersistence(builder.Configuration);
builder.Services.ConfigureJWT(builder.Configuration);
builder.Services.ConfigureMiscellaneousServices();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ReadAccess", policy =>
        policy.RequireRole("Guest", "Administrator", "PM"));
});

builder.Services.AddMassTransit(config => {
    config.AddConsumer<InstitutionCreatedConsumer>();
    config.AddConsumer<EmployeeCreatedConsumer>();
    config.AddConsumer<SimpleEntityCreatedConsumer>();

    config.UsingRabbitMq((ctx, cfg) => {
        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);

        cfg.ReceiveEndpoint("analytics_entity_queue", e => {
            e.ConfigureConsumer<InstitutionCreatedConsumer>(ctx);
            e.ConfigureConsumer<EmployeeCreatedConsumer>(ctx);
            e.ConfigureConsumer<SimpleEntityCreatedConsumer>(ctx);
        });
    });
});

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable CORS
app.UseCors("AllowAngularApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
