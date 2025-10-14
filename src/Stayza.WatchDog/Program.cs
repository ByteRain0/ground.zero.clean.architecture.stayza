var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services
    .AddEndpointsApiExplorer()
    .AddHealthChecks();

builder.Services
    .AddHealthChecksUI()
    .AddInMemoryStorage();

var app = builder.Build();

app.MapHealthChecksUI();
app.Run();