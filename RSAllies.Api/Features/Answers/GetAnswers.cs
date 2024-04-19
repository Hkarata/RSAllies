using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RSAllies.Api.Contracts;
using RSAllies.Api.Data;
using RSAllies.Api.HelperTypes;

namespace RSAllies.Api.Features.Answers;

public abstract class GetAnswers
{
    public class Query : IRequest<Result<List<AnswerDto>>>
    {
        
    }
    
    internal sealed class Handler(AppDbContext context) : IRequestHandler<Query, Result<List<AnswerDto>>>
    {
        public async Task<Result<List<AnswerDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var answers = await context.Answers
                .AsNoTracking()
                .Select(a => new AnswerDto
                {
                    QuestionId = a.QuestionId,
                    ChoiceId = a.ChoiceId
                }).ToListAsync(cancellationToken);

            return answers.Count != 0 ? answers : Result.Failure<List<AnswerDto>>(new Error("GetAnswers", "There are no answers"));
        }
    }
    
}

public class GetAnswersEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/answers", async (ISender sender) =>
        {
            var request = new GetAnswers.Query();
            var result = await sender.Send(request);
            return result.IsFailure ? Results.NotFound(result.Error) : Results.Ok(result);
        });
    }
}