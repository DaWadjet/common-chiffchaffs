using Application.Interfaces;
using Application.Services;
using CSONGE.Application.Exceptions;
using FluentValidation;
using MediatR;

namespace Application.Features.ProductAggregate.Queries
{
    public class GetBoughtFileQuery : IRequest<byte[]>
    {
        public Guid CaffFileId { get; set; }
    }

    public class GetBoughtFileHandler : IRequestHandler<GetBoughtFileQuery, byte[]>
    {
        private readonly IIdentityService identityService;
        private readonly IFileService fileService;

        public GetBoughtFileHandler(IIdentityService identityService, IFileService fileService)
        {
            this.identityService = identityService;
            this.fileService = fileService;
        }

        public async Task<byte[]> Handle(GetBoughtFileQuery request, CancellationToken cancellationToken)
        {
            var currentUser = await identityService.GetCurrentUser();

            if (!currentUser.BoughtFiles.Any(x => x.Id == request.CaffFileId))
            {
                throw new CSONGE.Application.Exceptions.ApplicationException("A fájl nem létezik, vagy még nem vásárolta meg!");
            }

            var file = await fileService.LoadCaffFileAsync(request.CaffFileId);

            return file;
        }
    }

    public class GetBoughtFileQueryValidator : AbstractValidator<GetBoughtFileQuery>
    {
        public GetBoughtFileQueryValidator()
        {
            RuleFor(x => x.CaffFileId)
                .NotEmpty();
        }
    }

}