using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RSAllies.Api.Data;
using RSAllies.Api.HelperTypes;

namespace RSAllies.Api.Features.Sessions;

public abstract class DeleteSession
{
    public class Command : IRequest<Result<Guid>>
    {
        public Guid Id { get; set; }
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
                return Result.Failure<Guid>(new Error("DeleteSession.NonExistentSession", "The specified session does not exit"));
            }

            session.IsDeleted = true;

            await context.SaveChangesAsync(cancellationToken);

            return session.Id;
        }
    }
}

public class DeleteSessionEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/sessions/{id:guid}", async (Guid id, ISender sender) =>
        {
            var request = new DeleteSession.Command { Id = id };
            var result = await sender.Send(request);
            return result.IsFailure ? Results.BadRequest(result.Error) : Results.Ok(result);
        });
    }
}