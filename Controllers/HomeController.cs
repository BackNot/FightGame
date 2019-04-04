using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using HooliganGame.Models;
using PagedList;
using HooliganGame.Infrastructure;

namespace HooliganGame.Controllers
{
   
    public class HomeController : Controller
    {
        ApplicationDbContext context;
        IList<int> numbers;

        public HomeController()
        {
           
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Help()
        {

            return View();
        }


      
        public ActionResult MyProfile()
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            if (User.Identity.GetUserId() != null)
            {
                var user = UserManager.FindById(User.Identity.GetUserId());
                
                ViewBag.role = user.Roles.SingleOrDefault().Role.Name;

                return View(user);
            }
            else return RedirectToAction("Login","Account");
        }
        [Authorize]
        
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult MyProfile(string submit)
        {
             var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
          
                var user = UserManager.FindById(User.Identity.GetUserId());

                if (user.Experience >= 1000) // if user has more than 1000exp allow him to gain level
                {
                    user.Level += 1;
                    user.Experience -= 1000;
                    UserManager.Update(user);
                }
                else
                {
                    ViewBag.GainLevel = "You don't have 1000 exp";
                }
            return View(user);
        }
        public ActionResult Train()
        {

            return View();
        }
        
        public ActionResult Roulette()
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            if (User.Identity.GetUserId() != null)
            {
                System.Web.HttpContext.Current.Session["numbers"] = null; // clear the session from past 
                var user = UserManager.FindById(User.Identity.GetUserId()); // get the current user
                ViewBag.UserMoney = user.Money;
                return View();
            }
            else return RedirectToAction("Login", "Account");
           
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Roulette(int id)
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var user = UserManager.FindById(User.Identity.GetUserId()); // get the current user
            ViewBag.UserMoney = user.Money;
            ViewBag.NotEnoughMoney = false; // if user don't have enough money we will turn this to true
            ViewBag.MaxBet = false; // if user has made too big bet ( example 110$ on one number )
            bool betNotAllowed = false; // this will tell the method if bet is too high
            if (id == 999) // 999 id means that user has rotated the wheel
            {
                 if (User.Identity.GetUserId() != null)
                 {
                     List<int> chosenNumbers = System.Web.HttpContext.Current.Session["numbers"] as List<int>; // get the numbers
                     if (chosenNumbers != null) // if player has made a bet
                     {
                         int totalBet = 0;
                         var groupedNumbers = chosenNumbers.GroupBy(i => i); // sorted numbers
                         foreach (var item in groupedNumbers)
                         {
                             totalBet += item.Count()*10; // add the value of the bet to a total Integer
                         }
                         if (user.Money >= totalBet) // if user has enough money
                         {
                           
                             Random rand = new Random(); 
                             int winningNumber = rand.Next(0, 36);
                             ViewBag.WinningNumber = winningNumber;
                             if (chosenNumbers.Contains(winningNumber)) // if user has guessed the number
                             {
                                 System.Web.HttpContext.Current.Session["numbers"] = null; // clear the session from past bets
                                 int winningBet = 0;
                                 int losingBet = 0;
                                 foreach (var item in groupedNumbers)
                                 {
                                     if (item.Key == winningNumber) winningBet = item.Count();
                                     if (item.Key != winningNumber) losingBet += item.Count();
                                 }
                                 

                                 user.Money += winningBet * 10 *  37; // Add the reward to the user
                                 user.Money -= losingBet * 10; // remove all the others losing bet
                                 UserManager.Update(user);
                                 ViewBag.MoneyWon = winningBet * 10 * 37;
                                 return View(); // Congrats javascript
                             }
                             else // if wrong number
                             {
                                 int countBet = 0;
                                 foreach (var item in groupedNumbers)
                                 {
                                     if (item.Key != winningNumber)  countBet+= item.Count();
                                 }
                                 ViewBag.CountBet = countBet * 10;
                                 user.Money -= countBet * 10; // remove the money from the user that lost
                                 UserManager.Update(user);
                                 System.Web.HttpContext.Current.Session["numbers"] = null; // clear the session from past bets
                                 ViewBag.MoneyWon = 0; // user won 0 money
                                 return View();
                             }
                         }
                         else
                         {
                             ViewBag.NotEnoughMoney = true;
                             return View(); // if user does not has enough money
                         }
                          
                     }
                     else return View(); // if no bet => go back to view
                 }
                return View();
            }
            else // if user has pressed a number
            {
                numbers = new List<int>(); // create empty list that will hold the first number
                List<int> chosenNumbers = System.Web.HttpContext.Current.Session["numbers"] as List<int>;

                // Store the chosen number in a session using a List 
                if (chosenNumbers != null) // if we already have made atlest 1 bet
                {
                    foreach (var num in chosenNumbers.GroupBy(i=>i)) // Check for maximum bet
                    {

                        if ((num.Count() == 10)&&(id==num.Key)) // if num.count equals 10 and id is the same number don't allow the bet
                        {
                            ViewBag.MaxBet = true;
                            betNotAllowed = true; 
                        }
                    }
                    if (!betNotAllowed) // if bet is allowed 
                    {
                        chosenNumbers.Add(id); // add the current number to the list and pass it 
                    }
                    return View(chosenNumbers); // to the view
                }
                else
                {
                    numbers.Add(id); // if we are here then this is the first bet we make => add the number to the empty list
                    System.Web.HttpContext.Current.Session["numbers"] = numbers; // give this list to the Session that will be received later
                    return View(numbers); // return the list to the view
                }
            }
        }
        [HttpGet]
        public ActionResult Fight(int? page,int? sortOrder)
        {
            ViewBag.CurrentSort = sortOrder;
            var context = new ApplicationDbContext();
            var users = from user in context.Users
                                          select user;
            switch(sortOrder)
            {
                case 1: users = users.OrderBy(obj=> obj.Level);break; // LEVEL 
                case 2: users = users.OrderBy(obj => obj.Speed); break; // SPEED
                case 3: users = users.OrderBy(obj => obj.Strength); break; //Strength
                default: 
                    users = users.OrderBy(obj => obj.UserName);
                    break;
            }
            int pageSize = 6;
            int pageNumber = (page ?? 1);
            return View(users.ToPagedList(pageNumber, pageSize));
        }
        [Authorize]
       [HttpPost]
     public ActionResult Fight(string id) //TODO
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var attacker = UserManager.FindById(User.Identity.GetUserId()); // get the current user
            var defender = UserManager.FindById(id); // user that is being attacked 
            int attackerPoints = 0;  // points in the fight - this is how is going to be calculated who wins
            int defenderPoints = 0; // points of defender
            double attackerBonus = 0;
            double defenderBonus = 0;
           // BEGIN FIGHT
           if(attacker.Strength>defender.Strength)
           {
               if (attacker.Strength - defender.Strength <= 100) attackerPoints += 2;
               else if (attacker.Strength - defender.Strength <= 200) attackerPoints += 3;
               else if (attacker.Strength - defender.Strength <= 300) attackerPoints += 4;
               else if (attacker.Strength - defender.Strength <= 400) attackerPoints += 5;
               else if (attacker.Strength - defender.Strength <= 500) attackerPoints += 7;
               else if (attacker.Strength - defender.Strength <= 800) attackerPoints += 10;
               else if (attacker.Strength - defender.Strength <= 999) attackerPoints += 20;
           }
           if(attacker.Strength<defender.Strength)
           {
               if (defender.Strength - attacker.Strength >= 999) defenderPoints += 20;
               else if (defender.Strength - attacker.Strength >= 800) defenderPoints += 10;
               else if (defender.Strength - attacker.Strength >= 500) defenderPoints += 7;
               else if (defender.Strength - attacker.Strength >= 400) defenderPoints += 5;
               else if (defender.Strength - attacker.Strength >= 300) defenderPoints += 4;
               else if (defender.Strength - attacker.Strength >= 200) defenderPoints += 3;
               else if (defender.Strength - attacker.Strength >= 100) defenderPoints += 2;
               else defenderPoints += 1;
           }
           // Speed
           if (attacker.Speed > defender.Speed)
           {
               if (attacker.Speed - defender.Speed <= 100) attackerPoints += 2;
               else if (attacker.Speed - defender.Speed <= 200) attackerPoints += 3;
               else if (attacker.Speed - defender.Speed <= 300) attackerPoints += 4;
               else if (attacker.Speed - defender.Speed <= 400) attackerPoints += 5;
               else if (attacker.Speed - defender.Speed <= 500) attackerPoints += 7;
               else if (attacker.Speed - defender.Speed <= 800) attackerPoints += 10;
               else if (attacker.Speed - defender.Speed <= 999) attackerPoints += 20;
           }
           if (attacker.Speed < defender.Speed)
           {
               if (defender.Speed - attacker.Speed >= 999) defenderPoints += 20;
               else if (defender.Speed - attacker.Speed >= 800) defenderPoints += 10;
               else if (defender.Speed - attacker.Speed >= 500) defenderPoints += 7;
               else if (defender.Speed - attacker.Speed >= 400) defenderPoints += 5;
               else if (defender.Speed - attacker.Speed >= 300) defenderPoints += 4;
               else if (defender.Speed - attacker.Speed >= 200) defenderPoints += 3;
               else if (defender.Speed - attacker.Speed >= 199) defenderPoints += 2;
               else defenderPoints += 1;
           }

