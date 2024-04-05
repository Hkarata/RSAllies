using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RSAllies.Api.Data;
using RSAllies.Api.HelperTypes;

namespace RSAllies.Api.Features.Users;

public abstract class DeleteUser
{
    public class Query : IRequest<Result<Guid>>
    {
        public Guid Id { get; set; }
    }
    
    internal sealed class Handler(AppDbContext context) : IRequestHandler<Query, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(Query request, CancellationToken cancellationToken)
        {
            var user = await context.Users
                .Where(u => u.Id == request.Id)
                .SingleOrDefaultAsync(cancellationToken);

            if (user == null)
            {
                Result.Failure<Guid>(new Error("GetUser.NonexistentUser", "The specified user does not exist"));
            }

            user!.IsDeleted = true;

            await context.SaveChangesAsync(cancellationToken);

            return user!.Id;
        }
    }
}

public class DeleteUserEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/users/{id:guid}", async (Guid id, ISender sender) =>
        {
            var request = new DeleteUser.Query { Id = id };
            var result = await sender.Send(request);
            return result.IsFailure ? Results.NotFound(result.Error) : Results.Ok(result);
        });
    }
}