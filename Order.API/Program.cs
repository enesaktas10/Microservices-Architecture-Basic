using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Consumers;
using Order.API.Models;
using Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<OrderAPIDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("connectionString")));

builder.Services.AddMassTransit(configurator =>
{
    configurator.AddConsumer<PaymentCompletedEventConsumer>();
    configurator.AddConsumer<StockNotReservedEventConsumer>();
    configurator.AddConsumer<PaymentFailedEventConsumer>();

    configurator.UsingRabbitMq((context, _configurator) =>
    {
        _configurator.Host(builder.Configuration["RabbitMQ"]); //kendimize has bir node icerisinde tuttugumuz icin bu sekilde verdik connectionstring icerisinde tutsaydik yukaridaki sql baglantisi ornegi gibi tutabilriidk
        _configurator.ReceiveEndpoint($"{RabbitMQSettings.Order_PaymentCompletedEventQueue}",e=>e.ConfigureConsumer<PaymentCompletedEventConsumer>(context));

        _configurator.ReceiveEndpoint($"{RabbitMQSettings.Order_StockNotReservedEventQueue}",e=>e.ConfigureConsumer<StockNotReservedEventConsumer>(context));

        _configurator.ReceiveEndpoint($"{RabbitMQSettings.Order_PaymentFailedEventConsumer}", e => e.ConfigureConsumer<PaymentFailedEventConsumer>(context));
    });
});

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
