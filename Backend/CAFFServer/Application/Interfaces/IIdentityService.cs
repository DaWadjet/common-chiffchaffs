using Domain.Entities.User;

namespace Application.Interfaces
{
    public interface IIdentityService
    {
        Guid GetCurrentUserId();
        Task<WebshopUser> GetCurrentUser();
    }
}
