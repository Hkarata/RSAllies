using Carter;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RSAllies.Api.Contracts;
using RSAllies.Api.Data;
using RSAllies.Api.Entities;
using RSAllies.Api.HelperTypes;

namespace RSAllies.Api.Features.Users
{
    public static class CreateUser
    {
        public class Command : IRequest<Result<UserDTO>>
        {
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string? Email { get; set; } = string.Empty;
            public string Phone { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        internal sealed class Handler(AppDbContext _context) : IRequestHandler<Command, Result<UserDTO>>
        {
            public async Task<Result<UserDTO>> Handle(Command request, CancellationToken cancellationToken)
            {
                var existingUserWithSameEmailOrPhone = _context.Users
                    .AsNoTracking()
                    .Any(u => u.Email == request.Email || u.Phone == request.Phone);

                if (existingUserWithSameEmailOrPhone)
                    return Result.Failure<UserDTO>(new Error("CreateUser.ExistentUser", "The specified user already exists"));

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    Password = request.Password,
                    Phone = request.Phone,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);

                await _context.SaveChangesAsync(cancellationToken);


                return user.Adapt<UserDTO>();
            }
        }

    }

    public class CreateUserEndPoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/users", async (UserDTO request, ISender sender) =>
            {
                var command = request.Adapt<CreateUser.Command>();

                var result = await sender.Send(command);

                if (result.IsFailure)
                {
                    return Results.BadRequest(result.Error);
                }

                return Results.Ok(result.Value);

            });
        }
    }
}
