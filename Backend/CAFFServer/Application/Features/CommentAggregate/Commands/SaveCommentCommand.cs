using Application.Interfaces;
using Application.Services;
using Dal;
using Domain.Entities.CommentAggregate;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.CommentAggregate.Commands
{
    public class SaveCommentCommand : IRequest
    {
        public Guid ProductId { get; set; }
        public string Content { get; set; }
    }

    public class SaveCommentCommandHandler : IRequestHandler<SaveCommentCommand, Unit>
    {
        private readonly WebshopDbContext dbContext;
        private readonly IIdentityService identityService;
        private readonly ILogger<SaveCommentCommandHandler> logger;

        public SaveCommentCommandHandler(WebshopDbContext dbContext, IIdentityService identityService, ILogger<SaveCommentCommandHandler> logger)
        {
            this.dbContext = dbContext;
            this.identityService = identityService;
            this.logger = logger;
        }
        public async Task<Unit> Handle(SaveCommentCommand request, CancellationToken cancellationToken)
        {
            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                Content = request.Content,
                CommenterId = identityService.GetCurrentUserId(),
                ProductId = request.ProductId
            };

            dbContext.Add(comment);

            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation($"Komment létrehozása: Felahasználó: {identityService.GetCurrentUserId()}, Komment: {comment.Id + " " + comment.Content}");

            return Unit.Value;
        }
    }

    public class SaveCommentCommandValidator : AbstractValidator<SaveCommentCommand>
    {
        public SaveCommentCommandValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty();

            RuleFor(x => x.Content)
                .NotEmpty();
        }
    }
}

