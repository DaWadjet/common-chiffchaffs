using Application.Features.ProductAggregate.Commands;
using CSONGE.Application.Exceptions;
using Domain.Entities.ProductAggregate;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Tests.Base;

namespace Tests
{
    public class ProductTest : UnitTestBase
    {
        [Fact]
        public async Task Test_SaveProduct()
        {
            var content = "Hello World from a Fake File";
            var fileName = "test.pdf";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;

            //create FormFile with desired data
            IFormFile file = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);

            var command = new SaveProductCommand
            {
                Name = "Test",
                Description = "Test",
                Price = 1,
                CaffFile = file
            };

            var logger = new Mock<ILogger<SaveProductCommandHandler>>();
            var handler = new SaveProductCommandHandler(mockedRepositories.ProductRepository, fileService, identityServiceUser, logger.Object);

            await handler.Handle(command, CancellationToken.None);

            var products = await mockedRepositories.ProductRepository
                .GetAll()
                .ToListAsync();

            Assert.Single(products);
            Assert.Equal(command.Name, products.First().Name);
            Assert.Equal(command.Description, products.First().Description);
            Assert.Equal(command.Price, products.First().Price);
        }

        [Fact]
        public async Task Test_UpdateProduct()
        {
            var id = Guid.NewGuid();
            var caffId = Guid.NewGuid();

            webshopDbContext.Products.Add(new Product 
            {
                Id = id,
                CaffFileId = caffId,
                CaffFile = new Domain.Entities.CaffFileAggregate.CaffFile { Id = caffId, OriginalFileName = "teszt", UploaderId = UserId},
                CreatedAt = DateTime.UtcNow,
                Description = "teszt",
                Name = "teszt",
                Price = 10,
                UploaderId = UserId,
            });

            await webshopDbContext.SaveChangesAsync();

            var command = new UpdateProductCommand
            {
                Name = "UpdatedName",
                Description = "UpdatedDesc",
                Price = 1,
                Id = id
            };

            var logger = new Mock<ILogger<UpdateProductCommandHandler>>();
            var handler = new UpdateProductCommandHandler(mockedRepositories.ProductRepository, identityServiceUser, logger.Object);

            await handler.Handle(command, CancellationToken.None);

            var products = await mockedRepositories.ProductRepository
                .GetAll()
                .ToListAsync();

            Assert.Single(products);
            Assert.Equal(command.Name, products.First().Name);
            Assert.Equal(command.Description, products.First().Description);
            Assert.Equal(command.Price, products.First().Price);
        }

        [Fact]
        public async Task Test_DeleteProduct()
        {
            var id = Guid.NewGuid();
            var caffId = Guid.NewGuid();

            webshopDbContext.Products.Add(new Product
            {
                Id = id,
                CaffFileId = caffId,
                CaffFile = new Domain.Entities.CaffFileAggregate.CaffFile { Id = caffId, OriginalFileName = "teszt", UploaderId = UserId },
                CreatedAt = DateTime.UtcNow,
                Description = "teszt",
                Name = "teszt",
                Price = 10,
                UploaderId = UserId,
            });

            await webshopDbContext.SaveChangesAsync();

            var command = new DeleteProductCommand
            {
                ProductId = id
            };

            var logger = new Mock<ILogger<DeleteProductCommandHandler>>();
            var handler = new DeleteProductCommandHandler(identityServiceUser, mockedRepositories.ProductRepository, fileService, logger.Object);

            await handler.Handle(command, CancellationToken.None);

            var products = await mockedRepositories.ProductRepository
                .GetAll()
                .ToListAsync();

            Assert.Empty(products);
        }

        [Fact]
        public async Task Test_ThrowUpdateProduct()
        {
            var id = Guid.NewGuid();
            var caffId = Guid.NewGuid();

            webshopDbContext.Products.Add(new Product
            {
                Id = id,
                CaffFileId = caffId,
                CaffFile = new Domain.Entities.CaffFileAggregate.CaffFile { Id = caffId, OriginalFileName = "teszt", UploaderId = AdminUserId },
                CreatedAt = DateTime.UtcNow,
                Description = "teszt",
                Name = "teszt",
                Price = 10,
                UploaderId = AdminUserId,
            });

            await webshopDbContext.SaveChangesAsync();

            var command = new UpdateProductCommand
            {
                Name = "UpdatedName",
                Description = "UpdatedDesc",
                Price = 1,
                Id = id
            };

            var logger = new Mock<ILogger<UpdateProductCommandHandler>>();
            var handler = new UpdateProductCommandHandler(mockedRepositories.ProductRepository, identityServiceUser, logger.Object);

            Func<Task> act = () => handler.Handle(command, CancellationToken.None);
            var exception = await Assert.ThrowsAsync<CSONGE.Application.Exceptions.ApplicationException>(act);

            Assert.Equal("Csak a feltöltő módosíthatja a fájlt!", exception.Message);
        }

        [Fact]
        public async Task Test_ThrowDeleteProduct()
        {
            var id = Guid.NewGuid();
            var caffId = Guid.NewGuid();

            webshopDbContext.Products.Add(new Product
            {
                Id = id,
                CaffFileId = caffId,
                CaffFile = new Domain.Entities.CaffFileAggregate.CaffFile { Id = caffId, OriginalFileName = "teszt", UploaderId = AdminUserId },
                CreatedAt = DateTime.UtcNow,
                Description = "teszt",
                Name = "teszt",
                Price = 10,
                UploaderId = AdminUserId,
            });

            await webshopDbContext.SaveChangesAsync();

            var command = new DeleteProductCommand
            {
                ProductId = id
            };

            var logger = new Mock<ILogger<DeleteProductCommandHandler>>();
            var handler = new DeleteProductCommandHandler(identityServiceUser, mockedRepositories.ProductRepository, fileService, logger.Object);

            Func<Task> act = () => handler.Handle(command, CancellationToken.None);
            var exception = await Assert.ThrowsAsync<CSONGE.Application.Exceptions.ApplicationException>(act);

            Assert.Equal("Csak a feltöltő módosíthatja a fájlt!", exception.Message);
        }
    }
}
