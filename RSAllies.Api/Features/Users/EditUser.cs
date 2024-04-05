using Carter;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RSAllies.Api.Contracts;
using RSAllies.Api.Data;
using RSAllies.Api.HelperTypes;

namespace RSAllies.Api.Features.Users;

public abstract class EditUser
{
    public class Command : IRequest<Result<Guid>>
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }

    internal sealed class Handler(AppDbContext context) : IRequestHandler<Command, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await context.Users
                .Where(u => u.Id == request.Id && !u.IsDeleted)
                .SingleOrDefaultAsync(cancellationToken);

            if (user is null)
            {
                Result.Failure<Guid>(new Error("GetUser.NonexistentUser", "The specified user does not exist"));
            }
            else
            {
                user.FirstName = request.FirstName;
                user.LastName = request.LastName;
                user.Email = request.Email;
                user.Phone = request.Phone;
                
                await context.SaveChangesAsync(cancellationToken);
            }
            
            return user!.Id;
        }
    }
}

public class EditUserEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("api/users", async (Guid id, UserDTO user, ISender sender) =>
        {
            var request = user.Adapt<EditUser.Command>();
            var result = await sender.Send(request);
            return result.IsFailure ? Results.NotFound(result.Error) : Results.Ok(result);
        });
    }
}