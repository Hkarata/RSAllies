using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RSAllies.Api.Contracts;
using RSAllies.Api.Data;
using RSAllies.Api.HelperTypes;

namespace RSAllies.Api.Features.Users
{
    public static class GetUser
    {

        public class Query : IRequest<Result<UserDTO>>
        {
            public Guid Id { get; set; }
        }

        internal sealed class Handler(AppDbContext _context) : IRequestHandler<Query, Result<UserDTO>>
        {
            public async Task<Result<UserDTO>> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await _context.Users
                    .AsNoTracking()
                    .Where(u => u.Id == request.Id)
                    .Select(u => new UserDTO { Id = u.Id, FirstName = u.FirstName, LastName = u.LastName, Email = u.Email, Phone = u.Phone})
                    .SingleOrDefaultAsync(cancellationToken);

                if (user == null)
                    return Result.Failure<UserDTO>(new Error("GetUser.NonexistentUser", "The specified user does not exist"));

                return user;
            }
        }
    }

    public class GetUserEndPoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/users/{id}", async (Guid Id, ISender sender) =>
            {
                var result = await sender.Send(new GetUser.Query { Id = Id });

                if (result.IsFailure)
                    return Results.NotFound(result.Error);

                return Results.Ok(result.Value);

            });
        }
    }
}
