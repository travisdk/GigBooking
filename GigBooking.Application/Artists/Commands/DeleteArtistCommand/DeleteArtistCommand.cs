using GigBooking.Domain.Abstractions;
using GigBooking.Domain.Primitives;
using GigBooking.Domain.ValueObjects;
using MediatR;
using GigBooking.Domain;

namespace GigBooking.Application.Artists.Commands;

public sealed record DeleteArtistCommand(Guid Id) : IRequest<Result>;


public class DeleteArtistCommandHandler : IRequestHandler<DeleteArtistCommand, Result>
{
    private readonly IArtistRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteArtistCommandHandler(IArtistRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;

    }
    public async Task<Result> Handle(DeleteArtistCommand request, CancellationToken cancellationToken)
    {

        var artist = await _repository.GetByIdAsync(new ArtistId(request.Id), cancellationToken);
        if (artist == null)
        {
            return Result.Failure(Errors.Artist.KeyNotFound);
        }
        
        var result = artist.DeActivate();
        if (result.IsFailure)
        {
            return result;
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}