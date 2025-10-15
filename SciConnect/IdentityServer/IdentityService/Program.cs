using IdentityService.Consumers;
using IdentityService.Consumers.EntityCreated;
using IdentityService.Data;
using IdentityService.Extentions;
using IdentityService.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

//builder.Services.AddMemoryCache();

builder.Services.AddStackExchangeRedisCache(
opts => {
    opts.Configuration = builder.Configuration.GetValue<string>("CacheSettings:ConnectionString");
        }
);


//builder.Services.AddAuthentication();

builder.Services.ConfigurePersistence(builder.Configuration);
builder.Services.ConfigureIdentity();
builder.Services.ConfigureJWT(builder.Configuration);
builder.Services.ConfigureMiscellaneousServices();

builder.Services.AddMassTransit(config => {
    config.AddConsumer<EmailConsumer>();
    config.AddConsumer<InstitutionCreatedConsumer>();
    config.AddConsumer<EmployeeCreatedConsumer>();
    config.AddConsumer<SimpleEntityCreatedConsumer>();
    //config.AddConsumers(typeof(Program).Assembly);
    config.UsingRabbitMq((ctx, cfg) => {
        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
        cfg.ReceiveEndpoint("notify_validation_queue", e =>
        {
            e.ConfigureConsumer<EmailConsumer>(ctx);
            //e.ConfigureConsumers(ctx); // <- configures all discovered consumers
        });

        cfg.ReceiveEndpoint("notify_broadcast_queue", e =>
        {
            e.ConfigureConsumer<InstitutionCreatedConsumer>(ctx);
            e.ConfigureConsumer<EmployeeCreatedConsumer>(ctx);
            e.ConfigureConsumer<SimpleEntityCreatedConsumer>(ctx);
        });
    });


});

builder.Services.AddScoped<IEmailSender, EmailSender>();

Console.WriteLine("MassTransit + Consumer configuration complete.");

builder.Logging.ClearProviders();
builder.Logging.AddConsole();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
    var retries = 0;
    while (retries++ < 10)
    {
        try
        {
            context.Database.Migrate();
            break;
        }
        catch
        {
            if (retries >= 10) throw;
            Thread.Sleep(3000);
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable CORS
app.UseCors("AllowAngularApp");

//app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
