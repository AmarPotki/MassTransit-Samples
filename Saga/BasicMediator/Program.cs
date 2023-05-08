using MassTransit;
using Quartz;
using Saga.Contracts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMassTransit(cfg =>
{
    cfg.SetKebabCaseEndpointNameFormatter();
    cfg.AddPublishMessageScheduler();
    cfg.AddQuartzConsumers();
    cfg.UsingRabbitMq((context, c) =>
    {
        c.UsePublishMessageScheduler();
        c.Host("rabbitmq://rabbitmq:rabbitmq@192.168.112.110:5672");
        c.ConfigureEndpoints(context);
    });

    cfg.AddRequestClient<SubmitOrder>();
    cfg.AddRequestClient<CheckOrder>();

});
builder.Services.AddQuartz(q =>
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


builder.Host.ConfigureLogging((_, logging) =>
{
    logging.AddConsole();

    logging.SetMinimumLevel(LogLevel.Trace);
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
