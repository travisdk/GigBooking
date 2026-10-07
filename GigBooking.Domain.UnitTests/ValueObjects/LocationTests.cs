using GigBooking.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace GigBooking.Domain.UnitTests.ValueObjects;

public class LocationTests
{

    [Fact]
    public void Create_WithValidParameters_ReturnsSuccess()
    {
        var result = Location.Create("TestStreet", "TestPostalCode", "TestCity", "DK");

        Assert.True(result.IsSuccess);
        Assert.Equal("TestStreet", result.Value.Street);
        Assert.Equal("TestPostalCode", result.Value.PostalCode);
        Assert.Equal("TestCity", result.Value.City);
        Assert.Equal("DK", result.Value.CountryCode);


    }
    [Theory]
    [InlineData(null, "TestPostalCode", "TestCity", "DK")]
    [InlineData("TestStreet", null, "TestCity", "DK")]
    [InlineData("TestStreet", "TestPostalCode", null, "DK")]
    public void Create_MissingFields_ReturnsFailure(string? street, string? postalCode,
         string? city, string? countryCode)
    {
        var result = Location.Create(street!, postalCode!, city!, countryCode!);
        Assert.True(result.IsFailure);
        Assert.Equal(Errors.Location.InvalidFields, result.Error);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("  ")]
    [InlineData("D")]
    [InlineData("DDK")]

    public void Create_InvalidCountryCode_ReturnsFailure(string? countryCode)
    {
        var result = Location.Create("TestStreet", "TestPostalCode","TestCity", countryCode!);
        Assert.True(result.IsFailure);
        Assert.Equal(Errors.Location.InvalidCountryCode, result.Error);
    }
}
