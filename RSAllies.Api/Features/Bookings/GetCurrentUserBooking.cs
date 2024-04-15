using MediatR;
using Microsoft.EntityFrameworkCore;
using RSAllies.Api.Contracts;
using RSAllies.Api.Data;
using RSAllies.Api.HelperTypes;

namespace RSAllies.Api.Features.Bookings;

public abstract class GetCurrentBooking
{
    public class Query: IRequest<Result<BookingDto>>
    {
        public Guid Id { get; set; }
    }

    internal sealed class Handler(AppDbContext context) : IRequestHandler<Query, Result<BookingDto>>
    {
        public async Task<Result<BookingDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var booking = await context.Bookings
                .AsNoTracking()
                .Where(b => b.UserId == request.Id && !b.IsDeleted)
                .Include(b => b.Session)
                .ThenInclude(s => s.Venue)
                .OrderByDescending(b => b.BookingDate)
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    VenueName = b.Session.Venue.Name,
                    VenueAddress = b.Session.Venue.Address,
                    SessionDate = b.Session.SessionDate
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (booking is null)
            {
                return Result.Failure<BookingDto>(new Error("GetCurrentBooking.NoBooking",
                    "The specified user has no current booking"));
            }
            
            return booking;
        }
    }
}