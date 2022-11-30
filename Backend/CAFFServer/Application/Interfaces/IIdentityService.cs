using Domain.Entities;

namespace Application.Interfaces
{
    public interface IIdentityService
    {
        Guid GetCurrentUserId();
        Task<WebshopUser> GetCurrentUser();
    }
}
