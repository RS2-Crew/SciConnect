using Autocomplete.API.Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using EventBus.Messages.Events;
using Autocomplete.API.Consumer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AutoDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("AutoDb")));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<EntityChangedConsumer>();
    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(new Uri(builder.Configuration["Rabbit:Host"]!), h =>
        {
            h.Username(builder.Configuration["Rabbit:User"]);
            h.Password(builder.Configuration["Rabbit:Pass"]);
        });

        cfg.ReceiveEndpoint("autocomplete-events", e =>
        {
            e.ConfigureConsumer<EntityChangedConsumer>(ctx);
        });
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AutoDbContext>();
    db.Database.EnsureCreated();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();
