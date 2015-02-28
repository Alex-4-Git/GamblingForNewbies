using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GamblingNewbies.Models;
using Microsoft.AspNet.Identity;

// This controller and associated views primarily written by Vangie & Jane.
namespace GamblingNewbies.Controllers
{

    public class TableController : Controller
    {
        private GamblingDBContext db = new GamblingDBContext();

        public void UpdateLastActiveDate(string userID)
        {
            if (userID != null)
            {
                using (GamblingDBContext db = new GamblingDBContext())
                {
                    var users = from u in db.Users
                                where u.UserID == userID
                                select u;


                    if (users != null && users.Count() == 1)
                    {
                        User currentUser = users.ToList().First();
                        currentUser.LastActiveTime = DateTime.Now;
                        db.SaveChanges();
                    }


                }
            }
        }
        public ActionResult Join(string id)
        {
            //user is active, update the LastActive
            string currentUserID = User.Identity.GetUserId();
            UpdateLastActiveDate(currentUserID);
            int tableid;
            string username = User.Identity.Name;
            User thisuser = getUserFromUsername(username);
            Table thistable = getTableFromID(Convert.ToInt16(id));

            if (String.IsNullOrEmpty(User.Identity.Name))
            {
                TempData["TableErrorLabel"] = "You need to be logged in to join a game.";
                return RedirectToAction("Index", "Game");
            }

            // one can join GameController.MaxGameNum number of games at a time
            List<Table> joinedGame = null;
            using (GamblingDBContext db = new GamblingDBContext())
            {
                var temp = from q in db.Tables
                           where (q.Username1 == username || q.Username2 == username) && q.Winner == null
                           select q;
                joinedGame = temp.ToList();
            }
            if (joinedGame.Count() >= GameController.MaxGameNum)
            {
                TempData["TableErrorLabel"] = String.Format("You have joined {0} games. Please finish these games before you start a new one.", GameController.MaxGameNum);
                return RedirectToAction("Index", "Game");
            }

            //table should be valid and one should have enough coins
            if (int.TryParse(id, out tableid))
            {
                if (!validTable(tableid))
                {
                    TempData["TableErrorLabel"] = "That's an invalid Table ID.";
                    return RedirectToAction("Index", "Game");
                }
                if(thisuser.Coins < thistable.Cost)
                {
                    TempData["TableErrorLabel"] = "You can only join the game if you coins exceeds the Bet Amount";
                    return RedirectToAction("Index", "Game");
                }
                addUser(tableid, username);
                return RedirectToAction("Index", "Table", new { id = id });
            }

            TempData["TableErrorLabel"] = "Trying to enter an invalid table!";
            return RedirectToAction("Index", "Game");

            //TempData["TableErrorLabel"] = "Something went wrong so we redirected you here.";
            //return RedirectToAction("Index", "Game");
        }

        public static User getUserFromUsername(string username)
        {
            User theuser = new User();
            using (var dbx = new GamblingDBContext())
            {
                var users = from u in dbx.Users
                            where u.Username == username
                            select u;
                if (users.Count() >= 1)
                {
                    theuser = users.First();
                }
                else { theuser = null; }
            }
            return theuser;
        }
        public static Boolean addUser(int tableid, string username)
        {
            Table thetable = new Table();
            Boolean joined = false;
            using (var dbx = new GamblingDBContext())
            {
                thetable = dbx.Tables.Find(tableid);

                if (String.IsNullOrEmpty(thetable.Username1) && thetable.Username2 != username)
                {
                    thetable.Username1 = username;
                    joined = true;
                }
                if (String.IsNullOrEmpty(thetable.Username2) && thetable.Username1 != username)
                {
                    thetable.Username2 = username;
                    joined = true;
                }
                dbx.SaveChanges();
            }
            return joined;
        }
        public static Table getTableFromID(int tableid)
        {
            Table thetable = new Table();
            using (var dbx = new GamblingDBContext())
            {
                thetable = dbx.Tables.Find(tableid);
            }
            return thetable;
        }

