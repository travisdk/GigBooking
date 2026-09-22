using GigBooking.Application.Gigs.Commands;
using GigBooking.Application.Gigs.DTOs;
using GigBooking.Domain.Entities;
using GigBooking.Domain.Primitives;
using GigBooking.Domain.ValueObjects;
using GigBooking.Infrastructure.Persistence;
using GigBooking.WebAPI.IntegrationTests.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace GigBooking.WebAPI.IntegrationTests.EndpointTests;

public class GigEndpointsTests : BaseEndpointTests
{
    private readonly TimeProvider _timeProvider;
    public GigEndpointsTests(CustomWebApplicationFactory factory) : base(factory) {
    
        _timeProvider = Factory.Services.GetRequiredService<TimeProvider>();    
    

    }
    private async Task<int> CountEntitiesAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return await dbContext.Set<Gig>().CountAsync();
    }

    [Fact] 

    public async Task GetById_WhenGigExist_ReturnsOkAndGigDTO()
    {
        var artistGuid = Guid.NewGuid();
        var artist = Artist.Create(new ArtistId(artistGuid), "TestArtist").Value;
        var venueGuid = Guid.NewGuid();
        var venue = Venue.Create(new VenueId(venueGuid), "TestVenue").Value;
        await AddArtistsAsync(artist);
        await AddVenuesAsync(venue);
        var scheduledAt = _timeProvider.GetUtcNow().AddDays(7);
        var gigGuid = Guid.NewGuid();
        var gig = Gig.Create(new GigId(gigGuid), new ArtistId(artistGuid),
             new VenueId(venueGuid), scheduledAt, _timeProvider.GetUtcNow()).Value;
        await AddGigsAsync(gig);

        var response = await Client.GetAsync(ApiRoutes.Gigs.GetById(gigGuid));
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<GigDTO>();
        Assert.NotNull(result);
        Assert.Equal(gigGuid, result.Id);
        Assert.Equal(artistGuid, result.ArtistId);
        Assert.Equal(venueGuid, result.VenueId);
        Assert.Equal(scheduledAt, result.ScheduledAt);
        
    }
    [Fact]
    public async Task GetById_WhenGigDoesNotExist_ReturnsNotFound_And_Error()
    {
        var gigGuid = Guid.NewGuid();

        var response = await Client.GetAsync(ApiRoutes.Gigs.GetById(gigGuid));
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var error = await response.Content.ReadFromJsonAsync<Error>();
        Assert.Equal(GigBooking.Domain.Errors.DomainErrors.Gig.KeyNotFound, error);
       
    }
    [Fact]
    public async Task GetAll_ReturnsAllGigs()
    {
       
        var artist1 = Artist.Create(new ArtistId(Guid.NewGuid()), "TestArtist1").Value;
        var artist2 = Artist.Create(new ArtistId(Guid.NewGuid()), "TestArtist2").Value;
        var venue1 = Venue.Create(new VenueId(Guid.NewGuid()), "TestVenue1").Value;
        var venue2 = Venue.Create(new VenueId(Guid.NewGuid()), "TestVenue2").Value;
        await AddArtistsAsync(artist1, artist2);
        await AddVenuesAsync(venue1, venue2);
        var scheduledAt1 = _timeProvider.GetUtcNow().AddDays(7);
        var scheduledAt2 = _timeProvider.GetUtcNow().AddDays(3);

        var gig1 = Gig.Create(new GigId(Guid.NewGuid()), artist1.Id, venue1.Id
             , scheduledAt1, _timeProvider.GetUtcNow()).Value;
        var gig2 = Gig.Create(new GigId(Guid.NewGuid()), artist2.Id, venue2.Id
             , scheduledAt2, _timeProvider.GetUtcNow()).Value;
        await AddGigsAsync(gig1, gig2);

        var response = await Client.GetAsync(ApiRoutes.Gigs.Prefix);
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<IEnumerable<GigDetailsDTO>>();
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Single(result, g => g.Id == gig1.Id.Value && g.ArtistId == artist1.Id.Value && g.VenueId == venue1.Id.Value        
          && g.ScheduledAt == scheduledAt1 && g.ArtistName == artist1.Name && g.VenueName == venue1.Name);
        Assert.Single(result, g => g.Id == gig2.Id.Value && g.ArtistId == artist2.Id.Value && g.VenueId == venue2.Id.Value
        && g.ScheduledAt == scheduledAt2 && g.ArtistName == artist2.Name && g.VenueName == venue2.Name);

    }


    [Fact]
    public async Task Create_WhenPayloadIsValid_ReturnsCreatedAndPersistsToDatabase()
    {
        var artist = Artist.Create(new ArtistId(Guid.NewGuid()), "TestArtist").Value;
        var venue = Venue.Create(new VenueId(Guid.NewGuid()), "TestVenue").Value;
        await AddArtistsAsync(artist);
        await AddVenuesAsync(venue);
        var scheduledAt = _timeProvider.GetUtcNow().AddDays(7);
        var command = new CreateGigCommand(artist.Id.Value, venue.Id.Value, scheduledAt);

        var response =  await Client.PostAsJsonAsync(ApiRoutes.Gigs.Prefix, command);
        
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        var result = await response.Content.ReadFromJsonAsync<GigDTO>();
        Assert.NotNull(result);
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var entityInDb = await dbContext.Set<Gig>().FindAsync(new GigId(result.Id));
        Assert.NotNull(entityInDb);
        Assert.Equal(entityInDb.ArtistId, artist.Id);
        Assert.Equal(entityInDb.VenueId, venue.Id);
        Assert.Equal(entityInDb.ScheduledAt, scheduledAt);
    }

    [Fact]
    public async Task Create_When_ArtistDoesNotExist_ReturnsBadRequestAndDoesNotPersist ()
    {
        var venue = Venue.Create(new VenueId(Guid.NewGuid()), "TestVenue").Value;
        await AddVenuesAsync(venue);
        var scheduledAt = _timeProvider.GetUtcNow().AddDays(7);
        var command = new CreateGigCommand(Guid.NewGuid(), venue.Id.Value, scheduledAt);

        var response = await Client.PostAsJsonAsync(ApiRoutes.Gigs.Prefix, command);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await response.Content.ReadFromJsonAsync<Error>();
        Assert.Equal(GigBooking.Domain.Errors.DomainErrors.Artist.KeyNotFound, error);

        var entities = await CountEntitiesAsync();
        Assert.Equal(0, entities);

    }

    [Fact]
    public async Task Create_When_VenueDoesNotExist_ReturnsBadRequestAndDoesNotPersist()
    {
        var artist = Artist.Create(new ArtistId(Guid.NewGuid()), "TestArtist").Value;
        await AddArtistsAsync(artist);
        var scheduledAt = _timeProvider.GetUtcNow().AddDays(7);
        var command = new CreateGigCommand(artist.Id.Value, Guid.NewGuid(), scheduledAt);

        var response = await Client.PostAsJsonAsync(ApiRoutes.Gigs.Prefix, command);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await response.Content.ReadFromJsonAsync<Error>();
        Assert.Equal(GigBooking.Domain.Errors.DomainErrors.Venue.KeyNotFound, error);

        var entities = await CountEntitiesAsync();
        Assert.Equal(0, entities);
    }

    [Fact]
    public async Task Create_WhenInvalidScheduledAtDate_ReturnsBadRequestAndDoesNotPersist()
    {
        var artist = Artist.Create(new ArtistId(Guid.NewGuid()), "TestArtist").Value;
        var venue = Venue.Create(new VenueId(Guid.NewGuid()), "TestVenue").Value;
        await AddArtistsAsync(artist);
        await AddVenuesAsync(venue);
        var scheduledAt = _timeProvider.GetUtcNow().AddDays(-1); // Past = invalid
        var command = new CreateGigCommand(artist.Id.Value, venue.Id.Value, scheduledAt);

        var response = await Client.PostAsJsonAsync(ApiRoutes.Gigs.Prefix, command);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await response.Content.ReadFromJsonAsync<Error>();
        Assert.NotNull(error);
        Assert.Equal(Domain.Errors.DomainErrors.Gig.InvalidScheduledAtDate, error);
        var entities = await CountEntitiesAsync();
        Assert.Equal(0, entities);

    }
}
