using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RSAllies.Api.Contracts;
using RSAllies.Api.Data;
using RSAllies.Api.HelperTypes;

namespace RSAllies.Api.Features.Bookings;

public abstract class GetUserBooking
{
    public class Query : IRequest<Result<BookingDto>>
    {
        public Guid Id { get; init; }
    }
    
    internal sealed class Handler(AppDbContext context) : IRequestHandler<Query, Result<BookingDto>>
    {
        public async Task<Result<BookingDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var booking = await context.Bookings
                .AsNoTracking()
                .Where(b => b.UserId == request.Id)
                .Include(b => b.Session)
                .SingleOrDefaultAsync(cancellationToken);

            var venue = await context.Venues
                .AsNoTracking()
                .Where(v => booking != null && v.Id == booking.Session.VenueId)
                .SingleOrDefaultAsync(cancellationToken);

            if (booking == null)
                return Result.Failure<BookingDto>(new Error("GetUserBooking.Non-ExistentBooking",
                    "The specified booking does not exist"));
            
            var userBooking = new BookingDto
            {
                Id = booking.Id,
                VenueName = venue?.Name,
                VenueAddress = venue?.Address,
                SessionDate = booking.Session.SessionDate
            };

            return userBooking;

        }
    }
}

public class GetUserBookingEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/user/{id:guid}/booking", async (Guid id, ISender sender) =>
        {
            var request = new GetUserBooking.Query { Id = id };
            var result = await sender.Send(request);
            return result.IsFailure ? Results.NotFound(result.Error) : Results.Ok(result);
        });
    }
}