        // GET: Tables
        public ActionResult Index(string id)
        {
            //user is active, update the LastActive
            string currentUserID = User.Identity.GetUserId();
            UpdateLastActiveDate(currentUserID);
            int tableid;
            string username = User.Identity.Name;
            Table thistable;
            User thisuser = getUserFromUsername(username);
            // Validate User
            if (thisuser == null)
            {
                TempData["TableErrorLabel"] = "You must be logged in to play!";
                return RedirectToAction("Index", "Game");
            }
            // Validate input ID
            if (int.TryParse(id, out tableid))
            {
                // Validate Table
                thistable = getTableFromID(tableid);
                if (thistable == null)
                {
                    TempData["TableErrorLabel"] = "You can't join that table!";
                    return RedirectToAction("Index", "Game");
                }
            }
            else { TempData["TableErrorLabel"] = "That's an invalid table!"; return RedirectToAction("Index", "Game"); }
            // Validate User belongs to Table
            if (thistable.Username1 != thisuser.Username && thistable.Username2 != thisuser.Username)
            {
                TempData["TableErrorLabel"] = "Sorry that table is closed!";
                return RedirectToAction("Index", "Game");
            }
            // Validate Table status
            if (thistable.Status == 1)
            {
                TempData["TableErrorLabel"] = "This game has already finished. Please see the Results.";
                return RedirectToAction("Index", "Game");
            }
            ///////////////// End of Validation

            ViewBag.Title = thistable.Name; // This is redundant but keep in case
            ViewBag.TableID = id;
            // Pass the choice of the current user
            if (thisuser.Username == thistable.Username1) { ViewBag.Choice = thistable.Choice1; }
            else { ViewBag.Choice = thistable.Choice2; }
            if (TempData["TableIndexError"] != null)
            {
                ViewBag.ErrorMessage = TempData["TableIndexError"].ToString();
            }

            return View(thistable);
        }

        [HttpPost]
        public ActionResult Index(string id, string option)
        {
            //user is active, update the LastActive
            string currentUserID = User.Identity.GetUserId();
            UpdateLastActiveDate(currentUserID);
            int tableid;
            int optionid;
            string username = User.Identity.Name;
            if (int.TryParse(id, out tableid) && int.TryParse(option, out optionid))
            {
                if (!validTable(tableid))
                {
                    TempData["TableErrorLabel"] = "I think that's an invalid table.";
                    return RedirectToAction("Index", "Game");
                }
                if (!UserinTable(tableid, username))
                {
                    TempData["TableErrorLabel"] = "You're not part of that table!";
                    return RedirectToAction("Index", "Game");
                }
                if (!hasEnoughMoney(tableid, username))
                {
                    TempData["TableIndexError"] = "You don't have enough money. :(";
                    return RedirectToAction("Index", "Table", new { id = id });
                }
                if (updateTableChoice(tableid, username, optionid))
                {
                    return RedirectToAction("Index", "Table", new { id = id });
                }
            }
            else { TempData["TableErrorLabel"] = "That's an invalid table!"; return RedirectToAction("Index", "Game"); }

            TempData["TableErrorLabel"] = "Something went wrong so we redirected you here.";
            return RedirectToAction("Index", "Game");
        }
        public static Boolean validTable(int tableid)
        {
            Table thetable = new Table();
            Boolean valid = false;
            using (var dbx = new GamblingDBContext())
            {
                thetable = dbx.Tables.Find(tableid);
                if (thetable != null) { valid = true; }
            }
            return valid;
        }
        public static Boolean UserinTable(int tableid, string username)
        {
            Table thetable = new Table();
            Boolean valid = false;
            using (var dbx = new GamblingDBContext())
            {
                thetable = dbx.Tables.Find(tableid);
                if (thetable.Username1 == username || thetable.Username2 == username) { valid = true; }
            }
            return valid;
        }
        public static Boolean hasEnoughMoney(int tableid, string username)
        {
            Table thetable = new Table();
            User theuser = new User();
            Boolean rich = false;
            using (var dbx = new GamblingDBContext())
            {
                thetable = dbx.Tables.Find(tableid);
                theuser = dbx.Users.SingleOrDefault(x => x.Username == username);
                if (thetable.Cost < theuser.Coins) { rich = true; }
            }
            return rich;
        }

