using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Data.Entity;

namespace HooliganGame.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {

        public int Level { get; set; }

        public int Experience { get; set; }

        public int Strength { get; set; }

        public int Speed { get; set; }

        public int Mentality { get; set; }

        public int Fights { get; set; }

        public double FightsWon { get; set; }

        public int Money { get; set; }
    }
    public class ApplicationRole : IdentityRole
    {

    }

   }