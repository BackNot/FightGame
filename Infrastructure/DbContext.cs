using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using HooliganGame.Models;

namespace HooliganGame.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
          Database.SetInitializer(new CustInit());

        }
    }

    public class CustInit : CreateDatabaseIfNotExists<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            // Seed DB with roles
            var store = new RoleStore<IdentityRole>(context);
            var manager = new RoleManager<IdentityRole>(store);
            var role = new ApplicationRole { Name = "Newbie" };
            manager.Create(role);
            role = new ApplicationRole { Name = "Hooligan" };
            manager.Create(role);
            role = new ApplicationRole { Name = "Almost Pro" };
            manager.Create(role);
            role = new ApplicationRole { Name = "Mafia" };
            manager.Create(role);

            // Seed users
            var userStore = new UserStore<IdentityUser>(context);
            var userManager = new UserManager<IdentityUser>(userStore);

            var user = new ApplicationUser() { UserName="user1" , Level=3 , Money=3000 , Speed=101 , Strength=120, Experience=1000 , Fights=10 , FightsWon=10 };
            userManager.Create(user, "123456");
            userManager.AddToRole(user.Id, "Newbie");

             user = new ApplicationUser() { UserName = "user2", Level = 15, Money = 11000, Speed = 300, Strength = 150, Experience = 1000, Fights = 22, FightsWon = 18 };
            userManager.Create(user, "123456");
            userManager.AddToRole(user.Id, "Hooligan");

            user = new ApplicationUser() { UserName = "user3", Level = 55, Money = 11000, Speed = 500, Strength = 250, Experience = 1000, Fights = 25, FightsWon = 25 };
            userManager.Create(user, "123456");
            userManager.AddToRole(user.Id, "Mafia");

            user = new ApplicationUser() { UserName = "user4", Level = 2, Money = 222, Speed = 5, Strength = 5, Experience = 100, Fights = 5, FightsWon = 1 };
            userManager.Create(user, "123456");
            userManager.AddToRole(user.Id, "Newbie");
            base.Seed(context);
        }
    }
}