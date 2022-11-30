using Microsoft.AspNetCore.Identity;

namespace Api.Services
{
    public class IdentityService
    {/*
        private readonly HttpContext httpContext;
        private readonly IVeterinaryUserRepository veterinaryUserRepository;
        private readonly UserManager<VeterinaryUser> userManager;

        public IdentityService(
            IHttpContextAccessor httpContextAccessor,
            IVeterinaryUserRepository veterinaryUserRepository,
            UserManager<VeterinaryUser> userManager)
        {
            httpContext = httpContextAccessor.HttpContext;
            this.veterinaryUserRepository = veterinaryUserRepository;
            this.userManager = userManager;
        }

        public Guid GetCurrentUserId()
        {
            var userId = httpContext.User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub).Value;
            return Guid.Parse(userId);
        }
        public async Task<VeterinaryUser> GetCurrentUser()
        {
            return await veterinaryUserRepository.FindAsync(GetCurrentUserId());
        }

        public async Task<bool> IsInRoleAsync(string role)
        {
            var currentUser = await GetCurrentUser();
            return await userManager.IsInRoleAsync(currentUser, role);
        }

        public async Task<bool> IsInRoleAsync(Guid userId, string role)
        {
            var user = await veterinaryUserRepository.FindAsync(userId);
            return await userManager.IsInRoleAsync(user, role);
        }*/
    }
}
