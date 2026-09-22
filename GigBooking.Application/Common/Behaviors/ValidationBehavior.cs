using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using GigBooking.Domain.Primitives;
using Xunit.Sdk;

namespace GigBooking.Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
      where TRequest : IRequest<TResponse>
      where TResponse : IValidationResult<TResponse>
{

    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken))
            );


        var failures = validationResults.SelectMany(r => r.Errors).Where(f => f is not null).ToList();

        if (failures.Count != 0)
        {
            var firstFailure = failures[0];

            var error = new Error(firstFailure.ErrorCode, firstFailure.ErrorMessage);
            
            return TResponse.WithError(error);

        }

        return await next();
    }

  

}
