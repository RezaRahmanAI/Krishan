using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class ApplicationUserRole : IdentityUserRole<int> { }
    public class ApplicationUserClaim : IdentityUserClaim<int> { }
    public class ApplicationUserLogin : IdentityUserLogin<int> { }

    public class ApplicationRole : IdentityRole<int, ApplicationUserRole>
    {
        public ApplicationRole() { }
    }

    public class ApplicationUser : IdentityUser<int, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {
        public int ConcernID { get; set; }
        public int EmployeeID { get; set; }
        public virtual SisterConcern SisterConcern { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser, int> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            userIdentity.AddClaim(new Claim("ConcernID", ConcernID.ToString()));

            return userIdentity;
        }
    }
}
