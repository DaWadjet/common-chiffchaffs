using Application.Interfaces;
using Domain.Entities.CommentAggregate;
using MediatR;

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

        public DeleteCommentCommandHandler(ICommentRepository commentRepository, IIdentityService identityService)
        {
            this.commentRepository = commentRepository;
            this.identityService = identityService;
        }
        public async Task<Unit> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {
            var comment = await commentRepository.SingleAsync(x => x.Id == request.CommentId);

            if (comment == null || (comment.CommenterId != identityService.GetCurrentUserId() && !(await identityService.GetCurrentUser()).IsAdmin))
            {
                throw new ApplicationException();
            }

            await commentRepository.DeleteAsync(comment);

            return Unit.Value;
        }
    }
}

