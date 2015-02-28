using GamblingNewbies.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

// This controller and associated views is primarily written by Vangie.
namespace GamblingNewbies.Controllers
{
    public class HomeController : Controller
    {
        private GamblingDBContext mydb = new GamblingDBContext();
        private const int TOP_RANKS = 3;

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
        public ActionResult Index()
        {
            //user is active, update the LastActive
            string currentUserID = User.Identity.GetUserId();
            UpdateLastActiveDate(currentUserID);
            // Pull the Latest Announcement
            List<Thread> threads = null;
            using (var db = new GamblingDBContext())
            {
                threads = (from t in db.Threads
                              where t.SectionID == 1
                              select t).ToList();
            }
            
            List<Post> posts = null;
            Post thepost = new Post();
            foreach (var x in threads)
            {
                using (var db = new GamblingDBContext())
                {
                    posts = (from p in db.Posts
                                 where p.ThreadID == x.ID
                                 orderby p.ReplyTime ascending
                                 select p).ToList();
                }
                int postcount = posts.Count();
                if (postcount <= 0) { continue; }
                if (thepost.ID == 0) {
                    thepost = posts.First();
                    continue;
                }
                var currpost = posts.First();
                // If currpost is earlier
                if (DateTime.Compare(currpost.ReplyTime, thepost.ReplyTime) > 0) {
                    thepost = currpost;
                }
            }
            if (thepost.ID != 0) { 
            ViewBag.LatestTitle = Server.HtmlDecode(mydb.Threads.Find(thepost.ThreadID).Title);
            ViewBag.LatestMessage = Server.HtmlDecode(thepost.Text);
            ViewBag.LatestUser = thepost.UserID;
            }
            else { ViewBag.LatestTitle = "No Announcements Yet!"; }
            List<User> y = null;
            using (var db = new GamblingDBContext())
            {
                y = (from u in db.Users
                    where u.UserID == thepost.UserID
                    select u).ToList();
            }
            
            if (y.Count() == 1)
            {
                ViewBag.LatestUser = y.First().Username;
            }

            // Pull the top three ranking
            List<User> userRank = null;

            using (var db = new GamblingDBContext())
            {
                userRank = (from item in db.Users
                            orderby item.Coins descending
                            select item).ToList();
                
            }
            
            // get the userRankList that could be displayed in home index page
            List<User> userRankList = userRank;

            if (userRank.Count() > TOP_RANKS)
            {
                userRankList = userRank.GetRange(0, TOP_RANKS);
            }

            ViewBag.UserRankList = userRankList;
            ViewBag.TopRanks = userRankList.Count();
            if (TempData["Registered"] != null)
            {
                ViewBag.RegisteredMessage = TempData["Registered"].ToString();
            }
            return View();
        }

        public ActionResult About()
        {
            //user is active, update the LastActive
            string currentUserID = User.Identity.GetUserId();
            UpdateLastActiveDate(currentUserID);
            ViewBag.Message = "Honestly, I don't think this string needs to be in the controller.";
            return View();
        }

        //public ActionResult Contact()
        //{
        //    ViewBag.Message = "Your contact page.";
        //    return View();
        //}
    }
}