        public static Boolean updateTableChoice(int tableid, string username, int option)
        {
            Table thetable = new Table();
            Boolean answer = true;
            Boolean completed = false;
            using (var dbx = new GamblingDBContext())
            {
                thetable = dbx.Tables.Find(tableid);

                if (thetable == null) { answer = false; }

                if (thetable.Username1 == username) { thetable.Choice1 = option; }
                else if (thetable.Username2 == username) { thetable.Choice2 = option; }
                else answer = false;

                if (thetable.Choice1 > 0 && thetable.Choice2 > 0 && thetable.Winner == null)
                {
                    // Determine the Winner
                    // 1 - scissor
                    // 2 - paper
                    // 3 - rock
                    if (thetable.Choice1 == 3 && thetable.Choice2 == 1) { 
                        thetable.Winner = thetable.Username1; 
                    }
                    else if (thetable.Choice1 == 3 && thetable.Choice2 == 2) { 
                        thetable.Winner = thetable.Username2;
                    }
                    else if (thetable.Choice1 == 2 && thetable.Choice2 == 1) { 
                        thetable.Winner = thetable.Username2;
                    }
                    else if (thetable.Choice1 == 2 && thetable.Choice2 == 3)
                    {
                        thetable.Winner = thetable.Username1;
                    }
                    else if (thetable.Choice1 == 1 && thetable.Choice2 == 2)
                    {
                        thetable.Winner = thetable.Username1;
                    }
                    else if (thetable.Choice1 == 1 && thetable.Choice2 == 3)
                    {
                        thetable.Winner = thetable.Username2;
                    }
                    else {
                        thetable.Winner = ""; 
                    }
                    completed = true;
                }
                dbx.SaveChanges();
            }

            // Pay the winner
            if (completed) { payWinner(tableid); }

            return answer;
        }
        public static void payWinner(int tableid)
        {
            Table thetable = getTableFromID(tableid);
            if (thetable.Winner == thetable.Username1)
            {
                addWin(thetable.Cost, thetable.Username1);
                addLoss(thetable.Cost, thetable.Username2);
            }
            else if (thetable.Winner == thetable.Username2)
            {
                addWin(thetable.Cost, thetable.Username2);
                addLoss(thetable.Cost, thetable.Username1);
            }
        }
        /// <summary>
        /// add coins to winner;
        /// add winTimes to winner;
        /// </summary>
        /// <param name="cost"></param>
        /// <param name="username"></param>
        public static void addWin(int cost, string username)
        {
            using (var dbx = new GamblingDBContext())
            {
                var users = from u in dbx.Users
                            where u.Username == username
                            select u;
                User user = users.First();
                user.Coins = user.Coins + cost;
                user.WinTimes = user.WinTimes + 1;
                dbx.SaveChanges();
            }
        }
        /// <summary>
        /// substract coins to losser
        /// add losstimes to losser
        /// </summary>
        /// <param name="cost"></param>
        /// <param name="username"></param>
        public static void addLoss(int cost, string username)
        {
            using (var dbx = new GamblingDBContext())
            {
                var users = from u in dbx.Users
                            where u.Username == username
                            select u;
                User user = users.First();
                user.Coins = user.Coins - cost;
                user.LossTimes = user.LossTimes + 1;
                dbx.SaveChanges();
            }
        }
        public string GetOpponentStatus(string id)
        {
            // Statuses:
            // invalid : table doesn't exist
            // opponent : missing opponent
            // choose : current player has not made choice
            // wait : opponent hasn't made choice yet
            // ok : go to results
            string username = User.Identity.Name;
            int tableid;
            Table thistable = new Table();
            if (int.TryParse(id, out tableid))
            {
                thistable = getTableFromID(tableid);
                if (thistable == null) { return "invalid"; }
            }
            else
            {
                return "invalid"; // Table does not exist?? Shouldn't happen.
            }

            if (thistable.Username1 != null && thistable.Username1 != "")
            {
                if (thistable.Username1 == username && thistable.Choice1 <= 0) { return "choose"; }
                else if (thistable.Choice1 <= 0) { return "wait"; }
            }
            else { return "opponent"; }
            if (thistable.Username2 != null && thistable.Username2 != "")
            {
                if (thistable.Username2 == username && thistable.Choice2 <= 0) { return "choose"; }
                else if (thistable.Choice2 <= 0) { return "wait"; }
            }
            else { return "opponent"; }

            return "ok";
        }

