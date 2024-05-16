using Microsoft.AspNetCore.Identity;

namespace DataAccess
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public ApplicationRole() : base()
        {
        }

        public ApplicationRole(string roleName, string description) : base(roleName)
        {
            Description = description;
        }


        public string Description { get; set; } = string.Empty;
        
    }
}
