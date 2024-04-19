using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RSAllies.Api.Contracts;
using RSAllies.Api.Data;
using RSAllies.Api.HelperTypes;

namespace RSAllies.Api.Features.Scores
{
    public abstract class GetUserScore
    {
        public class Query : IRequest<Result<ScoreDto>>
        {
            public Guid Id { get; set; }
        }


        internal sealed class Handler(AppDbContext context) : IRequestHandler<GetUserScore.Query, Result<ScoreDto>>
        {
            public async Task<Result<ScoreDto>> Handle(GetUserScore.Query request, CancellationToken cancellationToken)
            {
                var score = await context.Scores
                    .AsNoTracking()
                    .Where(s => s.UserId == request.Id)
                    .OrderByDescending(s => s.CreatedAt)
                    .Select(s => new ScoreDto
                    {
                        UserId = s.UserId,
                        ScoreValue = s.ScoreValue,
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                if (score is null)
                {
                    return Result.Failure<ScoreDto>(new Error("GetUserScore.NoScore", "The specified user has no scores"));
                }

                return score;
            }
        }
    }


    public class GetUserScoreEndPoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/score/{id:guid}", async (Guid id, ISender sender) =>
            {
                var request = new GetUserScore.Query { Id = id };
                var result = await sender.Send(request);
                return result.IsFailure ? Results.NotFound(result.Error) : Results.Ok(result);
            });
        }
    }

}
