using Carter;
using Mapster;
using MediatR;
using RSAllies.Api.Contracts;
using RSAllies.Api.Data;
using RSAllies.Api.Entities;
using RSAllies.Api.HelperTypes;

namespace RSAllies.Api.Features.Venues;

public abstract class CreateVenue
{
    public class Command : IRequest<Result<Guid>>
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int Capacity { get; set; }
    }

    internal sealed class Handler(AppDbContext context) : IRequestHandler<Command, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            var venue = new Venue
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Address = request.Address,
                Capacity = request.Capacity,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            context.Venues.Add(venue);

            await context.SaveChangesAsync(cancellationToken);

            return venue.Id;
        }
    }

}

public class CreateVenueEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/venues", async (CreateVenueDto venue, ISender sender) =>
        {
            var request = venue.Adapt<CreateVenue.Command>();
            var result = await sender.Send(request);
            return result.IsFailure ? Results.BadRequest(result) : Results.Ok(result);
        });
    }
}