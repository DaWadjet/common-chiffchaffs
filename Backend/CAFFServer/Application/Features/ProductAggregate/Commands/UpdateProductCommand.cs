using Application.Interfaces;
using CSONGE.Application.Exceptions;
using Domain.Entities.ProductAggregate;
using MediatR;

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

        public UpdateProductCommandHandler(IProductRepository productRepository, IIdentityService identityService)
        {
            this.productRepository = productRepository;
            this.identityService = identityService;
        }

        public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await productRepository.SingleAsync(x => x.Id == request.Id);

            if (product.UploaderId != identityService.GetCurrentUserId())
            {
                var currentUser = await identityService.GetCurrentUser();
                if (!currentUser.IsAdmin)
                {
                    throw new ApplicationExeption();
                }
            }

            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            await productRepository.UpdateAsync(product);

            return Unit.Value;
        }
    }
}
