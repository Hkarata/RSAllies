using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RSAllies.Api.Data;
using RSAllies.Api.HelperTypes;

namespace RSAllies.Api.Features.Bookings;

public abstract class DeleteBooking
{
    public class Command : IRequest<Result<Guid>>
    {
        public Guid Id { get; set; }
    }
    
    internal sealed class Handler(AppDbContext context) : IRequestHandler<Command, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            var booking = await context.Bookings
                .Where(b => b.Id == request.Id)
                .SingleOrDefaultAsync(cancellationToken);

            if (booking == null)
                return Result.Failure<Guid>(new Error("DeleteBooking.NonExistentBooking",
                    "The specified booking does not exist"));

            booking.IsDeleted = true;

            await context.SaveChangesAsync(cancellationToken);

            return booking.Id;
        }
    }
}

public class DeleteBookingEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/booking/{id:guid}", async (Guid id, ISender sender) =>
        {
            var request = new DeleteBooking.Command { Id = id };
            var result = await sender.Send(request);
            return result.IsFailure ? Results.NotFound(result.Error) : Results.Ok(result);
        });
    }
}