using Carter;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RSAllies.Api.Contracts;
using RSAllies.Api.Data;
using RSAllies.Api.HelperTypes;

namespace RSAllies.Api.Features.Users;

public abstract class CheckUser
{
    public class Query : IRequest<Result<Guid>>
    {
        public Guid Id { get; init; }
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string? Email { get; init; } = string.Empty;
        public string Phone { get; init; } = string.Empty;
    }

    internal sealed class Handler(AppDbContext context) : IRequestHandler<Query, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(Query request, CancellationToken cancellationToken)
        {
            var user = await context.Users
                .AsNoTracking()
                .Where(u => u.FirstName == request.FirstName &&
                            u.LastName == request.LastName &&
                            u.Phone == request.Phone)
                .SingleOrDefaultAsync(cancellationToken);


            if (user is null)
                return Result.Failure<Guid>(new Error("CheckUser.NonExistentUser",
                    "No user exist with the specified data"));

            return user.Id;
        }
    }
}

public class CheckUserEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/user/check-user", async (UserDTO user, ISender sender) =>
        {
            var request = user.Adapt<CheckUser.Query>();
            var result = await sender.Send(request);
            return result.IsFailure ? Results.BadRequest(result.Error) : Results.Ok(result);
        });
    }
}