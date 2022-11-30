using Dal;
using Domain.Entities.User;
using IdentityModel;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Api.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly UserManager<WebshopUser> userManager;
    private readonly WebshopDbContext context;

    [Required(ErrorMessage = "Kötelezõ")]
    [BindProperty]
    public string Username { get; set; } = "";

    [Required(ErrorMessage = "Kötelezõ")]
    [BindProperty]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Kötelezõ")]
    [MinLength(6, ErrorMessage = "A jelszónak legalább 6 karakterbõl kell állnia")]
    [BindProperty]
    public string Password { get; set; } = "";

    [Required(ErrorMessage = "Kötelezõ")]
    [BindProperty]
    public string ConfirmPassword { get; set; } = "";

    [BindProperty]
    public string ReturnUrl { get; set; } = "";

    public RegisterModel(UserManager<WebshopUser> userManager, WebshopDbContext context)
    {
        this.userManager = userManager;
        this.context = context;
    }

    public void OnGet(string returnUrl)
    {
        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(string action)
    {
        if (action == "login")
        {
            return RedirectToPage("/Account/Login", new { returnUrl = Request.GetEncodedUrl() });
        }

        await ValidateFieldsAsync();

        if (ModelState.IsValid)
        {
            var user = new WebshopUser
            {
                UserName = Username,
                Email = Email
            };

            var createResult = await userManager.CreateAsync(user, Password);
            if (createResult.Succeeded)
            {
                await userManager.AddClaimAsync(user, new Claim(JwtClaimTypes.Name, user.UserName));
                await userManager.AddClaimAsync(user, new Claim(JwtClaimTypes.Id, user.Id.ToString()));

                return Redirect(ReturnUrl);
            }
            else
            {
                AddModelErrorsForField(nameof(Username), createResult);
                AddModelErrorsForField(nameof(Email), createResult);
                AddModelErrorsForField(nameof(Password), createResult);
            }
        }

        return Page();
    }

    private void AddModelErrorsForField(string fieldName, IdentityResult identityResult)
    {
        foreach (var error in identityResult.Errors.Where(x => x.Code.ToLower().Contains(fieldName.ToLower())))
        {
            ModelState.AddModelError(fieldName, error.Description);
        }
    }

    private async Task ValidateFieldsAsync()
    {
        var usernameTaken = Username != null && await context.Users.AnyAsync(x => x.UserName == Username);
        var emailTaken = Email != null && await context.Users.AnyAsync(x => x.Email == Email);

        if (usernameTaken)
        {
            ModelState.AddModelError(nameof(Username), "A felhasználónév már foglalt!");
        }

        if (emailTaken)
        {
            ModelState.AddModelError(nameof(Email), "Az e-mail cím már foglalt!");
        }

        if (ConfirmPassword != Password)
        {
            ModelState.AddModelError(nameof(ConfirmPassword), "A megadott jelszavak nem egyeznek!");
        }
    }
}
