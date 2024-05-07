using DeliveryDrivers.Data;
using DeliveryDrivers.Infrastructure;
using DeliveryDrivers.Models;
using DeliveryDrivers.RabbitService;
using MotorcycleRental.Data;
using MotorcycleRental.RabbitService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<DeliveryDriverDatabaseSettings>(builder.Configuration.GetSection("Mottu"));
builder.Services.AddSingleton<DriverRepository>();
builder.Services.AddSingleton<IDriverRepository, DriverRepository>();
builder.Services.AddSingleton<MotorcycleRepository>();
builder.Services.AddSingleton<IMotorcycleService, MotorcycleRepository>();
builder.Services.AddScoped<MotorcycleConsumer>();
builder.Services.AddScoped<DriverConsumer>();
builder.Services.AddScoped<DriverimageS3>();
builder.Services.AddScoped<IRabbitBusService, RabbitMQService>();
builder.Services.AddScoped<RabbitMQQueue>();

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
