using Application.Interfaces;
using Application.Services;
using Dal;
using Domain.Entities.CommentAggregate;
using Domain.Entities.ProductAggregate;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.ProductAggregate.Commands
{
    public class SaveProductCommand : IRequest<Guid>
    { 
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price{ get; set; }
        public IFormFile CaffFile { get; set; }
    }

    public class SaveProductCommandHandler : IRequestHandler<SaveProductCommand, Guid>
    {
        private readonly IProductRepository productRepository;
        private readonly IFileService fileService;
        private readonly IIdentityService identityService;
        private readonly ILogger<SaveProductCommandHandler> logger;

        public SaveProductCommandHandler(IProductRepository productRepository, IFileService fileService, IIdentityService identityService, ILogger<SaveProductCommandHandler> logger)
        {
            this.productRepository = productRepository;
            this.fileService = fileService;
            this.identityService = identityService;
            this.logger = logger;
        }

        public async Task<Guid> Handle(SaveProductCommand request, CancellationToken cancellationToken)
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Comments = new(),
                CreatedAt = DateTime.UtcNow,
                Description = request.Description,
                Price = request.Price,
                Name = request.Name,
                UploaderId = identityService.GetCurrentUserId(),
            };
            /*
            byte[] fileBytes = new byte[0];
            using (var memoryStream = new MemoryStream())
            {
                await request.CaffFile.CopyToAsync(memoryStream);
                fileBytes = memoryStream.ToArray();
            }


            var file = await fileService.UploadFileAsync("", fileBytes);

            product.CaffFile = file;
            product.CaffFileId = file.Id;
            */
            await productRepository.InsertAsync(product);
            logger.LogInformation($"Termék létrehozása: Felahasználó: {identityService.GetCurrentUserId()}, Termék: {product.Id + " " + product.Name}");
            return product.Id;
        }
    }

    public class SaveProductCommandValidator : AbstractValidator<SaveProductCommand>
    {
        public SaveProductCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();

            RuleFor(x => x.CaffFile.FileName)
                .Must((x) => x.EndsWith(".caff"));

            RuleFor(x => x.Description)
                .NotEmpty();

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0);
        }
    }
}
