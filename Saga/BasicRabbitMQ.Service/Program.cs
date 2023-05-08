// See https://aka.ms/new-console-template for more information

using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
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
               
                x.AddSagaStateMachine<OrderStateMachine, OrderState>().InMemoryRepository();
                //    .RedisRepository(rs =>
                //{
                //    rs.DatabaseConfiguration("192.168.112.110:6379,password=UldaaGNtUmhRREl4UUVSbGRrOXdjMEE1T1RnPQ==");
                //});
                x.SetKebabCaseEndpointNameFormatter();
                x.AddPublishMessageScheduler();
                x.AddQuartzConsumers();
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                    cfg.Host("rabbitmq://rabbitmq:rabbitmq@192.168.112.110:5672");
                    cfg.UsePublishMessageScheduler();

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
            services.AddQuartz(q =>
            {
                q.SchedulerName = "MassTransit-Scheduler";
                q.SchedulerId = "AUTO";

                q.UseMicrosoftDependencyInjectionJobFactory();

                q.UseDefaultThreadPool(tp =>
                {
                    tp.MaxConcurrency = 10;
                });

                q.UseTimeZoneConverter();

                q.UsePersistentStore(s =>
                {
                    s.UseProperties = true;
                    s.RetryInterval = TimeSpan.FromSeconds(15);

                    s.UseSqlServer("Server=192.168.10.140;Database=QuartzService;User Id=sa;Password=***;MultipleActiveResultSets=true;Persist Security Info=False;Encrypt=False;TrustServerCertificate=True;");

                    s.UseJsonSerializer();

                    s.UseClustering(c =>
                    {
                        c.CheckinMisfireThreshold = TimeSpan.FromSeconds(20);
                        c.CheckinInterval = TimeSpan.FromSeconds(10);
                    });
                });
            });
        });