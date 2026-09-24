using FluentValidation.TestHelper;
using GigBooking.Application.Artists.Queries.GetArtist;
using GigBooking.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using static GigBooking.Application.Artists.Queries.GetArtist.GetArtistQueryHandler;

namespace GigBooking.Application.UnitTests.Artists.Queries;

public class GetVenueQueryValidatorTests
{
    private readonly GetArtistQueryValidator _validator;
    public  GetVenueQueryValidatorTests()
    {
        _validator = new GetArtistQueryValidator();
    }

    [Fact]
    public void Should_HaveError_WhenArtistIdIsEmpty() {
        var query = new GetArtistQuery(Guid.Empty);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.Id).WithErrorCode(
            Errors.Artist.EmptyArtistId.Code
            );
    }
}
