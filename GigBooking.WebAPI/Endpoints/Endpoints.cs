
using GigBooking.Application.Artists.Commands;
using GigBooking.Application.Artists.Commands.CreateArtistCommand;
using GigBooking.Application.Artists.Queries.GetArtist;
using GigBooking.Application.Artists.Queries.GetArtists;
using GigBooking.Application.Gigs.Commands;
using GigBooking.Application.Gigs.DTOs;
using GigBooking.Application.Gigs.Queries.GetGig;
using GigBooking.Application.Gigs.Queries.GetGigs;
using GigBooking.Application.Venues.Commands.CreateVenueCommand;
using GigBooking.Application.Venues.Queries.GetVenue;
using GigBooking.Application.Venues.Queries.GetVenues;
using GigBooking.Domain;
using MediatR;

namespace GigBooking.WebAPI.Endpoints;

public static class Endpoints
{
    public sealed record UpdateArtistRequest(string Name); // helper
     

    public static void MapEndpoints(this IEndpointRouteBuilder app)
    {
        var apiV1 = app.MapGroup("/api/v1");
        MapArtistEndpoints(apiV1);
        MapVenueEndpoints(apiV1);
        MapGigEndpoints(apiV1);
    }

    private static void MapGigEndpoints(RouteGroupBuilder apiGroup)
    {
        var gigGroup = apiGroup.MapGroup("/gigs").WithTags("gigs");
        gigGroup.MapGet("/", async (ISender sender, CancellationToken ctx) =>
        {
            var results = await sender.Send(new GetGigsQuery(), ctx);
            return Results.Ok(results.Value);
        });
        gigGroup.MapGet("/{id:guid}", 
            async (Guid id, ISender sender, CancellationToken ctx) =>
        {

            var result = await sender.Send(new GetGigQuery(id), ctx);
            if (result.IsFailure)
            {
                return Results.NotFound(result.Error);
            }
            return Results.Ok(result.Value);

        }).WithName("GetGigById");

        gigGroup.MapPost("/", async (CreateGigCommand command, ISender sender, CancellationToken ctx)
            =>
        {
            var result = await sender.Send(command, ctx);
            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error!);
            }
            GigDTO gig = result.Value;
            return Results.CreatedAtRoute("GetGigById", new { id = gig.Id }, gig);
        });
    }



    public static void MapArtistEndpoints(IEndpointRouteBuilder apiGroup)
    {

        var artistGroup = apiGroup.MapGroup("/artists").WithTags("artists");

        artistGroup.MapGet("/", async (ISender sender, CancellationToken ctx) =>
        {
            var result = await sender.Send(new GetArtistsQuery(), ctx);
            return Results.Ok(result.Value);
        });


        artistGroup.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken ctx) =>
        {
            var result = await sender.Send(new GetArtistQuery(id), ctx);
            if (result.IsFailure)
            {
                return Results.NotFound(result.Error);
            }
            return Results.Ok(result.Value);
        }).WithName("GetArtistById");
        artistGroup.MapPost("/", async (CreateArtistCommand command, ISender sender, CancellationToken ctx) =>
        {
            var result = await sender.Send(command, ctx);
            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }
            var artist = result.Value;
            return Results.CreatedAtRoute("GetArtistById", new { id = artist.Id }, artist);
        });

        artistGroup.MapPut("/{id:guid}", async (Guid id, UpdateArtistRequest request, ISender sender, CancellationToken ctx) =>
        {

           UpdateArtistCommand command = new UpdateArtistCommand(id, request.Name);

            var result = await sender.Send(command, ctx);
            if (result.IsFailure)
            {
                return result.Error == Errors.Artist.KeyNotFound ? Results.NotFound(result.Error) : Results.BadRequest(result.Error);
            }
            return Results.NoContent();
        }).WithName("UpdateArtist");

        artistGroup.MapDelete("/{id:guid}", async (Guid id, ISender sender, CancellationToken ctx)

             =>
        {
            var command  = new DeleteArtistCommand(id);
            var result = await sender.Send(command, ctx);

            if (result.IsFailure)
            {
                return result.Error == Errors.Artist.KeyNotFound ? Results.NotFound(result.Error) :
                   Results.BadRequest(result.Error);
            }
            return Results.NoContent();

        }).WithName("DeleteArtist");
    }
    public static void MapVenueEndpoints(IEndpointRouteBuilder apiGroup)
    {
        var venueGroup =  apiGroup.MapGroup("/venues").WithTags("venues");
        venueGroup.MapGet("/", async (ISender sender, CancellationToken ctx) => 
        
        {
            var result = await sender.Send(new GetVenuesQuery(), ctx);
            return Results.Ok(result.Value);
        });


        venueGroup.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken ctx) =>
        {
            var result = await sender.Send(new GetVenueQuery(id), ctx);
            if (result.IsFailure)
            {
                return Results.NotFound(result.Error);
            }
            return Results.Ok(result.Value);
        }).WithName("GetVenueById");


        venueGroup.MapPost("/", async (CreateVenueCommand command, ISender sender, CancellationToken ctx) =>
        {
            var result = await sender.Send(command, ctx);
            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }
            var venue = result.Value;
            return Results.CreatedAtRoute("GetVenueById", new { id = venue.Id }, venue);
        });

    }
}
