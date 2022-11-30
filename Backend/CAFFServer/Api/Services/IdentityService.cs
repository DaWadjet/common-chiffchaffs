using Application.Interfaces;
using Domain.Entities.User;
using System.Security.Claims;

namespace Api.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly HttpContext httpContext;
        private readonly IWebshopUserRepository webshopUserRepository;

        public IdentityService(
            IHttpContextAccessor httpContextAccessor,
            IWebshopUserRepository webshopUserRepository)
        {
            httpContext = httpContextAccessor.HttpContext;
            this.webshopUserRepository = webshopUserRepository;
        }

        public Guid GetCurrentUserId()
        {
            var userId = httpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            return Guid.Parse(userId);
        }
        public async Task<WebshopUser> GetCurrentUser()
        {
            return await webshopUserRepository.SingleAsync(x => x.Id == GetCurrentUserId());
        }
    }
}
