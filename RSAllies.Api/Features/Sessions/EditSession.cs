using Carter;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RSAllies.Api.Contracts;
using RSAllies.Api.Data;
using RSAllies.Api.HelperTypes;

namespace RSAllies.Api.Features.Sessions;

public abstract class EditSession
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
            var session = await context.Sessions
                .Where(s => s.Id == request.Id && !s.IsDeleted)
                .SingleOrDefaultAsync(cancellationToken);

            if (session is null)
            {
                return Result.Failure<Guid>(new Error("EditSession.Non-ExistentSession", "The specified session does not exist"));
            }

            var venue = await context.Venues
                .Where(v => v.Id == request.VenueId & !v.IsDeleted)
                .SingleOrDefaultAsync(cancellationToken);

            if (venue is null)
            {
                return Result.Failure<Guid>(new Error("EditSession.Non-ExistentVenue",
                    "The specified venue does not exit"));
            }

            session.Venue = venue;
            session.VenueId = venue.Id;
            session.SessionDate = request.SessionDate;
            session.CurrentCapacity = request.CurrentCapacity;

            await context.SaveChangesAsync(cancellationToken);

            return session.Id;
        }
    }
}

public class EditSessionEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("api/sessions", async (SessionDto session, ISender sender) =>
        {
            var request = session.Adapt<EditSession.Command>();
            var result = await sender.Send(request);
            return result.IsFailure ? Results.BadRequest(result.Error) : Results.Ok(result);
        });
    }
}