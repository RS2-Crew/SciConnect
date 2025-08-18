using IdentityService.Consumers;
using IdentityService.Consumers.EntityCreated;
using IdentityService.Extentions;
using IdentityService.Services;
using MassTransit;
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

builder.Services.AddMemoryCache();

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

// Configure the HTTP request pipeline.
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
