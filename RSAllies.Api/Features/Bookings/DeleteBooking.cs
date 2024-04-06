using MediatR;
using RSAllies.Api.Data;
using RSAllies.Api.HelperTypes;

namespace RSAllies.Api.Features.Bookings;

public abstract class DeleteUserBooking
{
    public class Command : IRequest<Result<Guid>>
    {
        public Guid Id { get; set; }
    }
    
    internal sealed class Handler(AppDbContext context) : IRequestHandler<Command, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            var booking  = await context.Bookings
                .Where(b => b.UserId == request.Id)
                .S
        }
    }
}