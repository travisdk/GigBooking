using FluentValidation.TestHelper;
using GigBooking.Application.Gigs.Queries.GetGig;
using GigBooking.Application.Venues.Queries.GetVenue;
using GigBooking.Domain;

namespace GigBooking.Application.UnitTests.Gigs.Queries;

public class GetGigQueryValidatorTests
{
    private readonly GetGigQueryValidator _validator;
    public GetGigQueryValidatorTests()
    {
        _validator = new GetGigQueryValidator();
    }

    [Fact]
    public void Should_HaveError_WhenGigIdIsEmpty()
    {
        var query = new GetGigQuery(Guid.Empty);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.Id).WithErrorCode(
            Errors.Gig.EmptyGigId.Code
            );
    }
}