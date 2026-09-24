using FluentValidation.TestHelper;
using GigBooking.Application.Venues.Queries.GetVenue;
using GigBooking.Domain;

namespace GigBooking.Application.UnitTests.Venues.Queries;

public class GetGigQueryValidatorTests
{
    private readonly GetVenueQueryValidator _validator;
    public  GetGigQueryValidatorTests()
    {
        _validator = new GetVenueQueryValidator();
    }

    [Fact]
    public void Should_HaveError_WhenArtistIdIsEmpty() {
        var query = new GetVenueQuery(Guid.Empty);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.Id).WithErrorCode(
            Errors.Venue.EmptyVenueId.Code
            );
    }
}
