using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RSAllies.Api.Data;
using RSAllies.Api.HelperTypes;

namespace RSAllies.Api.Features.Venues;

public abstract class DeleteVenue
{
    public class Command : IRequest<Result<Guid>>
    {
        public Guid Id { get; set; }
    }

    internal sealed class Handler(AppDbContext context) : IRequestHandler<Command, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            var venue = await context.Venues
                .Where(v => v.Id == request.Id)
                .SingleOrDefaultAsync(cancellationToken);

            if (venue == null)
                return Result.Failure<Guid>(
                    new Error("GetVenue.Non-existentVenue", "The specified venue does not exist"));

            if (venue.IsDeleted)
            {
                return Result.Failure<Guid>(
                    new Error("GetVenue.DeletedVenue", "The specified venue has been deleted already"));
            }

            venue.IsDeleted = true;

            await context.SaveChangesAsync(cancellationToken);

            return venue.Id;
        }
    }
}

public class DeleteVenueEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/venues/{id:guid}", async (Guid id, ISender sender) =>
        {
            var request = new DeleteVenue.Command { Id = id };
            var result = await sender.Send(request);
            return result.IsFailure ? Results.NotFound(result.Error) : Results.Ok(result);
        });
    }
}