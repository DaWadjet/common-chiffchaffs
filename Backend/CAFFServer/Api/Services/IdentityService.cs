using Application.Interfaces;
using Domain.Entities.User;
using IdentityModel;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

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
            var userId = httpContext.User.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject).Value;
            return Guid.Parse(userId);
        }
        public async Task<WebshopUser> GetCurrentUser()
        {
            return await webshopUserRepository.GetAll()
                .Include(x => x.Products)
                    .ThenInclude(x => x.CaffFile)
                .Include(x => x.OwnFiles)
                .Include(x => x.BoughtFiles)
                .SingleAsync(x => x.Id == GetCurrentUserId());
        }
    }
}
