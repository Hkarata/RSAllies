using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RSAllies.Api.Contracts;
using RSAllies.Api.Data;
using RSAllies.Api.HelperTypes;

namespace RSAllies.Api.Features.Users;

public abstract class GetUsers
{
    public class Query : IRequest<Result<List<UserDTO>>>
    {
        
    }
    
    internal sealed class Handler(AppDbContext context) : IRequestHandler<Query, Result<List<UserDTO>>>
    {
        public async Task<Result<List<UserDTO>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var users = await context.Users
                .AsNoTracking()
                .Where(u => !u.IsDeleted)
                .Select(u => new UserDTO
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Phone = u.Phone
                })
                .ToListAsync(cancellationToken);

            return users.Count is 0 ? Result.Failure<List<UserDTO>>(new Error("GetUsers.NoUsers", "There are no Users")) : users;
        }
    }
}

public class GetUsersEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/users", async (ISender sender) =>
        {
            var request = new GetUsers.Query();
            var result = await sender.Send(request);
            return result.IsFailure ? Results.NotFound(result.Error) : Results.Ok(result);
        });
    }
}