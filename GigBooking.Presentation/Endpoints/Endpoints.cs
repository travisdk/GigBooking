using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;


namespace GigBooking.Presentation.Endpoints;

public static class Endpoints
{

    public static void MapEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/", () => "Hello World!");
    }
}
