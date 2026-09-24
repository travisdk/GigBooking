using MediatR;
using FluentValidation;

using GigBooking.Application.Common.Interfaces;
using GigBooking.Application.Gigs.DTOs;
using GigBooking.Domain.Abstractions;
using GigBooking.Domain.Entities;
using GigBooking.Domain;
using GigBooking.Domain.Primitives;
using GigBooking.Domain.Services;
using GigBooking.Domain.ValueObjects;


namespace GigBooking.Application.Gigs.Commands;

public sealed record CreateGigCommand(Guid ArtistId, Guid VenueId, DateTimeOffset ScheduledAt) : IRequest<Result<GigDTO>>;

public class CreateGigCommandHandler : IRequestHandler<CreateGigCommand, Result<GigDTO>>
{
    private readonly IGigRepository _repository;
    private readonly IArtistRepository _artistRepository;
    private readonly IVenueRepository _venueRepository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGigBookingService _gigBookingService;
    private readonly TimeProvider _timeProvider;

    public CreateGigCommandHandler(IGigRepository repository, IArtistRepository artistRepository, 
        IVenueRepository venueRepository, IGigBookingService gigBookingService, IUnitOfWork unitOfWork, IGuidGenerator guidGenerator,
         TimeProvider timeProvider)
    {
        _repository = repository;
        _artistRepository = artistRepository;
        _venueRepository = venueRepository;
        _guidGenerator = guidGenerator;
        _unitOfWork = unitOfWork;
        _gigBookingService = gigBookingService;
        _timeProvider = timeProvider;
    }
    public async Task<Result<GigDTO>> Handle(CreateGigCommand request, CancellationToken cancellationToken)
    {
       
        if (await _artistRepository.GetByIdReadOnlyAsync(new ArtistId(request.ArtistId),
              cancellationToken) == null)
        {
            return Result<GigDTO>.Failure(Errors.Artist.KeyNotFound);
        }

        if (await _venueRepository.GetByIdReadOnlyAsync(new VenueId(request.VenueId), cancellationToken) == null)
        {
            return Result<GigDTO>.Failure(Errors.Venue.KeyNotFound);
        }


        var existingGigsOnDate = await _repository.GetGigsByDateAsync(request.ScheduledAt, cancellationToken);
        var bookingAllowed = _gigBookingService.CanBook(new ArtistId(request.ArtistId), 
              new VenueId(request.VenueId), existingGigsOnDate);
        if (bookingAllowed.IsFailure)
        {
            return Result<GigDTO>.Failure(bookingAllowed.Error!);
        }

        var gigId = new GigId(_guidGenerator.GenerateGuid());
        var gigResult = Gig.Create(gigId, new ArtistId(request.ArtistId), 
               new VenueId(request.VenueId),
               request.ScheduledAt, _timeProvider.GetUtcNow());
               
        if (gigResult.IsFailure)
        {
            return Result<GigDTO>.Failure(gigResult.Error!);
        }
        var gig = gigResult.Value;
        _repository.Add(gig);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        GigDTO gigDTO = new GigDTO(gig.Id.Value, gig.ArtistId.Value,
              gig.VenueId.Value, gig.ScheduledAt); 
        return Result<GigDTO>.Success(gigDTO);          
        
    }
}


public class CreateGigCommandValidator : AbstractValidator<CreateGigCommand>
{
    public CreateGigCommandValidator(TimeProvider timeProvider)
    {
        // 1. Tjek at ArtistId ikke er tomt (Guid.Empty)
        RuleFor(x => x.ArtistId)
                .NotEmpty()
                .WithErrorCode(Errors.Gig.EmptyArtistId.Code)
                .WithMessage(Errors.Gig.EmptyArtistId.Description);

        RuleFor(x => x.VenueId)
            .NotEmpty()
            .WithErrorCode(Errors.Gig.EmptyVenueId.Code)
            .WithMessage(Errors.Gig.EmptyVenueId.Description);

        RuleFor(x => x.ScheduledAt)
            .GreaterThan(x => timeProvider.GetUtcNow())
            .WithErrorCode(Errors.Gig.InvalidScheduledAtDate.Code)
            .WithMessage(Errors.Gig.InvalidScheduledAtDate.Description);
    }
}
