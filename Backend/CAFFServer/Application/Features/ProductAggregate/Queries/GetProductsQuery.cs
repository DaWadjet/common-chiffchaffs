using CSONGE.Application.Extensions;
using CSONGE.Application.Pagination;
using Domain.Entities.ProductAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ProductAggregate.Queries
{
    public class GetProductsQuery : IRequest<IPagedList<ProductDto>>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }

    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public string PreviewUrl { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<CommentDto> Comments { get; set; }
        public class CommentDto
        {
            public Guid Id { get; set; }
            public string CommenterName { get; set; }
            public string Content { get; set; }
        }
    }

    public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, IPagedList<ProductDto>>
    {
        private readonly IProductRepository productRepository;

        public GetProductsQueryHandler(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }

        public async Task<IPagedList<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await productRepository
                .GetAll()
                .Include(x => x.Comments)
                    .ThenInclude(x => x.Commenter)
                .OrderByDescending(x => x.CreatedAt)
                .ToPagedListAsync(request.PageIndex, request.PageSize);

            return new PagedList<ProductDto>
            {
                PageSize = products.PageSize,
                PageIndex = products.PageIndex,
                ItemCount = products.ItemCount,
                PageCount = products.PageCount,
                Items = products.Items.Select(x => new ProductDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Price = x.Price,
                    Description = x.Description,
                    CreatedAt = x.CreatedAt,
                    PreviewUrl = $"https://localhost:5001/previews/{x.CaffFileId}.bmp",
                    Comments = x.Comments.Select(x => new ProductDto.CommentDto
                    {
                        Id = x.Id,
                        CommenterName = x.Commenter.UserName,
                        Content = x.Content,
                    }).ToList()
                }).ToList()
            };
        }
    }
}
