using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RSAllies.Api.Contracts;
using RSAllies.Api.Data;
using RSAllies.Api.HelperTypes;

namespace RSAllies.Api.Features.Questions;

public abstract class GetQuestions
{

    public class Query : IRequest<Result<List<QuestionDto>>>
    {

    }

    internal sealed class Handler(AppDbContext context) : IRequestHandler<Query, Result<List<QuestionDto>>>
    {
        public async Task<Result<List<QuestionDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var questions = await context.Questions
                .AsNoTracking()
                .Where(q => q.IsEnglish)
                .Include(q => q.Choices)
                .Select(q => new QuestionDto
                {
                    Id = q.Id,
                    QuestionText = q.QuestionText,
                    Choices = q.Choices.Select(c => new ChoiceDto
                    {
                        Id = c.Id,
                        ChoiceText = c.ChoiceText,
                        IsAnswer = false
                    }).ToList(),
                    IsEnglish = true
                }).ToListAsync(cancellationToken);

            if (questions.Count == 0)
            {
                return Result.Failure<List<QuestionDto>>(new Error("GetQuestions.EnglishQuestions",
                    "There are no questions"));
            }

            return questions;
        }
    }

}

public class GetQuestionEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/questions", async (ISender sender) =>
        {
            var request = new GetQuestions.Query();
            var result = await sender.Send(request);
            return result.IsFailure ? Results.NotFound(result.Error) : Results.Ok(result);
        });
    }
}