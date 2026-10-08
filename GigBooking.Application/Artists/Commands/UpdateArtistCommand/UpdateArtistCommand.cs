using GigBooking.Domain.Abstractions;
using GigBooking.Domain.Primitives;
using GigBooking.Domain.ValueObjects;
using MediatR;
using GigBooking.Domain;

namespace GigBooking.Application.Artists.Commands.UpdateArtistCommand;

public sealed record UpdateArtistCommand(Guid Id, string Name) : IRequest<Result>;


public class UpdateArtistCommandHandler : IRequestHandler<UpdateArtistCommand, Result>
{
    private readonly IArtistRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateArtistCommandHandler(IArtistRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;

    }
    public async Task<Result> Handle(UpdateArtistCommand request, CancellationToken cancellationToken)
    {

        var artist = await _repository.GetByIdAsync(new ArtistId(request.Id), cancellationToken);
        if (artist == null)
        {
            return Result.Failure(Errors.Artist.KeyNotFound);
        }
        var result = artist.ChangeName(request.Name);
        if (result.IsFailure)
        {
            return result;
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}