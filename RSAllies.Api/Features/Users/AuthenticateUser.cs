using Carter;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RSAllies.Api.Contracts;
using RSAllies.Api.Data;
using RSAllies.Api.HelperTypes;

namespace RSAllies.Api.Features.Users;

public abstract class AuthenticateUser
{
    public class Command : IRequest<Result<UserDTO>>
    {
        public string Phone { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    internal sealed class Handler(AppDbContext context) : IRequestHandler<Command, Result<UserDTO>>
    {
        public async Task<Result<UserDTO>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await context.Users
                .AsNoTracking()
                .Where(u => u.Phone == request.Phone && u.Password == request.Password)
                .Select(u => new UserDTO
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Phone = u.Phone
                })
                .SingleOrDefaultAsync(cancellationToken);

            if (user is null)
            {
                return Result.Failure<UserDTO>(new Error("AuthenticateUser.Failed",
                    "No user exist with the specified credentials"));
            }

            return user;
        }
    }
}

public class AuthenticateUserEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/user/authenticate", async (AuthenticateDto user, ISender sender) =>
        {
            var request = user.Adapt<AuthenticateUser.Command>();
            var result = await sender.Send(request);
            return result.IsFailure ? Results.Ok(result.Error) : Results.Ok(result);
        });
    }
}