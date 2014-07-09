using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Principal;

public static class GenericPrincipalExtensions
{
    public static string DisplayName(this IPrincipal user)
    {
        if (user.Identity.IsAuthenticated)
        {
            ClaimsIdentity claimsIdentity = user.Identity as ClaimsIdentity;
            foreach (var claim in claimsIdentity.Claims)
            {
                if (claim.Type == "DisplayName")
                    return claim.Value;
            }
            return "";
        }
        else
            return "";
    }
}
namespace Transcribe.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string DisplayName { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {          
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            userIdentity.AddClaim(new Claim("DisplayName", this.DisplayName));

            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}