using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RSAllies.Api.Contracts;
using RSAllies.Api.Data;
using RSAllies.Api.HelperTypes;

namespace RSAllies.Api.Features.Venues;

public abstract class GetVenueSessions
{
    public class Query : IRequest<Result<List<SessionDto>>>
    {
        public Guid Id { get; init; }
    }
    
    internal sealed class Handler(AppDbContext context) : IRequestHandler<Query, Result<List<SessionDto>>>
    {
        public async Task<Result<List<SessionDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var sessions = await context.Sessions
                .AsNoTracking()
                .Where(s => s.VenueId == request.Id)
                .Select(s => new SessionDto
                {
                    Id = s.Id,
                    VenueId = s.VenueId,
                    SessionDate = s.SessionDate,
                    CurrentCapacity = s.CurrentCapacity
                })
                .ToListAsync(cancellationToken);

            if (sessions.Count == 0)
            {
                return Result.Failure<List<SessionDto>>(new Error("GetVenueSessions",
                    "There are no sessions for the specified venue"));
            }

            return sessions;
        }
    }
}

public class GetVenueSessionsEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/venue/{id:guid}/sessions", async (Guid id, ISender sender) =>
        {
            var request = new GetVenueSessions.Query { Id = id };
            var result = await sender.Send(request);
            return result.IsFailure ? Results.NotFound(result.Error) : Results.Ok(result);
        });
    }
}