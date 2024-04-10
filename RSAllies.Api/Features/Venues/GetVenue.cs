using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RSAllies.Api.Contracts;
using RSAllies.Api.Data;
using RSAllies.Api.HelperTypes;

namespace RSAllies.Api.Features.Venues;

public abstract class GetVenue
{
    public class Query : IRequest<Result<VenueDto>>
    {
        public Guid Id { get; init; }
    }

    internal sealed class Handler(AppDbContext context) : IRequestHandler<Query, Result<VenueDto>>
    {
        public async Task<Result<VenueDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var venue = await context.Venues
                .AsNoTracking()
                .Where(v => v.Id == request.Id && !v.IsDeleted)
                .Select(v => new VenueDto
                {
                    Id = v.Id,
                    Name = v.Name,
                    Address = v.Address,
                    Capacity = v.Capacity
                })
                .SingleOrDefaultAsync(cancellationToken);

            if (venue == null)
            {
                return Result.Failure<VenueDto>(
                    new Error("GetVenue.Non-existentVenue", "The specified venue does not exist"));
            }

            return venue;
        }
    }
}

public class GetVenueEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/venue/{id:guid}", async (Guid id, ISender sender) =>
        {
            var request = new GetVenue.Query { Id = id };
            var result = await sender.Send(request);
            return result.IsFailure ? Results.NotFound(result.Error) : Results.Ok(result);
        });
    }
}