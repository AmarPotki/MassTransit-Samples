using MassTransit;
using Quartz;
using QuartzService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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

        s.UseSqlServer("Server=192.168.10.140;Database=QuartzService;User Id=sa;Password=Ai@123456;MultipleActiveResultSets=true;Persist Security Info=False;Encrypt=False;TrustServerCertificate=True;");

        s.UseJsonSerializer();

        s.UseClustering(c =>
        {
            c.CheckinMisfireThreshold = TimeSpan.FromSeconds(20);
            c.CheckinInterval = TimeSpan.FromSeconds(10);
        });
    });
});

builder.Services.AddMassTransit(x =>
{
    x.AddPublishMessageScheduler();

    x.AddQuartzConsumers();

    x.AddConsumer<SampleConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.UsePublishMessageScheduler();
        cfg.Host("rabbitmq://rabbitmq:rabbitmq@192.168.112.110:5672");
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.Configure<MassTransitHostOptions>(options =>
{
    options.WaitUntilStarted = true;
});

builder.Services.AddQuartzHostedService(options =>
{
    options.StartDelay = TimeSpan.FromSeconds(5);
    options.WaitForJobsToComplete = true;
});

builder.Services.AddHostedService<SuperWorker>();

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
