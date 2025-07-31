using Stayza.Application;
using Stayza.Domain.Users;
using Stayza.Infrastructure;
using Stayza.Infrastructure.Persistence;
using Stayza.Web.Infrastructure.Endpoints;
using Stayza.Web.Infrastructure.Seed;
using Stayza.Web.Infrastructure.Session;
using Stayza.Web.Infrastructure.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddApplication()
    .AddInfrastructure()
    .AddConfiguredSwagger();

builder.Services.AddScoped<SessionAccessorService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.ApplyDbMigrations();
    app.SeedTestUser();
    app.MapOpenApi();
}
app.UseEndpoints<Program>();
app.MapIdentityApi<User>();
app.MapConfiguredSwagger();

app.Run();