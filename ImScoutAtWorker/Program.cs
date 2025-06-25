using ImScoutAtWorker;
using ImScoutAtWorker.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);
SystemInfo.InitValues(builder.Configuration);
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<RabbitMQClient>();
builder.Services.AddTransient<IMessageProcessor, MessageProcessor>();
builder.Services.AddSingleton<RentalContext>();
var host = builder.Build();
Console.WriteLine($"DB: {SystemInfo.SQL_CONNECTION_STRING}");
host.Run();