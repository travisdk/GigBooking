using GigBooking.WebAPI.IntegrationTests.Common;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace GigBooking.WebAPI.IntegrationTests.EndpointTests;

public class GigEndpointsTests :  IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private static string API_PREFIX = "/api/v1";


    public GigEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();

    }

    //[Fact]
    //public async Task CreateGig_WithValidData_ReturnsCreated()
    //{


    //    var newGig = GigBooking.Domain.Entities.Gig.Create(new )
    //    var response = await _client.PostAsJsonAsync(API_PREFIX+"/gigs",
            
             
    //        );

    //}

}
