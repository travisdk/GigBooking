using GigBooking.Domain.Entities;
using GigBooking.Domain.Primitives;
using GigBooking.Infrastructure.Persistence;
using GigBooking.WebAPI.IntegrationTests.Common;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;

namespace GigBooking.WebAPI.IntegrationTests.EndpointTests;

public abstract class BaseEndpointTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    protected readonly CustomWebApplicationFactory Factory;
    protected readonly HttpClient Client;

    protected BaseEndpointTests(CustomWebApplicationFactory factory)
    {
        Factory = factory;
        Client = Factory.CreateClient();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task InitializeAsync()
    {
        await Factory.ResetDatabaseAsync();
    }


    // HELPERS

    protected async Task AddEntitesAsync<TEntity>(params TEntity[] entities) where TEntity : class
    {
        using (var scope = Factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Set<TEntity>().AddRange(entities); // sync
            await dbContext.SaveChangesAsync();
        }
    }
    protected async Task AddArtistsAsync(params Artist[] artists) => await AddEntitesAsync(artists);
    protected async Task AddVenuesAsync(params Venue[] venues) => await AddEntitesAsync(venues);
    protected async Task AddGigsAsync(params Gig[] gigs) => await AddEntitesAsync(gigs);


}
