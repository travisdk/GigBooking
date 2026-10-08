using GigBooking.Domain.Abstractions;
using GigBooking.Domain.Primitives;
using GigBooking.Domain.ValueObjects;
using MediatR;
using GigBooking.Domain;

namespace GigBooking.Application.Venues.Commands.UpdateVenueCommand;

public sealed record UpdateVenueCommand(Guid Id, string Name, Location Location) : IRequest<Result>;


public class UpdateVenueCommandHandler : IRequestHandler<UpdateVenueCommand, Result>
{
    private readonly IVenueRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateVenueCommandHandler(IVenueRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;

    }
    public async Task<Result> Handle(UpdateVenueCommand request, CancellationToken cancellationToken)
    {
        var venue = await _repository.GetByIdAsync(new VenueId(request.Id), cancellationToken);
        if (venue == null)
        {
            return Result.Failure(Errors.Venue.KeyNotFound);
        }
        var nameResult = venue.ChangeName(request.Name);
        if (nameResult.IsFailure)
        {
            return nameResult;
        }

        var locationResult = venue.ChangeLocation(request.Location);
        if (locationResult.IsFailure)
        {
            return locationResult;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}