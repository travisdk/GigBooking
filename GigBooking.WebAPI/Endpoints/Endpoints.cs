
using GigBooking.Application.Artists.Commands.CreateArtistCommand;
using GigBooking.Application.Artists.Queries.GetArtist;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace GigBooking.WebAPI.Endpoints;

public static class Endpoints
{

    public static void MapEndpoints(this IEndpointRouteBuilder app)
    {
        var apiV1 = app.MapGroup("/api/v1");
        MapArtistEndpoints(apiV1);
        //.. more
    }

    public static void MapArtistEndpoints(IEndpointRouteBuilder apiGroup)
    {
        var artistGroup = apiGroup.MapGroup("/artists").WithTags("artists");

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
}
