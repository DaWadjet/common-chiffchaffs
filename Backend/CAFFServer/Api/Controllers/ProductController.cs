using Application.Features.ProductAggregate.Commands;
using Application.Features.ProductAggregate.Queries;
using CSONGE.Application.Pagination;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api
{
    [ApiController]
    [Route("api/product")]
    public class ProductController : ControllerBase
    {
        private readonly IMediator mediator;

        public ProductController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [Authorize(Policy = "User")]
        [HttpPost]
        public async Task<Guid> SaveProduct([FromForm] SaveProductCommand command)
        {
            return await mediator.Send(command);
        }

        [Authorize(Policy = "User")]
        [HttpPut]
        public async Task UpdateProduct(UpdateProductCommand command)
        {
            await mediator.Send(command);
        }

        [Authorize(Policy = "User")]
        [HttpDelete("{productId}")]
        public async Task DeleteProduct(Guid productId)
        {
            await mediator.Send(new DeleteProductCommand { ProductId = productId });
        }

        [Authorize(Policy = "User")]
        [HttpGet]
        public async Task<IPagedList<ProductDto>> ListProducts([FromQuery] GetProductsQuery query)
        {
            return await mediator.Send(query);
        }

        [Authorize(Policy = "User")]
        [HttpGet("own")]
        public async Task<IPagedList<ProductDto>> ListProducts([FromQuery] GetOwnedProductsQuery query)
        {
            return await mediator.Send(query);
        }


        [Authorize(Policy = "User")]
        [HttpGet("details/{id}")]
        public async Task<ProductDto> GetProductDetails(Guid id)
        {
            return await mediator.Send(new GetProductDetailsQuery { ProductId = id });
        }

        [Authorize(Policy = "User")]
        [HttpGet("file/{id}")]
        public async Task<IActionResult> GetBoughtFile(Guid id)
        {
            var fileContent = await mediator.Send(new GetBoughtFileQuery { CaffFileId = id });
            var content = new MemoryStream(fileContent);
            var contentType = "APPLICATION/octet-stream";
            return File(content, contentType);
        }

        [Authorize(Policy = "User")]
        [HttpGet("buy/{id}")]
        public async Task<IActionResult> BuyFile(Guid id)
        {
            var fileContent = await mediator.Send(new BuyProductCommand { CaffFileId = id });
            var content = new MemoryStream(fileContent);
            var contentType = "APPLICATION/octet-stream";
            return File(content, contentType);
        }

    }
}
