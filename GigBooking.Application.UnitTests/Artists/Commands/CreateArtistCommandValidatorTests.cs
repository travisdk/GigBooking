using GigBooking.Application.Artists.Commands.CreateArtistCommand;
using FluentValidation.TestHelper;
using Xunit;
using GigBooking.Domain;

namespace GigBooking.Application.UnitTests.Artists.Commands;

public class CreateArtistCommandValidatorTests
{
    private readonly CreateArtistCommandValidator _validator;
    
    public CreateArtistCommandValidatorTests()
    {
        _validator = new CreateArtistCommandValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_Have_Error_WhenArtistName_IsNullOrEmpty(string? name)
    {
        var command = new CreateArtistCommand(name!);
        
        var result = _validator.TestValidate(command);
        
        result.ShouldHaveValidationErrorFor(x => x.Name)
             .WithErrorCode(Errors.Artist.NoName.Code);
       
    }

    [Fact]
    public void Should_NotHave_Error_WhenCommand_IsValid()
    {
        var command = new CreateArtistCommand("ArtistName");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
