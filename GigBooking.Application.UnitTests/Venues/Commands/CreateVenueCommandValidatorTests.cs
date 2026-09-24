using FluentValidation.TestHelper;
using GigBooking.Application.Venues.Commands.CreateVenueCommand;
using GigBooking.Domain;
using Xunit;

namespace GigBooking.Application.UnitTests.Venues.Commands;

public class CreateVenueCommandValidatorTests
{
    private readonly CreateVenueCommandValidator _validator;
    
    public CreateVenueCommandValidatorTests()
    {
        _validator = new CreateVenueCommandValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_Have_Error_WhenVenueName_IsNullOrEmpty(string? name)
    {
        var command = new CreateVenueCommand(name!);
        
        var result = _validator.TestValidate(command);
        
        result.ShouldHaveValidationErrorFor(x => x.Name)
             .WithErrorCode(Errors.Venue.NoName.Code);
       
    }

    [Fact]
    public void Should_NotHave_Error_WhenCommand_IsValid()
    {
        var command = new CreateVenueCommand("VenueName");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
