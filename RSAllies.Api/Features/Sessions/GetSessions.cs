using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RSAllies.Api.Contracts;
using RSAllies.Api.Data;
using RSAllies.Api.HelperTypes;

namespace RSAllies.Api.Features.Sessions;

public abstract class GetSessions
{
    public class Query : IRequest<Result<List<SessionDto>>>
    {

    }

    internal sealed class Handler(AppDbContext context) : IRequestHandler<Query, Result<List<SessionDto>>>
    {
        public async Task<Result<List<SessionDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var sessions = await context.Sessions
                .AsNoTracking()
                .Where(s => !s.IsDeleted)
                .Include(s => s.Venue)
                .Select(s => new SessionDto
                {
                    Id = s.Id,
                    VenueId = s.VenueId,
                    VenueName = s.Venue.Name,
                    SessionDate = s.SessionDate,
                    CurrentCapacity = s.CurrentCapacity
                })
                .ToListAsync(cancellationToken);

            return sessions.Count == 0 ? Result.Failure<List<SessionDto>>(new Error("GetSessions.NoSessions", "There are no sessions")) : sessions;
        }
    }
}

public class GetSessionsEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/sessions", async (ISender sender) =>
        {
            var request = new GetSessions.Query();
            var result = await sender.Send(request);
            return result.IsFailure ? Results.NotFound(result.Error) : Results.Ok(result);
        });
    }
}