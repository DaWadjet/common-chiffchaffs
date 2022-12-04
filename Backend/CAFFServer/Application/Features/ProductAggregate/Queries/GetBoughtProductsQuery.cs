using Application.Interfaces;
using CSONGE.Application.Extensions;
using CSONGE.Application.Pagination;
using Dal.Repositories.ProductAggregate;
using Dal.Repositories.WebshopUserAggregate;
using Domain.Entities.ProductAggregate;
using Domain.Entities.User;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace Application.Features.ProductAggregate.Queries
{
    public class GetBoughtProductsQuery : IRequest<IPagedList<ProductDto>>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }

    public class GetBoughtProductsQueryHandler : IRequestHandler<GetBoughtProductsQuery, IPagedList<ProductDto>>
    {
        private readonly IWebshopUserRepository webshopUserRepository;
        private readonly IProductRepository productRepository;
        private readonly IIdentityService identityService;

        public GetBoughtProductsQueryHandler(IWebshopUserRepository webshopUserRepository, IProductRepository productRepository, IIdentityService identityService)
        {
            this.webshopUserRepository = webshopUserRepository;
            this.productRepository = productRepository;
            this.identityService = identityService;
        }

        public async Task<IPagedList<ProductDto>> Handle(GetBoughtProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await productRepository
                .GetAll()
                .Where(x => x.CaffFile.Customers.Any(c => c.Id == identityService.GetCurrentUserId()))
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

    public class GetBoughtProductsQueryValidator : AbstractValidator<GetOwnedProductsQuery>
    {
        public GetBoughtProductsQueryValidator()
        {
            RuleFor(x => x.PageIndex)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(1);
        }
    }
}
