using GigBooking.Application.Common.Interfaces;
using GigBooking.Domain.Abstractions;
using GigBooking.Domain.Entities;
using GigBooking.Domain.ValueObjects;
using GigBooking.Infrastructure.Persistence;
using GigBooking.Infrastructure.Persistence.Repositories;
using GigBooking.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Testcontainers.MsSql;

namespace GigBooking.Infrastructure.IntegrationTests;

public class GigRepositoryTests : IAsyncLifetime
{

    private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

    private ServiceProvider _serviceProvider = null!;

    public async Task DisposeAsync()
    {
        await _msSqlContainer.DisposeAsync();
    }

    public async Task InitializeAsync()
    {
         await _msSqlContainer.StartAsync();
        var services = new ServiceCollection();
        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(_msSqlContainer.GetConnectionString()));
        services.AddScoped<IGuidGenerator, GuidGenerator>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddScoped<IGigRepository, GigRepository>();
        services.AddScoped<IArtistRepository, ArtistRepository>();
        services.AddScoped<IVenueRepository, VenueRepository>();
        services.AddSingleton<TimeProvider>(TimeProvider.System);

        _serviceProvider = services.BuildServiceProvider();

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.EnsureCreatedAsync();
    }


    [Fact]
    public async Task Can_Add_And_Retrieve_Gig_From_Database()
    {
        ArtistId artistId;
        VenueId venueId;
        Guid newGigGuid;

        using (var scope = _serviceProvider.CreateScope())
        {
            var guidGenerator = scope.ServiceProvider.GetRequiredService<IGuidGenerator>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var artistRepository = scope.ServiceProvider.GetRequiredService<IArtistRepository>();
            var venueRepository = scope.ServiceProvider.GetRequiredService<IVenueRepository>();
            var artistGuid = guidGenerator.GenerateGuid();
            artistId = new ArtistId(artistGuid);
            var venueGuid = guidGenerator.GenerateGuid();
            venueId = new VenueId(venueGuid);
            var artist = Artist.Create(artistId, "TestArtist").Value;
            var venue = Venue.Create(venueId, "TestVenue").Value;
            artistRepository.Add(artist);
            venueRepository.Add(venue);
            await unitOfWork.SaveChangesAsync(new CancellationToken());

        }
        using (var scope = _serviceProvider.CreateScope())
        {
            var guidGenerator = scope.ServiceProvider.GetRequiredService<IGuidGenerator>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var timeProvider = scope.ServiceProvider.GetRequiredService<TimeProvider>();
            var gigRepository = scope.ServiceProvider.GetRequiredService<IGigRepository>();

            newGigGuid = guidGenerator.GenerateGuid();
            var gig = Gig.Create(new GigId(newGigGuid), artistId, venueId,
                                   timeProvider.GetUtcNow().AddDays(5), timeProvider.GetUtcNow()).Value;

            var result = gigRepository.Add(gig);

            await unitOfWork.SaveChangesAsync(new CancellationToken());

        }
        using (var scope = _serviceProvider.CreateScope()) {
            var gigRepository = scope.ServiceProvider.GetRequiredService<IGigRepository>();
            var retrievedGig = await gigRepository.GetByIdReadOnlyAsync(new GigId(newGigGuid), new CancellationToken());
        
            Assert.NotNull(retrievedGig);
            Assert.Equal(newGigGuid, retrievedGig.Id.Value);
            Assert.Equal(artistId, retrievedGig.ArtistId);
            Assert.Equal(venueId, retrievedGig.VenueId);

        }
    }
}
