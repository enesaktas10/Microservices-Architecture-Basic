using MassTransit;
using MongoDB.Driver;
using Shared;
using Shared.Events;
using Shared.Messages;
using Stock.API.Services;

namespace Stock.API.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {
        
        private IMongoCollection<Models.Entities.Stock> _stockCollection;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly IPublishEndpoint _publishEndpoint;

        public OrderCreatedEventConsumer(MongoDBService mongoDbService,ISendEndpointProvider sendEndpointProvider, IPublishEndpoint publishEndpoint)
        {
            _stockCollection = mongoDbService.GetCollection<Models.Entities.Stock>();
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            List<bool> stockResult = new();

            foreach (OrderItemMessage orderItem in context.Message.OrderItems)
            {
                stockResult.Add((await _stockCollection.FindAsync(s=>s.ProductId == orderItem.ProductId && s.Count >= orderItem.Count)).Any());
            }

            if (stockResult.TrueForAll(sr=> sr.Equals(true)))
            {
                foreach (OrderItemMessage orderItem in context.Message.OrderItems)
                {
                 Models.Entities.Stock stock =  await (await  _stockCollection.FindAsync(s => s.ProductId == orderItem.ProductId)).FirstOrDefaultAsync();

                 stock.Count -= orderItem.Count;
                 await _stockCollection.FindOneAndReplaceAsync(s => s.ProductId == orderItem.ProductId, stock);

                }

                StockReservedEvent stockReservedEvent = new()
                {
                    BuyerId = context.Message.BuyerId,
                    TotalPrice = context.Message.TotalPrice,
                    OrderId = context.Message.OrderId,
                };

                ISendEndpoint sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(
                    new Uri($"queue:{RabbitMQSettings.Payment_StockReservedEventQueue}"));

                await sendEndpoint.Send(stockReservedEvent);

                await Console.Out.WriteLineAsync("Stok islemleri basarili ...");

            }
            else
            {
                StockNotReservedEvent stockNotReservedEvent = new()
                {
                    Message = "...",
                    OrderId = context.Message.OrderId,
                    BuyerId = context.Message.BuyerId
                };

                await _publishEndpoint.Publish(stockNotReservedEvent);
                await Console.Out.WriteLineAsync("Stok islemleri basarisiz...");
            }

            //return Task.CompletedTask;
        }
    }
}
