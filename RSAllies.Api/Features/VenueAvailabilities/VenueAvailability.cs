using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RSAllies.Api.Contracts;
using RSAllies.Api.Data;
using RSAllies.Api.HelperTypes;

namespace RSAllies.Api.Features.VenueAvailabilities;

public abstract class VenueAvailability
{
    public class Query : IRequest<Result<List<DateTime>>>
    {
        public Guid Id { get; set; }
    }
    
    internal sealed class Handler(AppDbContext context) : IRequestHandler<Query, Result<List<DateTime>>>
    {
        public async Task<Result<List<DateTime>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var venue = await context.Venues
                .AsNoTracking()
                .Where(v => v.Id == request.Id && !v.IsDeleted)
                .Include(v => v.venueAvailabilities)
                .SingleOrDefaultAsync(cancellationToken);

            var dates = venue?.venueAvailabilities?.Select(av => av.AvailableDate).ToList();

            return dates?.Count == 0
                ? Result.Failure<List<DateTime>>(new Error("VenueAvailability.NotAvailable",
                    "The specified venue is not available"))
                : dates;
        }
    }
}

public class VenueAvailabilityEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/venue/{id:guid}/availability", async (Guid id, ISender sender) =>
        {
            var request = new VenueAvailability.Query { Id = id };
            var result = await sender.Send(request);
            return result.IsFailure ? Results.BadRequest(result.Error) : Results.Ok(result);
        });
    }
}