using Microsoft.Extensions.DependencyInjection;
using FluentValidation;

using GigBooking.Domain.Services;
using GigBooking.Application.Common.Behaviors;
namespace GigBooking.Application;

public static class DependencyInjection
{

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(AssemblyReference).Assembly);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));

        } );

        services.AddScoped<IGigBookingService, GigBookingService>();

        return services;
    }

}