           if((attacker.Speed>defender.Speed)&&(attacker.Strength<defender.Strength))
           { // CALCULATE BONUS (only in special cases is given bonus for ex. if user has bigger speed and lower str or otherwise)
               
               if (attacker.Speed > attacker.Strength)
                   attackerBonus = (1000 - attacker.Speed - attacker.Strength) / 100;
               else attackerBonus = (1000 - attacker.Strength - attacker.Speed) / 100;
               if (defender.Speed > defender.Strength)
                   defenderBonus = (1000 - defender.Speed - defender.Strength) / 100;
               else defenderBonus = (1000 - defender.Strength - defender.Speed) / 100;
           }
           if ((attacker.Speed < defender.Speed) && (attacker.Strength > defender.Strength))
           { // CALCULATE BONUS (only in special cases is given bonus for ex. if user has bigger speed and lower str or otherwise)
               if (attacker.Speed > attacker.Strength)
                   attackerBonus = (1000 - attacker.Speed - attacker.Strength) / 100;
               else attackerBonus = (1000 - attacker.Strength - attacker.Speed) / 100;
               if (defender.Speed > defender.Strength)
                   defenderBonus = (1000 - defender.Speed - defender.Strength) / 100;
               else defenderBonus = (1000 - defender.Strength - defender.Speed) / 100;
           }
           int totalPointsAttacker = attackerPoints + Convert.ToInt32(Math.Ceiling(attackerBonus));
           int totalPointsDefender = defenderPoints + Convert.ToInt32(Math.Ceiling(defenderBonus));
           int totalPoints = totalPointsAttacker + totalPointsDefender;
           if(totalPointsAttacker>totalPointsDefender)
           {
               ViewBag.Result = "won";
               if(attacker.Level<=defender.Level) // if attacker is smaller level give him more bonuses
               {
                   attacker.Money += attacker.Level * 50;
                   attacker.Speed += attacker.Level * 10;
                   attacker.Strength += attacker.Level * 10;
                   attacker.Experience += attacker.Level * 10;
                   attacker.Fights += 1;
                   attacker.FightsWon += 1;
                   defender.Fights += 1;
                   defender.Money -= 100; // withdraw 100$ from defender who lost the battle
                   ViewBag.GainedMoney = attacker.Level * 50;
                   ViewBag.GainedSpeed = attacker.Level * 10;
                   ViewBag.GainedStrength = attacker.Level * 10;

                   ViewBag.GainedExp = attacker.Level * 10;
               }
               else
               { 
                   attacker.Money += attacker.Level * 25;
                   attacker.Speed += attacker.Level * 5;
                   attacker.Strength += attacker.Strength * 5;
                   attacker.Experience += attacker.Level * 5;
                   attacker.Fights += 1;
                   attacker.FightsWon += 1;
                   defender.Fights += 1;
                   defender.Money -= 50; // take 50$ from defender
                   ViewBag.GainedMoney = attacker.Level * 25;
                   ViewBag.GainedSpeed = attacker.Level * 5;
                   ViewBag.GainedStrength = attacker.Level * 5;

                   ViewBag.GainedExp = attacker.Level * 5;
                  
               }
               UserManager.Update(attacker);
               UserManager.Update(defender);
           }
           else 
           {
               ViewBag.Result = "lost";
               attacker.Money -= attacker.Level * 15;
               attacker.Strength -= attacker.Level * 2;
               attacker.Speed -= attacker.Level * 2;
               attacker.Experience -= attacker.Level * 3;
               attacker.Fights += 1;
               defender.FightsWon += 1;
               defender.Fights += 1;
               defender.Experience += attacker.Level * 3;
               defender.Money += 100;
               UserManager.Update(attacker);
               UserManager.Update(defender);
               ViewBag.LostMoney = attacker.Level * 15;
               ViewBag.LostSpeed = attacker.Level * 2;
               ViewBag.LostStrength = attacker.Level * 2;
               ViewBag.LostExp = attacker.Level*3;
           }
           ViewBag.AttackerPoints = totalPointsAttacker;
           ViewBag.DefenderPoints = totalPointsDefender;
           ViewBag.TotalPoints = totalPoints;
           
           // END FIGHT
            return View("FightEnd");
        }
      
        
        public ActionResult Ranking(string sortOrder)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var ranks = from obj in context.Users
                        select obj;
            switch(sortOrder)
            {
                case "LevelHigh": ranks = ranks.OrderByDescending(obj => obj.Level); break;
                case "LevelLow": ranks = ranks.OrderBy(obj => obj.Level); break;
                case "MoneyHigh": ranks = ranks.OrderByDescending(obj => obj.Money); break;
                case "MoneyLow": ranks = ranks.OrderBy(obj => obj.Money); break;
                case "Fights": ranks = ranks.OrderByDescending(obj => obj.Fights); break;
                case "FightsWon": ranks = ranks.OrderByDescending(obj => obj.FightsWon); break;
                case "Str": ranks = ranks.OrderByDescending(obj => obj.Strength); break;
                case "Speed": ranks = ranks.OrderByDescending(obj => obj.Speed); break;
                default: ranks = ranks.OrderByDescending(obj => obj.Level); break;

            }
            return View(ranks.ToList());
        }
        public ActionResult AdminPanel()
        {
            return View();
        }

        public ActionResult Information()
        {
            return View();
        }
    }
}