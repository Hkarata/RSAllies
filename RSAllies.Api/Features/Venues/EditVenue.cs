using Carter;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RSAllies.Api.Contracts;
using RSAllies.Api.Data;
using RSAllies.Api.HelperTypes;

namespace RSAllies.Api.Features.Venues;

public abstract class EditVenue
{
    public class Command : IRequest<Result<VenueDto>>
    {
        public Guid Id { get; set; }
        public string Name { get; init; } = string.Empty;
        public string Address { get; init; } = string.Empty;
        public int Capacity { get; init; }
    }

    internal sealed class Handler(AppDbContext context) : IRequestHandler<Command, Result<VenueDto>>
    {
        public async Task<Result<VenueDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            var venue = await context.Venues
                .AsNoTracking()
                .Where(v => v.Id == request.Id && !v.IsDeleted)
                .SingleOrDefaultAsync(cancellationToken);

            if (venue == null)
            {
                return Result.Failure<VenueDto>(new Error("EditVenue.Non-ExistentVenue",
                    "The Specified venue does not exist"));
            }

            venue.Name = request.Name;
            venue.Address = request.Address;
            venue.Capacity = request.Capacity;

            await context.SaveChangesAsync(cancellationToken);

            return venue.Adapt<VenueDto>();
        }
    }

}

public class EditVenueEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("api/venues", async (Guid id, VenueDto venue, ISender sender) =>
        {
            var request = venue.Adapt<EditVenue.Command>();
            var result = await sender.Send(request);
            return result.IsFailure ? Results.NotFound(result.Error) : Results.Ok(result);
        });
    }
}