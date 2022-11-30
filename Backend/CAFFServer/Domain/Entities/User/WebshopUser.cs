using Microsoft.AspNetCore.Identity;

namespace Domain.Entities.User;

public class WebshopUser : IdentityUser<Guid>
{
    public bool IsAdmin { get; set; }
}
