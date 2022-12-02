using Application.Interfaces;
using Application.Services;
using CSONGE.Application.Exceptions;
using Domain.Entities.ProductAggregate;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.ProductAggregate.Commands
{
    public class UpdateProductCommand : IRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
    }

    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Unit>
    {
        private readonly IProductRepository productRepository;
        private readonly IIdentityService identityService;
        private readonly ILogger<UpdateProductCommandHandler> logger;

        public UpdateProductCommandHandler(IProductRepository productRepository, IIdentityService identityService, ILogger<UpdateProductCommandHandler> logger)
        {
            this.productRepository = productRepository;
            this.identityService = identityService;
            this.logger = logger;
        }

        public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await productRepository.SingleAsync(x => x.Id == request.Id);

            logger.LogInformation($"Termék módosítás kezdés: Felahasználó: {identityService.GetCurrentUserId()}, Termék: {product.Id + " " + product.Name + " " + product.Description + " " + product.Price}");

            if (product.UploaderId != identityService.GetCurrentUserId())
            {
                var currentUser = await identityService.GetCurrentUser();
                if (!currentUser.IsAdmin)
                {
                    throw new ApplicationExeption("Csak a feltöltő módosíthatja a fájlt!");
                }
            }

            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            await productRepository.UpdateAsync(product);

            logger.LogInformation($"Termék módosítása vége: Felahasználó: {identityService.GetCurrentUserId()}, Termék: {product.Id + " " + product.Name + " " + product.Description + " " + product.Price}");

            return Unit.Value;
        }
    }

    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();

            RuleFor(x => x.Name)
                .NotEmpty();

            RuleFor(x => x.Description)
                .NotEmpty();

            RuleFor(x => x.Price)
                .NotEmpty();
        }
    }
}