        public ActionResult Result(string id)
        {
            //user is active, update the LastActive
            string currentUserID = User.Identity.GetUserId();
            UpdateLastActiveDate(currentUserID);
            int tableid;
            string username = User.Identity.Name;
            Table thistable = new Table();
            User thisuser = getUserFromUsername(username);

            // Validate input ID
            if (int.TryParse(id, out tableid))
            {
                // Validate Table
                thistable = getTableFromID(tableid);
                if (thistable == null)
                {
                    TempData["TableErrorLabel"] = "Seems like you tried to view an invalid Result!";
                    return RedirectToAction("Index", "Game");
                }
            }
            else
            {
                TempData["TableErrorLabel"] = "That was an invalid Result page!";
                return RedirectToAction("Index", "Game");
            }

            // Test whether user is a guest, the winner, or the loser
            if (thistable.Winner == null)
            {
                TempData["TableErrorLabel"] = "The Results aren't available yet.";
                return RedirectToAction("Index", "Game");
            }
            if (thistable.Winner == thisuser.Username) { ViewBag.PersonalMessage = "Congratulations! You won!"; }
            else if (thistable.Winner == "") { ViewBag.PersonalMessage = "It's a Tie!"; }
            else if (thistable.Username1 == thisuser.Username || thistable.Username2 == thisuser.Username) { ViewBag.PersonalMessage = "Sorry, you lost!"; }
            else { ViewBag.PersonalMessage = "And the results are in!"; }

            return View(thistable);
        }

        public ActionResult Delete(string id)
        {
            //user is active, update the LastActive
            string currentUserID = User.Identity.GetUserId();
            UpdateLastActiveDate(currentUserID);
            int tableid;
            if (String.IsNullOrEmpty(User.Identity.Name))
            {
                TempData["TableErrorLabel"] = "Not logged in. How did you get here??";
                return RedirectToAction("Index", "Game");
            }
            string username = User.Identity.Name;
            if (int.TryParse(id, out tableid))
            {
                if (!validTable(tableid))
                {
                    TempData["TableErrorLabel"] = "You're trying to delete an invalid table.";
                    return RedirectToAction("Index", "Game");
                }
                if (!UserinTable(tableid, username))
                {
                    TempData["TableErrorLabel"] = "You can't delete a table you're not part of!";
                    return RedirectToAction("Index", "Game");
                }
                if (!deleteTable(tableid, username))
                {
                    TempData["TableErrorLabel"] = "You can't delete a table that's finished.";
                    return RedirectToAction("Index", "Game");
                }
                else {
                    TempData["TableErrorLabel"] = "Table was successfully deleted.";
                    return RedirectToAction("Index", "Game");
                }

            }
            else { TempData["TableErrorLabel"] = "Invalid table alert!"; return RedirectToAction("Index", "Game"); }

            //TempData["TableErrorLabel"] = "Something went wrong so we redirected you here.";
            //return RedirectToAction("Index", "Game");
        }

        public static Boolean deleteTable(int tableid, string username)
        {
            Boolean deleted = false;
            Table thetable = new Table();
            User theuser = new User();
            using (var dbx = new GamblingDBContext())
            {
                thetable = dbx.Tables.Find(tableid);
                theuser = dbx.Users.SingleOrDefault(x => x.Username == username);
                if (thetable.Winner == null)
                {
                    dbx.Tables.Remove(thetable);
                    deleted = true;
                }
                dbx.SaveChanges();
            }
            return deleted;
        }
    }
}
