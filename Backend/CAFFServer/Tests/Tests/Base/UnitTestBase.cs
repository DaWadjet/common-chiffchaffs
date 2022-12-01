using Application.Interfaces;
using Application.Services;
using Dal;
using Domain.Entities.CaffFileAggregate;
using Domain.Entities.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Tests.Base;

public class UnitTestBase : IDisposable
{
    protected readonly SqliteConnection connection;
    protected readonly UnitTestRepositories mockedRepositories;
    protected WebshopDbContext webshopDbContext;
    protected readonly IIdentityService identityServiceUser;
    protected readonly IIdentityService identityServiceAdmin;
    protected readonly IFileService fileService;

    protected readonly Guid UserId;
    protected readonly Guid AdminUserId;
    protected readonly Guid CaffFileId;

    public UnitTestBase()
    {
        connection = new SqliteConnection("Filename=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<WebshopDbContext>()
            .UseSqlite(connection)
            .Options;

        UserId = Guid.Parse("85b4a2a3-aa22-40e4-9b91-c72e96ab8e08");
        AdminUserId = Guid.Parse("85b4a2a3-aa22-40e4-9b91-c72e96ab8e09");
        CaffFileId = Guid.NewGuid();

        var mockFileService = new Mock<IFileService>();
        mockFileService.Setup(x => x.DeleteFiles(It.IsAny<Guid>())).Callback(() => { });
        mockFileService.Setup(x => x.UploadFileAsync(It.IsAny<string>(), It.IsAny<byte[]>())).Returns(Task.FromResult(new CaffFile
        {
            Id = CaffFileId,
            OriginalFileName = "originalName"
        }));

        var mockIdentityServiceUser = new Mock<IIdentityService>();
        mockIdentityServiceUser.Setup(x => x.GetCurrentUserId()).Returns(UserId);
        mockIdentityServiceUser.Setup(x => x.GetCurrentUser()).Returns(Task.FromResult(new WebshopUser
        {
            Id = UserId,
            UserName = "Test",
            IsAdmin = false
        }));

        var mockIdentityServiceAdmin = new Mock<IIdentityService>();
        mockIdentityServiceAdmin.Setup(x => x.GetCurrentUserId()).Returns(AdminUserId);
        mockIdentityServiceAdmin.Setup(x => x.GetCurrentUser()).Returns(Task.FromResult(new WebshopUser
        {
            Id = AdminUserId,
            UserName = "TestAdmin",
            IsAdmin = true
        }));

        identityServiceUser = mockIdentityServiceUser.Object;
        identityServiceAdmin = mockIdentityServiceAdmin.Object;

        webshopDbContext = new WebshopDbContext(options);
        webshopDbContext.Database.EnsureCreated();

        mockedRepositories = new UnitTestRepositories(webshopDbContext);

        var users = new List<WebshopUser>
            {
                new WebshopUser
                {
                    Id = UserId,
                    UserName = "user",
                    IsAdmin = false,
                },
                new WebshopUser
                {
                    Id = AdminUserId,
                    UserName = "admin",
                    IsAdmin = true,
                }
            };

        PasswordHasher<WebshopUser> passwordHasher = new PasswordHasher<WebshopUser>();
        foreach (var user in users)
        {
            user.PasswordHash = passwordHasher.HashPassword(user, "Aa1234.");
            user.NormalizedUserName = user.UserName.ToUpper();
            user.SecurityStamp = Guid.NewGuid().ToString();
        }

        webshopDbContext.Users.AddRange(users);
        webshopDbContext.SaveChanges();
    }


    public void Dispose()
    {
        webshopDbContext.Dispose();
        connection.Dispose();
    }
}