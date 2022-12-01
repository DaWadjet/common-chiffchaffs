using Application.Interfaces;
using Application.Services;
using Dal;
using Domain.Entities.ProductAggregate;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.ProductAggregate.Commands
{
    public class SaveProductCommand : IRequest<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public FileInformation FileInfo { get; set; }

        public class FileInformation
        {
            public IFormFile CaffFile { get; set; }
            public string OriginalFileName { get; set; }
        }
    }

    public class SaveProductCommandHandler : IRequestHandler<SaveProductCommand, Guid>
    {
        private readonly IProductRepository productRepository;
        private readonly IFileService fileService;
        private readonly IIdentityService identityService;

        public SaveProductCommandHandler(IProductRepository productRepository, IFileService fileService, IIdentityService identityService)
        {
            this.productRepository = productRepository;
            this.fileService = fileService;
            this.identityService = identityService;
        }

        public async Task<Guid> Handle(SaveProductCommand request, CancellationToken cancellationToken)
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Comments = new(),
                CreatedAt = DateTime.UtcNow,
                Description = request.Description,
                Name = request.Name,
                UploaderId = identityService.GetCurrentUserId(),
            };
            /*
            byte[] fileBytes = new byte[0];
            using (var memoryStream = new MemoryStream())
            {
                await request.FileInfo.CaffFile.CopyToAsync(memoryStream);
                fileBytes = memoryStream.ToArray();
            }


            var file = await fileService.UploadFileAsync("", fileBytes);

            product.CaffFile = file;
            product.CaffFileId = file.Id;
            */
            await productRepository.InsertAsync(product);

            return product.Id;
        }
    }
}
