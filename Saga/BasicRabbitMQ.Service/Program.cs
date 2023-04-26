// See https://aka.ms/new-console-template for more information

using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Saga.Components.Consumers;
using Saga.Components.StateMachines;

Console.WriteLine("Hello, World!");

var host = CreateHostBuilder(args).Build();


host.RunAsync();

//var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
//{
//    sbc.Host("rabbitmq://rabbitmq:rabbitmq@192.168.112.110:5672");
//    sbc.ReceiveEndpoint(KebabCaseEndpointNameFormatter.Instance.Consumer<SubmitOrderConsumer>(), ep =>
//    {
//       ep.Consumer<SubmitOrderConsumer>();

//    });
//});
//await bus.StartAsync();

Console.WriteLine("Bus Started. Waiting for Messages...");
Console.WriteLine("------------------------------------");
Console.ReadLine();


static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureLogging((_, logging) =>
        {
            logging.ClearProviders();
            logging.AddSimpleConsole(options => options.IncludeScopes = true);
            logging.AddConsole();
            logging.AddDebug();
            logging.SetMinimumLevel(LogLevel.Trace);
        }).ConfigureServices((hostContext, services) =>
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumersFromNamespaceContaining<SubmitOrderConsumer>();
               
                x.AddSagaStateMachine<OrderStateMachine, OrderState>().RedisRepository(rs =>
                {
                    rs.DatabaseConfiguration("192.168.112.110:6379,password=UldaaGNtUmhRREl4UUVSbGRrOXdjMEE1T1RnPQ==");
                });
                x.SetKebabCaseEndpointNameFormatter();
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                    cfg.Host("rabbitmq://rabbitmq:rabbitmq@192.168.112.110:5672");


                    cfg.ReceiveEndpoint("submit-order-error", e =>
                    {
                        e.Consumer<DashboardFaultConsumer>(context);
                      //  e.Consumer<SubmitOrderConsumer>(context);

                    });
                });
                x.AddConfigureEndpointsCallback((_, _, cfg) =>
                {

                    cfg.UseMessageRetry(r => r.Interval(1, 1000));

                });
            });
        });