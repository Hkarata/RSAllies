using Carter;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RSAllies.Api.Contracts;
using RSAllies.Api.Data;
using RSAllies.Api.Entities;
using RSAllies.Api.HelperTypes;

public abstract class CreateVenueSessions
{
    public class Command : IRequest<Result<bool>>
    {
        public List<SessionRequest>? Sessions { get; set; }

        public class SessionRequest
        {
            public Guid VenueId { get; set; }
            public DateTime SessionDate { get; set; }
            public int CurrentCapacity { get; set; }
        }
    }

    internal sealed class Handler : IRequestHandler<Command, Result<bool>>
    {
        private readonly AppDbContext _context;

        public Handler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
        {
            var venueIds = request.Sessions!.Select(s => s.VenueId)
                                           .Distinct();

            var venues = await _context.Venues
                .Where(v => venueIds.Contains(v.Id) && !v.IsDeleted)
                .ToDictionaryAsync(v => v.Id, cancellationToken);

            var sessions = new List<Session>();

            foreach (var sessionRequest in request.Sessions!)
            {
                if (!venues.TryGetValue(sessionRequest.VenueId, out var venue))
                {
                    return Result.Failure<bool>(new Error("CreateSession.Non-ExistentVenue",
                        "The specified venue does not exist"));
                }

                var session = new Session
                {
                    Id = Guid.NewGuid(),
                    SessionDate = sessionRequest.SessionDate,
                    CurrentCapacity = sessionRequest.CurrentCapacity,
                    VenueId = venue.Id,
                    Venue = venue,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                sessions.Add(session);
            }

            _context.Sessions.AddRange(sessions);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success(true);
        }
    }
}

public class CreateVenueSessionsEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/sessions", async (List<SessionDto> sessions, ISender sender) =>
        {
            if (sessions == null || !sessions.Any())
            {
                return Results.BadRequest("No sessions provided.");
            }

            var request = new CreateVenueSessions.Command
            {
                Sessions = sessions.Adapt<List<CreateVenueSessions.Command.SessionRequest>>()
            };

            var result = await sender.Send(request);

            return result.IsFailure ? Results.BadRequest(result.Error) : Results.Ok(result.Value);
        });
    }
}
