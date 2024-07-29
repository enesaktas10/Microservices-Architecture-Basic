using MassTransit;
using MongoDB.Driver;
using Shared;
using Stock.API.Consumers;
using Stock.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(configurator =>
{
    //burada OrderCreatedEventConsumer sinifinin bir consumer oldugunu belirttik
    configurator.AddConsumer<OrderCreatedEventConsumer>();

    configurator.UsingRabbitMq((context, _configurator) =>
    {
        _configurator.Host(builder.Configuration["RabbitMQ"]);

        //rabbitmq icerisindeki hangi kuyruktan bu consumerin dinleme islemi yapilacagini da bildirmemiz gerekiyor
        _configurator.ReceiveEndpoint(RabbitMQSettings.Stock_OrderCreatedEventQueue,e=>e.ConfigureConsumer<OrderCreatedEventConsumer>(context));
    });
});

builder.Services.AddSingleton<MongoDBService>();

using IServiceScope scope =  builder.Services.BuildServiceProvider().CreateScope();
MongoDBService mongoDbService =  scope.ServiceProvider.GetService<MongoDBService>();
var collection = mongoDbService.GetCollection<Stock.API.Models.Entities.Stock>();
if (!collection.FindSync(s=>true).Any())
{
    await collection.InsertOneAsync(new() {ProductId = "1", Count = 2000});
    await collection.InsertOneAsync(new() { ProductId = "2", Count = 1000 });
    await collection.InsertOneAsync(new() { ProductId = "3", Count = 3000 });
    await collection.InsertOneAsync(new() { ProductId = "4", Count = 5000 });
    await collection.InsertOneAsync(new() { ProductId = "5", Count = 500 });
}

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
