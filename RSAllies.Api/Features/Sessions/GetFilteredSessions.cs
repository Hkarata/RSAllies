using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RSAllies.Api.Contracts;
using RSAllies.Api.Data;
using RSAllies.Api.HelperTypes;

namespace RSAllies.Api.Features.Sessions;

public abstract class GetFilteredSessions
{
    public class Query : IRequest<Result<List<FilteredSessionDto>>>
    {
        public string Address { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }
    
    internal sealed class Handler(AppDbContext context) : IRequestHandler<Query, Result<List<FilteredSessionDto>>>
    {
        public async Task<Result<List<FilteredSessionDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var sessions = await context.Sessions
                .AsNoTracking()
                .Include(s => s.Venue)
                .Where(s => s.Venue.Address == request.Address && s.SessionDate >= request.Date)
                .Select(s => new FilteredSessionDto
                {
                    Id = s.Id,
                    VenueId = s.Venue.Id,
                    VenueName = s.Venue.Name,
                    SessionDate = s.SessionDate,
                    VenueCapacity = s.Venue.Capacity,
                    CurrentCapacity = s.CurrentCapacity,
                    IsFull = s.CurrentCapacity >= s.Venue.Capacity
                }).ToListAsync(cancellationToken);

            return sessions.Count == 0 ? Result.Failure<List<FilteredSessionDto>>(new Error("GetFilteredSessions", "No sessions available")) : sessions;
        }
    }
}

public class GetFilteredSessionsEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/sessions/filter/region/{region}/date{date:datetime}",
            async (string region, DateTime date, ISender sender) =>
            {
                var request = new GetFilteredSessions.Query { Address = region, Date = date };
                var result = await sender.Send(request);
                return result.IsFailure ? Results.Ok(result.Error) : Results.Ok(result);
            });
    }
}