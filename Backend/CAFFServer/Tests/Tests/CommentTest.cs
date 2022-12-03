using Application.Features.CommentAggregate.Commands;
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
    public class CommentTest : UnitTestBase
    {
        [Fact]
        public async Task Test_SaveComment()
        {
            var id = Guid.NewGuid();
            var caffId = Guid.NewGuid();
            var content = "teszt_content";

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

            var command = new SaveCommentCommand
            {
                ProductId = id,
                Content = content,
            };

            var logger = new Mock<ILogger<SaveCommentCommandHandler>>();
            var handler = new SaveCommentCommandHandler(webshopDbContext, identityServiceUser, logger.Object);

            await handler.Handle(command, CancellationToken.None);

            var comments = await mockedRepositories.CommentRepository
                .GetAll()
                .ToListAsync();

            Assert.Single(comments);
            Assert.Equal(command.Content, comments.First().Content);
        }

        [Fact]
        public async Task Test_UpdateComment()
        {
            var id = Guid.NewGuid();
            var caffId = Guid.NewGuid();
            var commentId = Guid.NewGuid();
            var content = "teszt_content";
            var updatedContent = "teszt_content_updated";

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

            webshopDbContext.Comments.Add(new Domain.Entities.CommentAggregate.Comment { Id = commentId, CommenterId = UserId, ProductId = id, Content = content });

            await webshopDbContext.SaveChangesAsync();

            var command = new UpdateCommentCommand
            {
                CommentId = commentId,
                Content = updatedContent,
            };

            var logger = new Mock<ILogger<UpdateCommentCommandHandler>>();
            var handler = new UpdateCommentCommandHandler(mockedRepositories.CommentRepository, identityServiceUser, logger.Object);

            await handler.Handle(command, CancellationToken.None);

            var comments = await mockedRepositories.CommentRepository
                .GetAll()
                .ToListAsync();

            Assert.Single(comments);
            Assert.Equal(command.Content, comments.First().Content);
        }

        [Fact]
        public async Task Test_DeleteComment()
        {
            var id = Guid.NewGuid();
            var caffId = Guid.NewGuid();
            var commentId = Guid.NewGuid();
            var content = "teszt_content";

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

            webshopDbContext.Comments.Add(new Domain.Entities.CommentAggregate.Comment { Id = commentId, CommenterId = UserId, ProductId = id, Content = content });

            await webshopDbContext.SaveChangesAsync();

            var command = new DeleteCommentCommand
            {
                CommentId = commentId,
            };

            var logger = new Mock<ILogger<DeleteCommentCommandHandler>>();
            var handler = new DeleteCommentCommandHandler(mockedRepositories.CommentRepository, identityServiceUser, logger.Object);

            await handler.Handle(command, CancellationToken.None);

            var comments = await mockedRepositories.CommentRepository
                .GetAll()
                .ToListAsync();

            Assert.Empty(comments);
        }

        [Fact]
        public async Task Test_ThrowUpdateComment()
        {
            var id = Guid.NewGuid();
            var caffId = Guid.NewGuid();
            var commentId = Guid.NewGuid();
            var content = "teszt_content";
            var updatedContent = "teszt_content_updated";

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

            webshopDbContext.Comments.Add(new Domain.Entities.CommentAggregate.Comment { Id = commentId, CommenterId = AdminUserId, ProductId = id, Content = content });

            await webshopDbContext.SaveChangesAsync();

            var command = new UpdateCommentCommand
            {
                CommentId = commentId,
                Content = updatedContent,
            };

            var logger = new Mock<ILogger<UpdateCommentCommandHandler>>();
            var handler = new UpdateCommentCommandHandler(mockedRepositories.CommentRepository, identityServiceUser, logger.Object);

            Func<Task> act = () => handler.Handle(command, CancellationToken.None);
            var exception = await Assert.ThrowsAsync<CSONGE.Application.Exceptions.ApplicationException>(act);

            Assert.Equal("Csak a saját kommentek módosíthatók!", exception.Message);
        }

        [Fact]
        public async Task Test_ThrowDeleteComment()
        {
            var id = Guid.NewGuid();
            var caffId = Guid.NewGuid();
            var commentId = Guid.NewGuid();
            var content = "teszt_content";

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

            webshopDbContext.Comments.Add(new Domain.Entities.CommentAggregate.Comment { Id = commentId, CommenterId = AdminUserId, ProductId = id, Content = content });

            await webshopDbContext.SaveChangesAsync();

            var command = new DeleteCommentCommand
            {
                CommentId = commentId,
            };

            var logger = new Mock<ILogger<DeleteCommentCommandHandler>>();
            var handler = new DeleteCommentCommandHandler(mockedRepositories.CommentRepository, identityServiceUser, logger.Object);

            Func<Task> act = () => handler.Handle(command, CancellationToken.None);
            var exception = await Assert.ThrowsAsync<CSONGE.Application.Exceptions.ApplicationException>(act);

            Assert.Equal("Csak a saját kommentek módosíthatók!", exception.Message);
        }
    }
}
