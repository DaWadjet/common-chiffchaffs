using Domain.Entities.ProductAggregate;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ProductAggregate.Queries
{
    public class GetProductDetailsQuery : IRequest<ProductDto>
    {
        public Guid ProductId { get; set; }
    }

    public class GetProductDetailsQueryHandler : IRequestHandler<GetProductDetailsQuery, ProductDto>
    {
        private readonly IProductRepository productRepository;

        public GetProductDetailsQueryHandler(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }

        public async Task<ProductDto> Handle(GetProductDetailsQuery request, CancellationToken cancellationToken)
        {
            var product = await productRepository
                .GetAll()
                .Include(x => x.Comments)
                    .ThenInclude(x => x.Commenter)
                .SingleAsync(x => x.Id == request.ProductId);

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                CreatedAt = product.CreatedAt,
                PreviewUrl = $"https://localhost:5001/previews/{product.CaffFileId}.bmp",
                Comments = product.Comments.Select(x => new ProductDto.CommentDto
                {
                    Id = x.Id,
                    CommenterName = x.Commenter.UserName,
                    Content = x.Content,
                }).ToList()
            };
        }
    }

    public class GetProductDetailsQueryValidator : AbstractValidator<GetProductDetailsQuery>
    {
        public GetProductDetailsQueryValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty();
        }
    }
}
