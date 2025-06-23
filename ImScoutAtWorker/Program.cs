using ImScoutAtWorker;
using ImScoutAtWorker.Interfaces;

var builder = Host.CreateApplicationBuilder(args);
SystemInfo.InitValues(builder.Configuration);
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<RabbitMQClient>();
builder.Services.AddTransient<IMessageProcessor, MessageProcessor>();
var host = builder.Build();
host.Run();