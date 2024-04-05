using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RSAllies.Api.Contracts;
using RSAllies.Api.Data;
using RSAllies.Api.HelperTypes;

namespace RSAllies.Api.Features.Venues;

public abstract class GetVenues
{
    public class Query : IRequest<Result<List<VenueDto>>>
    {
        
    }
    
    internal sealed class Handler(AppDbContext context) : IRequestHandler<Query, Result<List<VenueDto>>>
    {
        public async Task<Result<List<VenueDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var venues = await context.Venues
                .AsNoTracking()
                .Where(v => !v.IsDeleted)
                .Select(v => new VenueDto
                {
                    Id = v.Id,
                    Name = v.Name,
                    Address = v.Address,
                    Capacity = v.Capacity
                })
                .ToListAsync(cancellationToken);
            
            return venues.Count == 0 ? Result.Failure<List<VenueDto>>(new Error("GetVenues.NoVenues", "There are no venues")) : venues;
        }
    }
}

public class GetVenuesEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/venues", async (ISender sender) =>
        {
            var request = new GetVenues.Query();
            var result = await sender.Send(request);
            return result.IsFailure ? Results.NotFound(result.Error) : Results.Ok(result);
        });
    }
}