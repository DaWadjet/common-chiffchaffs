using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Api.Pages.Account;

public class LogoutModel : PageModel
{
    private readonly IConfiguration configuration;
    public LogoutModel(
        IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(string action)
    {
        if (action == "cancel")
        {
            return Redirect(configuration.GetValue<string>("AfterAbortedLogoutUri"));
        }
        else
        {
            await HttpContext.SignOutAsync(JwtBearerDefaults.AuthenticationScheme);
            return Redirect(configuration.GetValue<string>("AfterAbortedLogoutUri"));
        }
    }
}
