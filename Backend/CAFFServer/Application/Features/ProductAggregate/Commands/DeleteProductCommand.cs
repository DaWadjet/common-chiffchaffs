using Application.Interfaces;
using Application.Services;
using CSONGE.Application.Exceptions;
using Domain.Entities.CommentAggregate;
using Domain.Entities.ProductAggregate;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.ProductAggregate.Commands
{
    public class DeleteProductCommand : IRequest
    {
        public Guid ProductId { get; set; }
    }

    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Unit>
    {
        private readonly IIdentityService identityService;
        private readonly IProductRepository productRepository;
        private readonly IFileService fileService;
        private readonly ILogger<DeleteProductCommandHandler> logger;

        public DeleteProductCommandHandler(IIdentityService identityService, IProductRepository productRepository, IFileService fileService, ILogger<DeleteProductCommandHandler> logger)
        {
            this.identityService = identityService;
            this.productRepository = productRepository;
            this.fileService = fileService;
            this.logger = logger;
        }

        public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await productRepository.SingleAsync(x => x.Id == request.ProductId);

            if (product.UploaderId != identityService.GetCurrentUserId())
            {
                var currentUser = await identityService.GetCurrentUser();
                if (!currentUser.IsAdmin)
                {
                    throw new CSONGE.Application.Exceptions.ApplicationException("Csak a feltöltő módosíthatja a fájlt!");
                }
            }

            var fileId = product.CaffFileId;
            await productRepository.DeleteAsync(product);
            fileService.DeleteFiles(fileId.GetValueOrDefault());

            logger.LogInformation(message: $"Termék törlése: Felahasználó: {identityService.GetCurrentUserId()}, Termék: {product.Id + " " + product.Name}");
            return Unit.Value;
        }
    }

    public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
    {
        public DeleteProductCommandValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty();
        }
    }
}
