using MassTransit;
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

    cfg.UsingRabbitMq((context, c) =>
    {
        c.Host("rabbitmq://rabbitmq:rabbitmq@192.168.112.110:5672");
        c.ConfigureEndpoints(context);
    });

    cfg.AddRequestClient<SubmitOrder>();
    cfg.AddRequestClient<CheckOrder>();
  



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
