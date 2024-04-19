using Carter;
using Mapster;
using MediatR;
using RSAllies.Api.Contracts;
using RSAllies.Api.Data;
using RSAllies.Api.HelperTypes;

namespace RSAllies.Api.Features.Scores;

public abstract class CreateUserScore
{
    
    public class Command : IRequest<Result<bool>>
    {
        public Guid UserId { get; set; }
        public int ScoreValue { get; set; }
    }
    
    
    internal sealed class Handler(AppDbContext context) : IRequestHandler<Command, Result<bool>>
    {
        public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
        {
            var userScore = new Entities.Score
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                ScoreValue = request.ScoreValue
            };

            context.Scores.Add(userScore);

            await context.SaveChangesAsync(cancellationToken);

            return true;

        }
    }
    
    
}


public class CreateUserScoreEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/score", async (ScoreDto score, ISender sender) =>
        {
            var request = score.Adapt<CreateUserScore.Command>();
            var result = await sender.Send(request);
            return Results.Ok(result);
        });
    }
}