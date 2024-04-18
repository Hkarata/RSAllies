using Carter;
using Mapster;
using MediatR;
using RSAllies.Api.Contracts;
using RSAllies.Api.Data;
using RSAllies.Api.Entities;
using RSAllies.Api.HelperTypes;

namespace RSAllies.Api.Features.Questions;

public abstract class CreateQuestion
{
 
    public class Command : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }
        
        public string QuestionText { get; set; } = string.Empty;
        
        public List<CommandChoices>? ChoicesList { get; set; }
        
    }
    
    public class CommandChoices
    {
        public Guid Id { get; set; }
        public string ChoiceText { get; set; } = string.Empty;
        public bool IsAnswer { get; set; }
    }
    
    internal sealed class Handler(AppDbContext context) : IRequestHandler<Command, Result<bool>>
    {
        public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
        {
            var question = new Question
            {
                Id = Guid.NewGuid(),
                QuestionText = request.QuestionText,
                Choices = request.ChoicesList!.Select(c => new Choice
                {
                    Id = Guid.NewGuid(),
                    ChoiceText = c.ChoiceText,
                    IsCorrect = c.IsAnswer
                }).ToList()
            };
            
            var correctChoice = question.Choices.FirstOrDefault(c => c.IsCorrect);
            if (correctChoice != null)
            {
                var answer = new Answer
                {
                    Id = Guid.NewGuid(),
                    QuestionId = question.Id,
                    ChoiceId = correctChoice.Id
                };

                context.Add(answer);
            }

            context.Questions.Add(question);
            await context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
    
}

public class CreateQuestionEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/question", async (QuestionDto question, ISender sender) =>
        {
            var request = new CreateQuestion.Command
            {
                Id = question.Id,
                QuestionText = question.QuestionText,
                ChoicesList = question.Choices?.Select(c =>new CreateQuestion.CommandChoices
                {
                    Id = c.Id,
                    ChoiceText = c.ChoiceText,
                    IsAnswer = c.IsAnswer
                }).ToList()
            };
            
            var result = await sender.Send(request);
            
            return result.IsFailure ? Results.BadRequest(result.Error) : Results.Ok(result);
        });
    }
}