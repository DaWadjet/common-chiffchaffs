using Application.Interfaces;
using Application.Services;
using Domain.Entities.CommentAggregate;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.CommandAggreagte.Commands
{
    public class DeleteCommentCommand : IRequest
    {
        public Guid CommentId { get; set; }
    }

    public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, Unit>
    {
        private readonly ICommentRepository commentRepository;
        private readonly IIdentityService identityService;
        private readonly ILogger<DeleteCommentCommandHandler> logger;

        public DeleteCommentCommandHandler(ICommentRepository commentRepository, IIdentityService identityService, ILogger<DeleteCommentCommandHandler> logger)
        {
            this.commentRepository = commentRepository;
            this.identityService = identityService;
            this.logger = logger;
        }
        public async Task<Unit> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {
            var comment = await commentRepository.SingleAsync(x => x.Id == request.CommentId);

            if (comment == null || (comment.CommenterId != identityService.GetCurrentUserId() && !(await identityService.GetCurrentUser()).IsAdmin))
            {
                throw new ApplicationException();
            }

            await commentRepository.DeleteAsync(comment);

            logger.LogInformation($"Komment törlése: Felahasználó: {identityService.GetCurrentUserId()}, Komment: {comment.Id + " " + comment.Content}");
            return Unit.Value;
        }
    }
}

