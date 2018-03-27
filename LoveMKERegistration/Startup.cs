using LoveMKERegistration.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using System.Configuration;

[assembly: OwinStartupAttribute(typeof(LoveMKERegistration.Startup))]
namespace LoveMKERegistration
{
    public partial class Startup
    {

        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateRolesandUsers();
        }
        // This method creates default User roles and Admin user for login.
        private void CreateRolesandUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            // In Startup this is creating first Admin Role and creating a default Admin User.
            if (!roleManager.RoleExists("Admin"))
            {

                //First create Admin role.
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);

                //Then create a Admin super user who will maintain the website.
                var user = new ApplicationUser();
                user.UserName = ConfigurationManager.AppSettings["Admin:Username"];
                user.Email = ConfigurationManager.AppSettings["Admin:Email"];
                string userPWD = ConfigurationManager.AppSettings["Admin:Password"];

                var chkUser = UserManager.Create(user, userPWD);

                //Add default User to Role Admin.
                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "Admin");
                }
            }

            //Creating Leader role.
            if (!roleManager.RoleExists("Leader"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Leader";
                roleManager.Create(role);
            }

            //Creating User role.
            if (!roleManager.RoleExists("User"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "User";
                roleManager.Create(role);
            }
        }
    }
}
