using GigBooking.Application.Artists.Commands.CreateArtistCommand;
using FluentValidation.TestHelper;
using Xunit;

namespace GigBooking.Application.UnitTests.Artists.Commands;

public class CreateArtistCommandValidatorTests
{
    private readonly CreateArtistCommandValidator _validator;
    
    public CreateArtistCommandValidatorTests()
    {
        _validator = new CreateArtistCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_WhenArtistName_IsNullOrEmpty()
    {
        var command = new CreateArtistCommand(string.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
}
