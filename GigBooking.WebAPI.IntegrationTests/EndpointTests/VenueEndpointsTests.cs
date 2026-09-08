using GigBooking.Application.Artists.Commands.CreateArtistCommand;
using GigBooking.Application.Artists.DTOs;
using GigBooking.Application.Venues.Commands.CreateVenueCommand;
using GigBooking.Application.Venues.DTOs;
using GigBooking.Domain.Entities;

using GigBooking.Domain.Primitives;
using GigBooking.Domain.ValueObjects;
using GigBooking.Infrastructure.Persistence;
using GigBooking.WebAPI.IntegrationTests.Common;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using Testcontainers.MsSql;
namespace GigBooking.WebAPI.IntegrationTests.EndpointTests;

public class VenueEndpointsTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;
    public VenueEndpointsTests(CustomWebApplicationFactory factory)

    {
        _factory = factory;
        _client = factory.CreateClient();
    }


    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
    }

    public Task DisposeAsync()
    {
      return   Task.CompletedTask;
    }

    // HELPERS
    private async Task AddVenuesAsync(params Venue[] venues)
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Set<Venue>().AddRange(venues); // sync
            await dbContext.SaveChangesAsync();
        }
    }

    [Fact] 
    public async Task GetById_WhenVenueExists_ReturnsOkAndVenueDTO()
    {
        var venueGuid = Guid.NewGuid();
        var venueName = "TestVenue";
        var venue = Venue.Create(new VenueId(venueGuid), venueName).Value;
        await AddVenuesAsync(venue);

        var response = await _client.GetAsync(ApiRoutes.Venues.GetById(venueGuid));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<VenueDTO>();
        Assert.NotNull(result);
        Assert.Equal(venueGuid, result.Id);
        Assert.Equal(venueName, result.Name);

    }
    [Fact]
    public async Task GetById_WhenVenueIsNotFound_ReturnsNotFound()
    {
        var unknownVenueGuid = Guid.NewGuid();
        var response = await _client.GetAsync(ApiRoutes.Venues.GetById(unknownVenueGuid));
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<Error>();

        Assert.Equal(GigBooking.Domain.Errors.DomainErrors.Venue.KeyNotFound, result );
    }
    
    [Fact]
    public async Task GetAll_ReturnsAllVenues()
    {
        var venue1Guid = Guid.NewGuid();
        var venue1Name = "TestVenue 1";
        var venue1 = Venue.Create(new VenueId(venue1Guid), venue1Name).Value;
        var venue2Guid = Guid.NewGuid();
        var venue2Name = "TestVenue 2";
        var venue2 = Venue.Create(new VenueId(venue2Guid), venue2Name).Value;

        await AddVenuesAsync(venue1, venue2);

        var response = await _client.GetAsync(ApiRoutes.Venues.Prefix);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<VenueDTO>>();
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Single(result, a => a.Id == venue1Guid && a.Name.Equals(venue1Name));
        Assert.Single(result, a => a.Id == venue2Guid && a.Name.Equals(venue2Name));

    }

    [Fact]
    public async Task Create_WhenPayloadIsValid_ReturnsCreatedAndPersistsInDatabase()
    {
        var name = "TestVenue";
        var command = new CreateVenueCommand(name);
        var response = await _client.PostAsJsonAsync(ApiRoutes.Venues.Prefix, command);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        
        var result = await response.Content.ReadFromJsonAsync<VenueDTO>();

        Assert.NotNull(result);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var entityInDb = await dbContext.Set<Venue>().FindAsync(new VenueId(result.Id));
        Assert.NotNull(entityInDb);
        Assert.Equal(name, entityInDb.Name);
           
    }

    [Fact]
    public async Task Create_WhenNameIsMissing_ReturnsBadRequestAndDoesNotPersist()
    {
        var name = string.Empty;
        var command = new CreateVenueCommand(name);
        var response = await _client.PostAsJsonAsync(ApiRoutes.Venues.Prefix, command);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<Error>();

        Assert.Equal(GigBooking.Domain.Errors.DomainErrors.Venue.NoName, 
               result);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var entitiesInDb = await dbContext.Set<Venue>().CountAsync();
        
        Assert.Equal(0, entitiesInDb);
    }
}
