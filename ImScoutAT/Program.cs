using ImScoutAT;

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.AddSimpleConsole(options =>
{
    options.SingleLine = true;
    options.IncludeScopes = true;
});
builder.Services.AddHostedService<Worker>();
builder.Services.AddMemoryCache(options =>
{
    options.TrackStatistics = true;
    options.SizeLimit = 51200;
});
SystemInfo.InitValues(builder.Configuration);
builder.Services.AddTransient<Scrapper>();
builder.Services.AddTransient<MessageClient>();
builder.Services.AddTransient<DisplayHelper>();

var host = builder.Build();
host.Run();