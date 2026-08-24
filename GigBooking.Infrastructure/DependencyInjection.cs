using GigBooking.Application.Common.Interfaces;
using GigBooking.Domain.Abstractions;
using GigBooking.Infrastructure.Data;
using GigBooking.Infrastructure.Repositories;
using GigBooking.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace GigBooking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IGuidGenerator, GuidGenerator>();
        services.AddScoped<IArtistRepository, ArtistRepository>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlite(connectionString);
        });


        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

        return services;
    }

}
