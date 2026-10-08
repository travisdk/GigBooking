using GigBooking.Application.Artists.Commands;
using GigBooking.Application.Artists.Commands.CreateArtistCommand;
using GigBooking.Application.Artists.DTOs;
using GigBooking.Domain;
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
using static GigBooking.WebAPI.Endpoints.Endpoints;
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
    [Fact] 
    public async Task Update_NameIsMissing_ReturnsBadRequestAndDoesNotPersist()
    {
        var artist1Guid = Guid.NewGuid();
        var artist1Name = "TestArtist";
        var artist1 = Artist.Create(new ArtistId(artist1Guid), artist1Name).Value;
        await AddArtistsAsync(artist1); 
        var newName = string.Empty;
        var request = new UpdateArtistRequest(newName);

        var response = await Client.PutAsJsonAsync($"{ ApiRoutes.Artists.Prefix}/{artist1Guid}", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<Error>();
        Assert.Equal(GigBooking.Domain.Errors.Artist.NoName, result);
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var entityInDb = await dbContext.Set<Artist>().AsNoTracking()
            .FirstOrDefaultAsync(artist => artist.Id == artist1.Id);
        Assert.NotNull(entityInDb);
        Assert.Equal(artist1Name, entityInDb.Name);

    }

    [Fact]
    public async Task Update_WhenNameIsValid_ReturnsNoContentAndPersistsToDatabase()
    {
        var artist1Guid = Guid.NewGuid();
        var artist1Name = "TestArtist";
        var artist1 = Artist.Create(new ArtistId(artist1Guid), artist1Name).Value;
        await AddArtistsAsync(artist1);
        var newName = "NewName";
        var request = new UpdateArtistRequest(newName);
        
        var response = await Client.PutAsJsonAsync($"{ApiRoutes.Artists.Prefix}/{artist1Guid}", request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var entityInDb = await dbContext.Set<Artist>().AsNoTracking()
            .FirstOrDefaultAsync(artist => artist.Id == artist1.Id);
        Assert.NotNull(entityInDb);
        Assert.Equal(newName, entityInDb.Name);
    }

    [Fact]
    public async Task Update_WhenArtistIsNotFound_ReturnsFailureAndDoesNotPersist()
    {
        var artistGuid = Guid.NewGuid();
        var artistName = "TestArtist";
        var artist = Artist.Create(new ArtistId(artistGuid), artistName).Value;
        await AddArtistsAsync(artist);
        var newName = "NewName";
        var request = new UpdateArtistRequest(newName);
        var wrongGuid = Guid.NewGuid();

        var response = await Client.PutAsJsonAsync($"{ApiRoutes.Artists.Prefix}/{wrongGuid}", request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var error = await response.Content.ReadFromJsonAsync<Error>();
        Assert.Equal(Errors.Artist.KeyNotFound, error);
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var entityInDb = await dbContext.Set<Artist>().AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == artist.Id);
        Assert.NotNull(entityInDb);
        Assert.Equal(artistName, entityInDb.Name);

    }

    [Fact]
    public async Task Delete_WhenArtistIsActive_ReturnsNoContentAndPersistsToDatabase()
    {
        var artist1Guid = Guid.NewGuid();
        var artist1Name = "TestArtist";
        var artist1 = Artist.Create(new ArtistId(artist1Guid), artist1Name).Value;
        await AddArtistsAsync(artist1); // active by default

        var response = await Client.DeleteAsync($"{ApiRoutes.Artists.Prefix}/{artist1Guid}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var entityInDb = await dbContext.Set<Artist>().AsNoTracking()
            .FirstOrDefaultAsync(artist => artist.Id == artist1.Id);
        Assert.NotNull(entityInDb); 
        Assert.False(entityInDb.IsActive);
    }
    [Fact]
    public async Task Delete_WhenArtistIsAlreadyInactive_ReturnsBadRequestAndDoesNotPersistToDatabase()
    {
        var artist1Guid = Guid.NewGuid();
        var artist1Name = "TestArtist";
        var artist1 = Artist.Create(new ArtistId(artist1Guid), artist1Name).Value;
        artist1.DeActivate();
        await AddArtistsAsync(artist1);

        var response = await Client.DeleteAsync($"{ApiRoutes.Artists.Prefix}/{artist1Guid}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<Error>();
        Assert.NotNull(result);
        Assert.Equal(Domain.Errors.Artist.AlreadyInactive, result);
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var entityInDb = await dbContext.Set<Artist>().AsNoTracking()
            .FirstOrDefaultAsync(artist => artist.Id == artist1.Id);
        Assert.NotNull(entityInDb); // still there
        Assert.False(entityInDb.IsActive);
    }


    [Fact]
    public async Task Delete_WhenArtistIsNotFound_ReturnsNotFound()
    {
        var artist1Guid = Guid.NewGuid();
        var artist1Name = "TestArtist";
        var artist1 = Artist.Create(new ArtistId(artist1Guid), artist1Name).Value;
        await AddArtistsAsync(artist1);
        var wrongGuid = Guid.NewGuid();
        var response = await Client.DeleteAsync($"{ApiRoutes.Artists.Prefix}/{wrongGuid}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<Error>();
        Assert.NotNull(result);
        Assert.Equal(Domain.Errors.Artist.KeyNotFound, result);
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var entityInDb = await dbContext.Set<Artist>().AsNoTracking()
            .FirstOrDefaultAsync(artist => artist.Id == artist1.Id);
        Assert.NotNull(entityInDb); // still there
    }

}
