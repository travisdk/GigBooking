
using GigBooking.Application.Artists.Commands.CreateArtistCommand;
using GigBooking.Application.Artists.Queries.GetArtist;
using GigBooking.Application.Artists.Queries.GetArtists;
using GigBooking.Application.Gigs.Commands;
using GigBooking.Application.Gigs.DTOs;
using GigBooking.Application.Gigs.Queries;
using GigBooking.Application.Gigs.Queries.GetGig;
using GigBooking.Application.Venues.Commands.CreateVenueCommand;
using GigBooking.Application.Venues.Queries.GetVenue;
using GigBooking.Application.Venues.Queries.GetVenues;
using GigBooking.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace GigBooking.WebAPI.Endpoints;

public static class Endpoints
{

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

        gigGroup.MapPost("/", async (CreateGigCommand request, ISender sender, CancellationToken ctx)
            =>
        {
            var result = await sender.Send(request, ctx);
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
        artistGroup.MapPost("/", async (CreateArtistCommand request, ISender sender, CancellationToken ctx) =>
        {
            var result = await sender.Send(request, ctx);
            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }
            var artist = result.Value;
            return Results.CreatedAtRoute("GetArtistById", new { id = artist.Id }, artist);
        });

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


        venueGroup.MapPost("/", async (CreateVenueCommand request, ISender sender, CancellationToken ctx) =>
        {
            var result = await sender.Send(request, ctx);
            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }
            var venue = result.Value;
            return Results.CreatedAtRoute("GetVenueById", new { id = venue.Id }, venue);
        });

    }
}
