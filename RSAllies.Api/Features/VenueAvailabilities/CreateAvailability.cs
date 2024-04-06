using Carter;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RSAllies.Api.Contracts;
using RSAllies.Api.Data;
using RSAllies.Api.Features.Sessions;
using RSAllies.Api.HelperTypes;

namespace RSAllies.Api.Features.VenueAvailabilities;

public abstract class CreateAvailability
{
    public class Command : IRequest<Result<Guid>>
    {
        public Guid VenueId { get; set; }
        public DateTime AvailableDate { get; set; }
    }
    
    internal sealed class Handler(AppDbContext context) : IRequestHandler<Command, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            var venue = await context.Venues
                .Where(v => v.Id == request.VenueId && !v.IsDeleted)
                .SingleOrDefaultAsync(cancellationToken);

            if (venue is null)
            {
                return Result.Failure<Guid>(new Error("CreateAvailability.Non-ExistentVenue",
                    "The specified venue does not exist"));
            }

            var availability = new Entities.VenueAvailability
            {
                Id = Guid.NewGuid(),
                VenueId = venue.Id,
                Venue = venue,
                AvailableDate = request.AvailableDate,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            context.VenueAvailabilities.Add(availability);

            await context.SaveChangesAsync(cancellationToken);

            return availability.Id;
        }
    }
}

public class CreateAvailabilityEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/venue/availability", async (VenueAvailabilityDto availability, ISender sender) =>
        {
            var request = availability.Adapt<CreateAvailability.Command>();
            var result = await sender.Send(request);
            return result.IsFailure ? Results.NotFound(result.Error) : Results.Ok(result);
        });
    }
}