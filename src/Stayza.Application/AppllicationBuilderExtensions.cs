using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stayza.Application.Books;
using Stayza.Application.Loans;

namespace Stayza.Application;

public static class AppllicationBuilderExtensions
{
    public static IHostApplicationBuilder AddApplication(this IHostApplicationBuilder builder)
    {
        builder.Services
            .AddScoped<LoansService>()
            .AddScoped<BooksService>();

        builder.Services.AddSingleton<LoansBackgroundJobs>();

        builder.Services.AddValidatorsFromAssembly(typeof(LoansService).Assembly);

        return builder;
    }
}