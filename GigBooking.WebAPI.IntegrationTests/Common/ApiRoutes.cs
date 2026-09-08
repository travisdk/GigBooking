using System;
using System.Collections.Generic;
using System.Text;

namespace GigBooking.WebAPI.IntegrationTests.Common;

public static class ApiRoutes
{

    private const string Base = "/api/v1";

    public static class Artists
    {
        public const string Prefix = $"{Base}/artists";
        public static string GetById(Guid id) => $"{Prefix}/{id}";
    }
    public static class Venues
    {
        public const string Prefix = $"{Base}/Venues";
        public static string GetById(Guid id) => $"{Prefix}/{id}";
    }
    public static class Gigs
    {
        public const string Prefix = $"{Base}/gigs";
        public static string GetById(Guid id) => $"{Prefix}/{id}";
    }
}
