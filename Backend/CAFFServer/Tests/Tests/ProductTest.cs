using Application.Features.ProductAggregate.Commands;
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
    }
}
