using Carter;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RSAllies.Api.Contracts;
using RSAllies.Api.Data;
using RSAllies.Api.Entities;
using RSAllies.Api.HelperTypes;

namespace RSAllies.Api.Features.Bookings;

public abstract class CreateBooking
{
    public class Command : IRequest<Result<Guid>>
    {
        public Guid UserId { get; set; }
        public Guid SessionId { get; set; }
        public DateTime BookingDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
    
    internal sealed class Handler(AppDbContext context) : IRequestHandler<Command, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            var session = await context.Sessions
                .Where(s => s.Id == request.SessionId)
                .Include(s => s.Venue)
                .SingleOrDefaultAsync(cancellationToken);

            if (session?.CurrentCapacity >= session?.Venue.Capacity)
            {
                return Result.Failure<Guid>(new Error("CreateBooking.SessionFull", "The Selected session is full"));
            }
            
            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                SessionId = request.SessionId,
                BookingDate = request.BookingDate,
                Status = request.Status,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            if (session != null) session.CurrentCapacity++;

            context.Bookings.Add(booking);

            await context.SaveChangesAsync(cancellationToken);

            return booking.Id;
        }
    }
}

public class CreateBookingEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/bookings", async (CreateBookingDto booking, ISender sender) =>
        {
            var request = booking.Adapt<CreateBooking.Command>();
            var result = await sender.Send(request);
            return result.IsFailure ? Results.BadRequest(result.Error) : Results.Ok(result);
        });
    }
}