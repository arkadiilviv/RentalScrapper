using ImScoutAtWorker;

var builder = Host.CreateApplicationBuilder(args);
SystemInfo.InitValues(builder.Configuration);
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<RabbitMQClient>();
var host = builder.Build();
host.Run();