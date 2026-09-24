using GigBooking.Application.Artists.Commands.CreateArtistCommand;
using GigBooking.Application.Artists.DTOs;
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

public class ArtistEndpointsTests :  BaseEndpointTests
{

    public ArtistEndpointsTests(CustomWebApplicationFactory factory): base(factory) {}

    [Fact] 
    public async Task GetById_WhenArtistExists_ReturnsOkAndArtistDTO()
    {
        var artistGuid = Guid.NewGuid();
        var artistName = "TestArtist";
        var artist = Artist.Create(new ArtistId(artistGuid), artistName).Value;
        await AddArtistsAsync(artist);

        var response = await Client.GetAsync(ApiRoutes.Artists.GetById(artistGuid));
      
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<ArtistDTO>();
        Assert.NotNull(result);
        Assert.Equal(artistGuid, result.Id);
        Assert.Equal(artistName, result.Name);

    }
    [Fact]
    public async Task GetById_WhenArtistIsNotFound_ReturnsNotFound()
    {
        var unknownArtistGuid = Guid.NewGuid();
        
        var response = await Client.GetAsync(ApiRoutes.Artists.GetById(unknownArtistGuid));
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<Error>();
        Assert.Equal(GigBooking.Domain.Errors.Artist.KeyNotFound, result );
    }
    
    [Fact]
    public async Task GetAll_ReturnsAllArtists()
    {
        var artist1Guid = Guid.NewGuid();
        var artist1Name = "TestArtist 1";
        var artist1 = Artist.Create(new ArtistId(artist1Guid), artist1Name).Value;
        var artist2Guid = Guid.NewGuid();
        var artist2Name = "TestArtist 2";
        var artist2 = Artist.Create(new ArtistId(artist2Guid), artist2Name).Value;
        await AddArtistsAsync(artist1, artist2);

        var response = await Client.GetAsync(ApiRoutes.Artists.Prefix);
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<IEnumerable<ArtistDTO>>();
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Single(result, a => a.Id == artist1Guid && a.Name.Equals(artist1Name));
        Assert.Single(result, a => a.Id == artist2Guid && a.Name.Equals(artist2Name));

    }

    [Fact]
    public async Task Create_WhenPayloadIsValid_ReturnsCreatedAndPersistsInDatabase()
    {
        var name = "TestArtist";
        var command = new CreateArtistCommand(name);
        
        var response = await Client.PostAsJsonAsync(ApiRoutes.Artists.Prefix, command);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        var result = await response.Content.ReadFromJsonAsync<ArtistDTO>();
        Assert.NotNull(result);
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var entityInDb = await dbContext.Set<Artist>().FindAsync(new ArtistId(result.Id));
        Assert.NotNull(entityInDb);
        Assert.Equal(name, entityInDb.Name);
           
    }

    [Fact]
    public async Task Create_WhenNameIsMissing_ReturnsBadRequestAndDoesNotPersist()
    {
        var name = string.Empty;
        var command = new CreateArtistCommand(name);
        
        var response = await Client.PostAsJsonAsync(ApiRoutes.Artists.Prefix, command);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<Error>();
        Assert.Equal(GigBooking.Domain.Errors.Artist.NoName, 
               result);
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var entitiesInDb = await dbContext.Set<Artist>().CountAsync();
        Assert.Equal(0, entitiesInDb);
    }
}
