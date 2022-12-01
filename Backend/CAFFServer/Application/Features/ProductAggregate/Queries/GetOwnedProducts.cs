using Application.Interfaces;
using CSONGE.Application.Extensions;
using CSONGE.Application.Pagination;
using Domain.Entities.ProductAggregate;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace Application.Features.ProductAggregate.Queries
{
    public class GetOwnedProductsQuery : IRequest<IPagedList<ProductDto>>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }

    public class GetOwnedProductsQueryHandler : IRequestHandler<GetOwnedProductsQuery, IPagedList<ProductDto>>
    {
        private readonly IProductRepository productRepository;
        private readonly IIdentityService identityService;

        public GetOwnedProductsQueryHandler(IProductRepository productRepository, IIdentityService identityService)
        {
            this.productRepository = productRepository;
            this.identityService = identityService;
        }

        public async Task<IPagedList<ProductDto>> Handle(GetOwnedProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await productRepository
                .GetAll()
                .Where(x => x.UploaderId == identityService.GetCurrentUserId())
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

    public class GetOwnedProductsQueryValidator : AbstractValidator<GetOwnedProductsQuery>
    {
        public GetOwnedProductsQueryValidator()
        {
            RuleFor(x => x.PageIndex)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(1);
        }
    }
}
