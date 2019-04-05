using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using HooliganGame.Models;
using HooliganGame.Infrastructure;


namespace HooliganGame.Controllers
{
    public class TrainController : Controller
    {
        [Authorize]
        public ActionResult Speed()
        {
            return View();
        }
        [Authorize]
        public ActionResult Strength()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Treadmill() // if user chose to run on the treadmill
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            if (User.Identity.GetUserId() != null)
            {
                var user = UserManager.FindById(User.Identity.GetUserId()); // find that user   

                ViewBag.role = user.Roles.SingleOrDefault().Role.Name;
                if(user.Money>=user.Level*5)
                {
                    
                
                ViewBag.SpeedGained = user.Level * 10;
                ViewBag.MoneySpended = user.Level * 50;
                ViewBag.ExpGained = user.Level * 5;
                user.Speed += user.Level * 10; // increase his speed by his level * 10
                user.Money -= user.Level * 50; // decrease his money
                user.Experience += user.Level * 5;
                UserManager.Update(user);
                }
                else
                {
                    ViewBag.Money = "You don't have enough money";
                }
                return View("SpeedTired"); // go back to view
            }
            else return RedirectToAction("Login", "Account");
        }
        [HttpPost]
        public ActionResult Rope()
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            if (User.Identity.GetUserId() != null)
            {
                var user = UserManager.FindById(User.Identity.GetUserId()); // find that user   

                ViewBag.role = user.Roles.SingleOrDefault().Role.Name;
                if (user.Money >= user.Level * 25)
                {
                    ViewBag.SpeedGained = user.Level * 5;
                    ViewBag.MoneySpended = user.Level * 25;
                    ViewBag.ExpGained = user.Level * 5;
                    user.Speed += user.Level * 5; // increase his speed by his level * 5
                    user.Money -= user.Level * 25; // decrease his money
                    user.Experience += user.Level * 5;
                    UserManager.Update(user);
                }
                else
                {
                    ViewBag.Money = "You don't have enough money";
                }
                return View("SpeedTired"); // go back to view
            }
            else return RedirectToAction("Login", "Account");
        }
        [HttpPost]
        public ActionResult Bench()
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var user = UserManager.FindById(User.Identity.GetUserId()); // find that user   
            if (user.Money >= user.Level * 25)
            {
                ViewBag.StrengthGained = user.Level * 10;
                ViewBag.MoneySpended = user.Level * 25;
                ViewBag.ExpGained = user.Level * 5;
                user.Strength += user.Level * 10; // increase his str by level * 10
                user.Money -= user.Level * 25; // decrease his money
                user.Experience += user.Level * 5;
                UserManager.Update(user);
            }
            else
            {
                ViewBag.Money = "You don't have enough money";
            }
            return View("StrengthTired"); // go to view
        }
        [HttpPost]
        public ActionResult Squat()
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var user = UserManager.FindById(User.Identity.GetUserId()); // find that user   
            if (user.Money >= user.Level * 30)
            {
                ViewBag.StrengthGained = user.Level * 15;
                ViewBag.MoneySpended = user.Level * 30;
                ViewBag.ExpGained = user.Level * 5;
                user.Strength += user.Level * 15; // increase his str by level * 15
                user.Money -= user.Level * 30; // decrease his money
                user.Experience += user.Level * 5;
                UserManager.Update(user);
            }
            else
            {
                ViewBag.Money = "You don't have enough money";
            }
            return View("StrengthTired"); // go to view
        }
        [HttpPost]
        public ActionResult Dips()
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var user = UserManager.FindById(User.Identity.GetUserId()); // find that user   
            if (user.Money >= user.Level * 35)
            {
                ViewBag.StrengthGained = user.Level * 20;
                ViewBag.MoneySpended = user.Level * 35;
                ViewBag.ExpGained = user.Level * 5;
                user.Strength += user.Level * 20; // increase his str by level * 20
                user.Money -= user.Level * 35; // decrease his money
                user.Experience += user.Level * 5;
                UserManager.Update(user);
            }
            else
            {
                ViewBag.Money = "You don't have enough money";
            }
            return View("StrengthTired"); // go to view
        }
        [HttpPost]
        public ActionResult PullUp()
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var user = UserManager.FindById(User.Identity.GetUserId()); // find that user   
            if (user.Money >= user.Level * 10)
            {
                ViewBag.StrengthGained = user.Level * 10;
                ViewBag.MoneySpended = user.Level * 10;
                ViewBag.ExpGained = user.Level * 5;
                user.Strength += user.Level * 10; // increase his str by level* 17
                user.Money -= user.Level * 10; // decrease his money
                user.Experience += user.Level * 5;
                UserManager.Update(user);
            }
            else
            {
                ViewBag.Money = "You don't have enough money";
            }
            return View("StrengthTired"); // go to view
        }

        [HttpPost]
        public ActionResult Biceps()
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var user = UserManager.FindById(User.Identity.GetUserId()); // find that user   
            if (user.Money >= user.Level * 5)
            {
                ViewBag.StrengthGained = user.Level * 5;
                ViewBag.MoneySpended = user.Level * 5;
                ViewBag.ExpGained = user.Level * 5;

                user.Strength += user.Level * 5; // increase his str by level*5
                user.Money -= user.Level * 5; // decrease his money
                user.Experience += user.Level * 5;
                UserManager.Update(user);
            }
            else
            {
                ViewBag.Money = "You don't have enough money";
            }
            return View("StrengthTired"); // go to view
        }
	}
}
