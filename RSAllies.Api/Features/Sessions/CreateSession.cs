using Carter;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RSAllies.Api.Contracts;
using RSAllies.Api.Data;
using RSAllies.Api.Entities;
using RSAllies.Api.HelperTypes;

namespace RSAllies.Api.Features.Sessions;

public abstract class CreateSession
{
    public class Command : IRequest<Result<Guid>>
    {
        public Guid Id { get; set; }
        public Guid VenueId { get; set; }
        public DateTime SessionDate { get; set; }
        public int CurrentCapacity { get; set; }
    }

    internal sealed class Handler(AppDbContext context) : IRequestHandler<Command, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            var venue = await context.Venues
                .Where(v => v.Id == request.VenueId && !v.IsDeleted)
                .SingleOrDefaultAsync(cancellationToken);

            if (venue is null)
            {
                return Result.Failure<Guid>(new Error("CreateSession.Non-ExistentVenue",
                    "The specified venue does not exist"));
            }

            var session = new Session
            {
                Id = Guid.NewGuid(),
                SessionDate = request.SessionDate,
                CurrentCapacity = request.CurrentCapacity,
                VenueId = venue.Id,
                Venue = venue,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            context.Add(session);

            await context.SaveChangesAsync(cancellationToken);

            return session.Id;

        }
    }
}

public class CreateSessionEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/session", async (SessionDto session, ISender sender) =>
        {
            var request = session.Adapt<CreateSession.Command>();
            var result = await sender.Send(request);
            return result.IsFailure ? Results.BadRequest(result.Error) : Results.Ok(result);
        });
    }
}