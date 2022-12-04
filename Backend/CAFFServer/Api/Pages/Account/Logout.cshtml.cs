using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Api.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly IConfiguration configuration;
        public string LogoutId { get; set; } = "";
        public LogoutModel(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void OnGet(string logoutId)
        {
            LogoutId = logoutId;
        }

        public async Task<IActionResult> OnPostAsync(string action)
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return Redirect(configuration.GetValue<string>("AfterAbortedLogoutUri"));
        }
    }
}
