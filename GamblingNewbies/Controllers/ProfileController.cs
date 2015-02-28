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

namespace GamblingNewbies.Controllers
{
    public class ProfileController : Controller
    {
        private GamblingDBContext db = new GamblingDBContext();

        // GET: Profile
        public ActionResult Index()
        {
            //user is active, update the LastActive
            string currentUserID = User.Identity.GetUserId();
            UpdateLastActiveDate(currentUserID);
            return Redirect("/Home/Index");
        }

       /// <summary>
       /// Show Username, coins and ranking by ViewBag
       /// </summary>
       /// <param name="id"></param>
       /// <returns></returns>
        public ActionResult DearUser(string id) // id is userName
        {
            //user is active, update the LastActive
            string currentUserID = User.Identity.GetUserId();
            UpdateLastActiveDate(currentUserID);
            if(id == null)
            {
                return Redirect("/Profile/Index");
            }

            List<User> personlist = new List<User>();
            List<User> ranklist = new List<User>();
            List<Message> messages = new List<Message>();
            using (GamblingDBContext db = new GamblingDBContext())
            {
                personlist = (from q in db.Users
                         where q.Username == id
                             select q).ToList();
           
                ranklist = (from q in db.Users
                 orderby q.Coins descending
                 select q).ToList();

                messages = (from q in db.Wall
                               where q.UserName == id
                               orderby q.ReplyTime descending
                               select q).ToList();
            }
            foreach(Message x in messages)
            {
                x.Text = Server.HtmlDecode(x.Text);
            }

            if (personlist.Count()!=1)
            {
                return Redirect("/Profile/Index");
            }

            if(TempData["MessageLabel"]!= null)
            {
                ViewBag.warning = TempData["MessageLabel"].ToString();
            }
            if(TempData["MessageOnTheWall"]!= null)
            {
                ViewBag.message = TempData["MessageOnTheWall"].ToString();
            }
            
            ViewBag.name = personlist[0].Username;
            ViewBag.coin = personlist[0].Coins;
            ViewBag.winTimes = personlist[0].WinTimes;
            ViewBag.lossTimes = personlist[0].LossTimes;
            ViewBag.userDescription = Server.HtmlDecode(personlist[0].UserDiscription);

            if (personlist[0].Username == User.Identity.GetUserName())
            {
                ViewBag.isCurrentUser = true;
            }

            ViewBag.userRank = ranklist.FindIndex(x => x.Username == id) + 1;

            return View(messages);

        }

        [ValidateInput(false)]
        public ActionResult PostNewMessage(string message, string user_name)
        {
            //user is active, update the LastActive
            string currentUserID = User.Identity.GetUserId();
            UpdateLastActiveDate(currentUserID);
            string parsedMessage = Server.HtmlEncode(message);
            Message newMessage = new Message();
            string post_user_name = "null";
            if (String.IsNullOrEmpty(User.Identity.Name))
            {
                TempData["MessageLabel"] = "You are not logged in!";
                return RedirectToAction("DearUser", "Profile", new { id = user_name });
            }
            else { post_user_name = User.Identity.Name; }
            //check if logged in;
            var users = from u in db.Users
                        where u.Username == user_name 
                        select u;
            int usercount = users.Count();
            if (usercount != 1)
            {
                TempData["MessageLabel"] = "You are not logged in!";
                return RedirectToAction("DearUser", "Profile", new { id = user_name});
            }

            if (parsedMessage.Length == 0) 
            {
                TempData["MessageLabel"] = "Title and text can not be empty!";
                return RedirectToAction("DearUser", "Profile", new { id = user_name});
            }         

            if (parsedMessage.Length > 150)
            {
                TempData["MessageLabel"] = "Your thread exceeds the 150 characters length.";
                TempData["MessageOnTheWall"] = parsedMessage;
                return RedirectToAction("DearUser", "Profile", new { id = user_name });
            }

            //update the Wall table;
            newMessage.UserName = user_name;
            newMessage.PostUserName = post_user_name;
            newMessage.ReplyTime = DateTime.UtcNow;
            newMessage.Text = parsedMessage;

            db.Wall.Add(newMessage);
            db.SaveChanges();

            return RedirectToAction("DearUser", "Profile", new { id = user_name });
        }

        public ActionResult EditUserDescription(string username)
        {
            //user is active, update the LastActive
            string currentUserID = User.Identity.GetUserId();
            UpdateLastActiveDate(currentUserID);
            if (username.Equals(User.Identity.GetUserName()))
            {
                ViewBag.userName = username;
                if (TempData["MessageLabel"] != null)
                {
                    ViewBag.MessageLabel = TempData["MessageLabel"].ToString();
                }
                return View();
            }
            else
            {
                return RedirectToAction("DearUser", "Profile", new { id = User.Identity.GetUserName()});
            }
        }

        [ValidateInput(false)]
        public ActionResult UpdateUserDescription(string username, string text)
        {
            //user is active, update the LastActive
            string currentUserID = User.Identity.GetUserId();
            UpdateLastActiveDate(currentUserID);

            if (text.Length == 0)
            {
                TempData["MessageLabel"] = "Updates can not be empty!";
                return RedirectToAction("EditUserDescription", "Profile", new { username = username});
            }
            else if (text.Length > 200)
            {
                TempData["MessageLabel"] = "Updates can not exceed 200 characters!";
                return RedirectToAction("EditUserDescription", "Profile", new { username = username });
            }

            else
            {
                using (GamblingDBContext db = new GamblingDBContext())
                {
                    var users = from u in db.Users
                                where u.Username == username
                                select u;
                    if (users.Count() == 1)
                    {
                        User currentUser = users.ToList().First();
                        currentUser.UserDiscription = Server.HtmlEncode(text);
                        db.SaveChanges();
                    }

                }
                return RedirectToAction("DearUser", "Profile", new { id = username });
            }
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


                    if (users != null && users.Count() == 1)
                    {
                        User currentUser = users.ToList().First();
                        currentUser.LastActiveTime = DateTime.Now;
                        db.SaveChanges();
                    }


                }
            }
        }
    }
}
