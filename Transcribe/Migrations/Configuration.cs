namespace Transcribe.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Transcribe.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<Transcribe.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Transcribe.Models.ApplicationDbContext context)
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            string email = "admin@transcribe.com";
            string displayName = "Admin";
            string password = System.Configuration.ConfigurationManager.AppSettings["initialAdminPassword"];

            //Create Role Admin if it does not exist
            if (!RoleManager.RoleExists(displayName))
            {
                var roleresult = RoleManager.Create(new IdentityRole(displayName));
            }

            //Create User=Admin with password=123456
            var user = new ApplicationUser();
            user.Email = email;
            user.UserName = email;
            user.DisplayName = displayName;
            var adminresult = UserManager.Create(user, password);

            //Add User Admin to Role Admin
            if (adminresult.Succeeded)
            {
                var result = UserManager.AddToRole(user.Id, displayName);
            }
            base.Seed(context);
        }
    }
}
