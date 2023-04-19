// See https://aka.ms/new-console-template for more information

using BasicRabbitMQ.Components.Consumers;
using MassTransit;

Console.WriteLine("Hello, World!");


var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
{
    sbc.Host("rabbitmq://rabbitmq:rabbitmq@192.168.112.110:5672");
    sbc.ReceiveEndpoint("Consumer1", ep =>
    {
        ep.Consumer<SubmitOrderConsumer>();
    });
});
await bus.StartAsync();

Console.WriteLine("Bus Started. Waiting for Messages...");
Console.WriteLine("------------------------------------");
Console.ReadLine();