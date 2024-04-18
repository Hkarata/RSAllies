using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RSAllies.Api.Contracts;
using RSAllies.Api.Data;
using RSAllies.Api.HelperTypes;

namespace RSAllies.Api.Features.Bookings;

public abstract class GetUserBookings
{
    public class Query : IRequest<Result<List<BookingDto>>>
    {
        public Guid Id { get; init; }
    }

    internal sealed class Handler(AppDbContext context) : IRequestHandler<Query, Result<List<BookingDto>>>
    {
        public async Task<Result<List<BookingDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var bookings = await context.Bookings
                .AsNoTracking()
                .Where(b => b.UserId == request.Id && !b.IsDeleted)
                .Include(b => b.Session)
                .ThenInclude(b => b.Venue)
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    VenueName = b.Session.Venue.Name,
                    VenueAddress = b.Session.Venue.Address,
                    SessionDate = b.Session.SessionDate
                })
                .ToListAsync(cancellationToken);

            if (bookings.Count == 0)
            {
                return Result.Failure<List<BookingDto>>(new Error("GetUserBookings.NoBookings",
                    "The specified user has no bookings"));
            }

            return bookings;
        }
    }
}

public class GetUserBookingEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/user/{id:guid}/bookings", async (Guid id, ISender sender) =>
        {
            var request = new GetUserBookings.Query { Id = id };
            var result = await sender.Send(request);
            return result.IsFailure ? Results.NotFound(result.Error) : Results.Ok(result);
        });
    }
}