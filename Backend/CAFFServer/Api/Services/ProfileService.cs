using Domain.Entities.User;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Api.Services
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<WebshopUser> userManager;

        public ProfileService(UserManager<WebshopUser> userManager)
        {
            this.userManager = userManager;
        }
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user = await userManager.GetUserAsync(context.Subject);

            context.IssuedClaims.Add(new Claim(JwtClaimTypes.Role, user.IsAdmin ? "admin" : "user"));
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var user = await userManager.GetUserAsync(context.Subject);
            context.IsActive = user != null;
        }
    }
}
