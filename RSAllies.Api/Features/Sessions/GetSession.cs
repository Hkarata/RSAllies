using Carter;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RSAllies.Api.Contracts;
using RSAllies.Api.Data;
using RSAllies.Api.HelperTypes;

namespace RSAllies.Api.Features.Sessions;

public abstract class GetSession
{
    public class Query : IRequest<Result<SessionDto>>
    {
        public Guid Id { get; set; }
    }
    
    internal sealed class Handler(AppDbContext context) : IRequestHandler<Query, Result<SessionDto>>
    {
        public async Task<Result<SessionDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var session = await context.Sessions
                .AsNoTracking()
                .Where(s => s.Id == request.Id && !s.IsDeleted)
                .SingleOrDefaultAsync(cancellationToken);

            if (session is null)
            {
                return Result.Failure<SessionDto>(new Error("GetSession.Non-ExistentSession",
                    "The specified session does not exist"));
            }

            return new SessionDto{Id = session.Id, VenueId = session.VenueId, SessionDate = session.SessionDate, CurrentCapacity = session.CurrentCapacity};
        }
    }
}

public class GetSessionEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/session/{id:guid}", async (Guid id, ISender sender) =>
        {
            var request = new GetSession.Query { Id = id };
            var result = await sender.Send(request);
            return result.IsFailure ? Results.BadRequest(result.Error) : Results.Ok(result);
        });
    }
}