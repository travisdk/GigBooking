using FluentValidation;
using GigBooking.Application.Common.Behaviors;
using GigBooking.Domain.Primitives;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GigBooking.Application.UnitTests.Common.Behavior;

public class ValidationBehaviorTests
{
    [Fact]
    public async Task Handle_ShouldReturnFailureAndNotCallNext_WhenValidationFails()
    {
        // Arrange
        var nextWasCalled = false;
        RequestHandlerDelegate<Result<string>> next = (cancellationToken) =>
        {
            nextWasCalled = true;
            return Task.FromResult(Result<string>.Success("Everything OK"));
        };
        var validator = new FailingTestCommandValidator();
        var validators = new List<IValidator<TestCommand>> { validator };
        var behaviour = new ValidationBehavior<TestCommand, Result<string>>(validators);
        var command = new TestCommand();

        // Act
        var result = await behaviour.Handle(command, next, CancellationToken.None);

        // Assert
        Assert.False(nextWasCalled); // unchanged
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal("Test.Error", result.Error.Code);
    }
    [Fact]
    public async Task Handle_ShouldReturnSuccessAndCallNext_WhenValidationSucceeds()
    {
        // Arrange
        var nextWasCalled = false;
        RequestHandlerDelegate<Result<string>> next = (cancellationToken) =>
        {
            nextWasCalled = true;
            return Task.FromResult(Result<string>.Success("Everything OK"));
        };
        var validator = new SucceedingTestCommandValidatior();
        var validators = new List<IValidator<TestCommand>> { validator };
        var behaviour = new ValidationBehavior<TestCommand, Result<string>>(validators);
        var command = new TestCommand();

        // Act
        var result = await behaviour.Handle(command, next, CancellationToken.None);

        Assert.True(nextWasCalled); // changed
        Assert.True(result.IsSuccess);
        Assert.Equal("Everything OK", result.Value);

    }

    [Fact]
    public async Task Handle_ShouldCallNext_WhenNoValidatorsExists()
    {
        // Arrange
        var nextWasCalled = false;
        RequestHandlerDelegate<Result<string>> next = (cancellationToken) =>
        {
            nextWasCalled = true;
            return Task.FromResult(Result<string>.Success("Everything OK"));
        };

        var validators = Enumerable.Empty<IValidator<TestCommand>>();
        var behaviour = new ValidationBehavior<TestCommand, Result<string>>(validators);
        var command = new TestCommand();

        // Act
        var result = await behaviour.Handle(command, next, CancellationToken.None);

        // Assert
        Assert.True(nextWasCalled); // changed
        Assert.True(result.IsSuccess);
        Assert.Equal("Everything OK", result.Value);


    }
}


public sealed record TestCommand() : IRequest<Result<string>>;
public sealed class FailingTestCommandValidator : AbstractValidator<TestCommand>
{
    public FailingTestCommandValidator()
    {
        RuleFor(x => x).Custom((command, context) =>
        {
            context.AddFailure(new FluentValidation.Results.ValidationFailure("TestProperty", "Something went wrong")
            {
                ErrorCode = "Test.Error"
            });

        });
    }
}
public sealed class SucceedingTestCommandValidatior : AbstractValidator<TestCommand>
{
    public SucceedingTestCommandValidatior()
    {
    }
}
