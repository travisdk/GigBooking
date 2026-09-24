using FluentValidation.TestHelper;
using GigBooking.Application.Gigs.Commands;
using GigBooking.Domain;
using GigBooking.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace GigBooking.Application.UnitTests.Gigs.Commands;

public class CreateGigCommandValidatorTests
{

    private readonly TimeProvider _timeProvider = TimeProvider.System;
    private readonly CreateGigCommandValidator _validator;

    public CreateGigCommandValidatorTests()
    {
        _validator = new CreateGigCommandValidator(_timeProvider);
    }


    [Fact]
    public void Should_Have_Error_WhenScheduledAt_Is_In_The_Past()
    {
        var pastDate = _timeProvider.GetUtcNow().AddDays(-1);
        var command = new CreateGigCommand(Guid.NewGuid(), Guid.NewGuid(), pastDate);
        
        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ScheduledAt).WithErrorCode(
            Errors.Gig.InvalidScheduledAtDate.Code);

    }

    [Fact]
    public void Should_Have_Error_WhenArtistId_Is_Empty()
    {
        var futureDate = _timeProvider.GetUtcNow().AddDays(7);
        var command = new CreateGigCommand(Guid.Empty, Guid.NewGuid(), futureDate);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ArtistId).
            WithErrorCode(Errors.Gig.EmptyArtistId.Code);

    }
    [Fact]
    public void Should_Have_Error_WhenVenueId_Is_Empty()
    {
        var futureDate = _timeProvider.GetUtcNow().AddDays(7);
        var command = new CreateGigCommand(Guid.NewGuid(), Guid.Empty, futureDate);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.VenueId).
            WithErrorCode(Errors.Gig.EmptyVenueId.Code);
        

    }
    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        var futureDate = _timeProvider.GetUtcNow().AddDays(7);
        var command = new CreateGigCommand(Guid.NewGuid(), Guid.NewGuid(), futureDate);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();

    }
}
