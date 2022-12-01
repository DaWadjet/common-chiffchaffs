using Application.Interfaces;
using Application.Services;
using CSONGE.Application.Exceptions;
using Domain.Entities.ProductAggregate;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ProductAggregate.Commands
{
    public class BuyProductCommand : IRequest<byte[]>
    {
        public Guid ProductId { get; set; }
    }

    public class BuyProductCommandHandler : IRequestHandler<BuyProductCommand, byte[]>
    {
        private readonly IIdentityService identityService;
        private readonly IProductRepository productRepository;
        private readonly IFileService fileService;

        public BuyProductCommandHandler(IIdentityService identityService, IProductRepository productRepository, IFileService fileService)
        {
            this.identityService = identityService;
            this.productRepository = productRepository;
            this.fileService = fileService;
        }

        public async Task<byte[]> Handle(BuyProductCommand request, CancellationToken cancellationToken)
        {
            var currentUser = await identityService.GetCurrentUser();
            var product = await productRepository
                .GetAll()
                .Include(x => x.CaffFile)
                .SingleAsync(x => x.Id == request.ProductId);

            if (currentUser.BoughtFiles.Any(x => x.Id == product.CaffFileId)) {
                throw new ApplicationExeption("Korábban már megvásárolta ezt a caff fájlt.");
            }

            if (currentUser.OwnFiles.Any(x => x.Id == product.CaffFileId))
            {
                throw new ApplicationExeption("Nincs lehetősége megvásárolni a saját termékét.");
            }

            var file = await fileService.LoadCaffFileAsync(product.CaffFileId.GetValueOrDefault());

            product.CaffFile.Customers.Add(currentUser);
            await productRepository.UpdateAsync(product);

            return file;
        }
    }

    public class BuyProductCommandValidator : AbstractValidator<BuyProductCommand>
    {
        public BuyProductCommandValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty();
        }
    }
}
