using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RSAllies.Api.Contracts;
using RSAllies.Api.Data;
using RSAllies.Api.HelperTypes;

namespace RSAllies.Api.Features.Sessions;

public abstract class GetSessionDateRange
{
    public class Query : IRequest<Result<List<SessionDto>>>
    {
        public Guid VenueId { get; set; }
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
    }
    
    internal sealed class Handler(AppDbContext context) : IRequestHandler<Query, Result<List<SessionDto>>>
    {
        public async Task<Result<List<SessionDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var sessions = await context.Sessions
                .Where(s => s.VenueId == request.VenueId && s.SessionDate >= request.StartDate && s.SessionDate <= request.EndDate && !s.IsDeleted)
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

            if (sessions.Count == 0)
            {
                return Result.Failure<List<SessionDto>>(new Error("GetSessionDateRange.NoSessions",
                    "The are no sessions in the specified date range"));
            }

            return sessions;
        }
    }
}

public class GetSessionDateRangeEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/venue/{id:guid}/sessions/from/{startDate:datetime}/to/{endDate:datetime}",
            async (Guid id, DateTime startDate, DateTime endDate, ISender sender) =>
            {
                var request = new GetSessionDateRange.Query { VenueId = id    , StartDate = startDate, EndDate = endDate };
                var result = await sender.Send(request);
                return result.IsFailure ? Results.BadRequest(result.Error) : Results.Ok(result);
            });
    }
}