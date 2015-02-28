using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GamblingNewbies.Models;
using System.Web.Script.Serialization;
using Microsoft.AspNet.Identity;


namespace GamblingNewbies.Controllers
{
    public class GameController : Controller
    {
        private GamblingDBContext db = new GamblingDBContext();
        const string choice1 = "All Game";
        const string choice2 = "Open Game";
        const string choice3 = "My Game";
        public const int MaxGameNum = 5; // the max number of game one can play at the same time.
        //
        // GET: /Game/
        public ActionResult Index()
        {
            //user is active, update the LastActive
            string currentUserID = User.Identity.GetUserId();
            UpdateLastActiveDate(currentUserID);

            //var users = from u in db.Users
            //              select u;
            
            // Keep Below //
            if (TempData["TableErrorLabel"] != null)
            {
                ViewBag.TableErrorLabel = TempData["TableErrorLabel"].ToString();
            }
            // Keep Above //
            ViewBag.choice1 = choice1;
            ViewBag.choice2 = choice2;
            ViewBag.choice3 = choice3;

            return View();
        }

        public string UpdateOnlineUsers()
        {
            //Let's edit this
            List<User> userlist = null;
            using (var db = new GamblingDBContext())
            {
                var users = from u in db.Users
                            select u;
                userlist = users.ToList();
            }
           
           
            int length = userlist.Count();
            
            for (int i = 0; i < length; i++)
            {
                if (DateTime.Compare(DateTime.Now, userlist[i].LastActiveTime.AddSeconds(30)) > 0)
                {
                    userlist.Remove(userlist[i]);
                    i--;// when list is cut, we need to amend the index and the length of list
                    length = length - 1;
                }
            }
            var jsonSerialiser = new JavaScriptSerializer();
            string json = jsonSerialiser.Serialize(userlist);
            return json;
            //Let's edit this
        }

        public void UpdateLastActiveDate(string userID) 
        {
            if (userID != null)
            {
                using (GamblingDBContext db = new GamblingDBContext())
                {
                    var users = from u in db.Users
                                where u.UserID == userID
                                select u;

                   
                        if ( users != null && users.Count() == 1)
                        {
                            User currentUser = users.ToList().First();
                            currentUser.LastActiveTime = DateTime.Now;
                            db.SaveChanges();
                        }
                    

                }
            }
        }

     

        public string allTableInfo(string id)
        {
            string choice = id;
            List<Table> alltable = null;
            using(GamblingDBContext db = new GamblingDBContext())
            {
                if(choice == choice1)
                {
                    var tables = from q in db.Tables
                                 orderby q.Ptime descending
                                 select q;
                    alltable = tables.ToList();
                }
                else if(choice == choice2)
                {
                    var tables = from q in db.Tables
                                 where String.IsNullOrEmpty(q.Username1) || String.IsNullOrEmpty(q.Username2)
                                 select q;
                    alltable = tables.ToList();
                }
                else
                {
                    var tables = from q in db.Tables
                                 where q.Username1 == User.Identity.Name || q.Username2 == User.Identity.Name
                                 select q;
                    alltable = tables.ToList();
                }
               
                var jsonSerialiser = new JavaScriptSerializer();
                string json = jsonSerialiser.Serialize(alltable);
                return json;
            }           
        }
        

        public ActionResult NewGame()
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["TableErrorLabel"] = "Please login in order to play.";
                return RedirectToAction("Index", "Game");
            }

            if (TempData["Message"] != null)
            {
                ViewBag.MessageLabel = TempData["Message"].ToString();
            }
            if (TempData["Message1"] != null)
            {
                ViewBag.MessageLabel1 = TempData["Message1"].ToString();
            }
            if (TempData["BetAmount"] != null)
            {
                ViewBag.BetAmount = TempData["BetAmount"];
            }
            if (TempData["BetAmount1"]!= null)
            {
                ViewBag.BetAmount1 = TempData["BetAmount1"];
            }
            return View();
        }

        public ActionResult PostNewGame(string username, string amount, string reserve, string title)
        {
            
            if (string.IsNullOrEmpty(amount)|| string.IsNullOrEmpty(title))
            {
                TempData["Message"] = "Please fill in the title and the mount you want to bet";
                return RedirectToAction("NewGame","Game");
            }

            // find all games this user joined but have finished yet
            List<Table> joinedGame = null;
            using(GamblingDBContext db = new GamblingDBContext())
            {
                var temp = from q in db.Tables
                           where (q.Username1 == username || q.Username2 == username) && q.Winner == null
                           select q;
                joinedGame = temp.ToList();
            }
            if(joinedGame.Count() >= MaxGameNum)
            {
                TempData["TableErrorLabel"] = String.Format("You have joined {0} games. Please finish these games before you start a new one.", MaxGameNum);
                return RedirectToAction("Index", "Game");
            }


            User thisUser = new User();
            using (GamblingDBContext db = new GamblingDBContext())
            {
                var temp = from q in db.Users
                               where q.Username == username
                               select q;
                thisUser = temp.First();
            }

            using (GamblingDBContext db = new GamblingDBContext())
            {
                Table newTable = new Table();
                newTable.Username1 = username;
                int temp = 0;
                if(Int32.TryParse(amount, out temp) && (temp > 0))
                {
                    newTable.Cost = temp;
                    //one can only set bet amount less than his coins
                    if(thisUser.Coins < temp)
                    {
                        TempData["BetAmount1"] = "Sorry, you do not have that amount of coins to bet";
                        return RedirectToAction("NewGame", "Game");
                    }
                   
                }
                else
                {
                    TempData["BetAmount"] = "Please choose a positive interger as your bet amount";
                    return RedirectToAction("NewGame","Game");
                }

                newTable.Name = title;
                newTable.Username2 = reserve;
                if(username == reserve)
                {
                    TempData["Message1"] = "Buddy, you can not reserve this table for youself";
                    return RedirectToAction("NewGame", "Game");
                }
                newTable.Status = 0;
                newTable.Choice1 = 0;
                newTable.Choice2 = 0;
                newTable.Winner = null;
                newTable.User1Wins = 0;
                newTable.User2Wins = 0;
                newTable.TotalRounds = 0;
                newTable.Ptime = DateTime.Now;

                try
                {
                    db.Tables.Add(newTable);
                    db.SaveChanges();
                    var usersTable = from item in db.Tables
                                       where item.Username1 == username
                                       select item;
                    Table newest = usersTable.ToList().Last();
                    return Redirect("/Table/" + newest.ID.ToString());
                }
                catch (Exception ex)
                {
                    TempData["TableErrorLabel"] = "Sorry something blow up in the database and game creation failed.";
                    return RedirectToAction("Index", "Game");
                }
                
            }
           
        }
	}
}