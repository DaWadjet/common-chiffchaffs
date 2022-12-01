using Application.Interfaces;
using Domain.Entities.CommentAggregate;
using MediatR;

namespace Application.Features.CommandAggreagte.Commands
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

        public UpdateCommentCommandHandler(ICommentRepository commentRepository, IIdentityService identityService)
        {
            this.commentRepository = commentRepository;
            this.identityService = identityService;
        }
        public async Task<Unit> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
        {
            var comment = await commentRepository.SingleAsync(x => x.Id == request.CommentId);

            if (comment == null || (comment.CommenterId != identityService.GetCurrentUserId() && !(await identityService.GetCurrentUser()).IsAdmin))
            {
                throw new ApplicationException();
            }

            comment.Update(request.Content);

            await commentRepository.UpdateAsync(comment);

            return Unit.Value;
        }
    }
}

