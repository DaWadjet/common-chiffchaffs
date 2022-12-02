using Application.Interfaces;
using Application.Services;
using CSONGE.Application.Exceptions;
using Domain.Entities.CommentAggregate;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.CommentAggregate.Commands
{
    public class UpdateCommentCommand : IRequest
    {
        public Guid CommentId { get; set; }
        public string Content { get; set; }
    }

    public class UpdateCommentCommandHandler : IRequestHandler<UpdateCommentCommand, Unit>
    {
        private readonly ICommentRepository commentRepository;
        private readonly IIdentityService identityService;
        private readonly ILogger<UpdateCommentCommandHandler> logger;

        public UpdateCommentCommandHandler(ICommentRepository commentRepository, IIdentityService identityService, ILogger<UpdateCommentCommandHandler> logger)
        {
            this.commentRepository = commentRepository;
            this.identityService = identityService;
            this.logger = logger;
        }
        public async Task<Unit> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
        {
            var comment = await commentRepository.SingleAsync(x => x.Id == request.CommentId);
            
            logger.LogInformation($"Komment módosítás kezdés: Felahasználó: {identityService.GetCurrentUserId()}, Komment: {comment.Id + " " + comment.Content}");

            if (comment == null || comment.CommenterId != identityService.GetCurrentUserId() && !(await identityService.GetCurrentUser()).IsAdmin)
            {
                throw new ApplicationExeption("Csak a saját kommentek módosíthatók!");
            }
            var regiContent = comment.Content;

            comment.Update(request.Content);

            await commentRepository.UpdateAsync(comment);

            logger.LogInformation($"Komment módosítás vége: Felahasználó: {identityService.GetCurrentUserId()},  Komment: {comment.Id + " " + comment.Content}");

            return Unit.Value;
        }
    }

    public class UpdateCommentCommandValidator : AbstractValidator<UpdateCommentCommand>
    {
        public UpdateCommentCommandValidator()
        {
            RuleFor(x => x.CommentId)
                .NotEmpty();

            RuleFor(x => x.Content)
                .NotEmpty();
        }
    }
}

