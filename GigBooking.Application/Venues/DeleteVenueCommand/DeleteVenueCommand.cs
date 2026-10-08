using GigBooking.Domain.Abstractions;
using GigBooking.Domain.Primitives;
using GigBooking.Domain.ValueObjects;
using MediatR;
using GigBooking.Domain;

namespace GigBooking.Application.Venues.Commands.DeleteVenueCommand;

public sealed record DeleteVenueCommand(Guid Id) : IRequest<Result>;


public class DeleteVenueCommandHandler : IRequestHandler<DeleteVenueCommand, Result>
{

    private readonly IVenueRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteVenueCommandHandler(IVenueRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;

    }
    public async Task<Result> Handle(DeleteVenueCommand request, CancellationToken cancellationToken)
    {

        var venue = await _repository.GetByIdAsync(new VenueId(request.Id), cancellationToken);
        if (venue == null)
        {
            return Result.Failure(Errors.Venue.KeyNotFound);
        }
        
        var result = venue.DeActivate();
        if (result.IsFailure)
        {
            return result;
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}