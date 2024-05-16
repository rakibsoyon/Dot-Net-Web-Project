using Microsoft.AspNetCore.Identity;

namespace DataAccess
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string Name { get; set; } = string.Empty;
    }
}
