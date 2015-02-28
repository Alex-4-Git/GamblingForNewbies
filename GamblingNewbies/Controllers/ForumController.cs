using GamblingNewbies.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

///This controller (and associated views) is primarily written by Xiaoyu.
namespace GamblingNewbies.Controllers
{
    public class ForumController : Controller
    {
        
        //
        // GET: /Forum/

        /// <summary>
        /// show all the sections to the user;
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            //user is active, update the LastActive
            string currentUserID = User.Identity.GetUserId();
            UpdateLastActiveDate(currentUserID);

            using (GamblingDBContext db = new GamblingDBContext())
            {
                return View(db.Sections.ToList());
            }

        }

        /// <summary>
        /// show all the threads in section id;
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Section(string id)
        {
            int sectionid;
            if (!int.TryParse(id, out sectionid))
            {
                return RedirectToAction("Index", "Forum");
            }
            //user is active, update the LastActive
            string currentUserID = User.Identity.GetUserId();
            UpdateLastActiveDate(currentUserID);
            //Basically get the number of sections, if the given parameter is out of bounds, redirect to the Index of Forum
            int minID = 0;
            int maxID = 3; // pull from db.Section count(*) instead of hardcoding
            if (sectionid < minID || sectionid > maxID)
            {
                return RedirectToAction("Index", "Forum");
            }
            string Title;
            List<User> userlist = new List<User>();
            using(GamblingDBContext db = new GamblingDBContext())
            {
            var threads = from t in db.Threads
                          where t.SectionID == sectionid
                          orderby t.PostTime descending
                          select t;
            Title = db.Sections.Find(sectionid).Name;
            var threadList = threads.ToList();

            foreach(Thread x in threadList)
            {
                x.Title = Server.HtmlDecode(x.Title);
            }
             
            foreach (var thread in threadList)
            {
                var x = from u in db.Users
                        where u.UserID == thread.UserID
                        select u;
                if (x.Count() == 1)
                {
                    thread.UserID = x.First().Username;
                }
                userlist = db.Users.ToList();
            }
            ViewBag.AllUsers = userlist;
            ViewBag.section_id = id;
            ViewBag.Title = Title;
            return View(threadList);
        }
        }

        //FILL THIS IN!!!
        public ActionResult NewThread(string section_id)
        {
            //user is active, update the LastActive
            string currentUserID = User.Identity.GetUserId();
            UpdateLastActiveDate(currentUserID);

            int sectionid;
            if (int.TryParse(section_id, out sectionid))
            {
                List<Section> sections;
                using (GamblingDBContext db = new GamblingDBContext())
                {
                    sections = (from s in db.Sections
                              where s.ID == sectionid
                                    select s).ToList();
                }

                int sectioncounter = sections.Count();
                if (sectioncounter == 1) 
                {
                    // Creates thread in that section
                    ViewBag.section_id = section_id;
                    if (TempData["MessageLabel"] != null)
                    {
                        ViewBag.MessageLabel = TempData["MessageLabel"].ToString();
                    }
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Forum");
                }
            }
            else
            {
                return RedirectToAction("Index", "Forum");
            }
        }

        [ValidateInput(false)]
        public ActionResult PostNewThread(string title, string text, int section_id, string user_id)
        {
            //user is active, update the LastActive
            string currentUserID = User.Identity.GetUserId();
            UpdateLastActiveDate(currentUserID);

            Thread newThread = new Thread();
            Post newPost = new Post();

            //check if logged in;
            List<User> users;
            using (GamblingDBContext db = new GamblingDBContext())
            {
                users = (from u in db.Users
                        where u.UserID == user_id
                            select u).ToList();
            }
            
            int usercount = users.Count();
            if (usercount != 1)
            {
                TempData["MessageLabel"] = "You are not logged in!";
                return RedirectToAction("NewThread", "Forum", new { section_id = section_id });
            }

            if (title.Length == 0 || text.Length == 0)
            {
                TempData["MessageLabel"] = "Title and text can not be empty!";
                return RedirectToAction("NewThread", "Forum", new { section_id = section_id });
            }

            if (title.Length > 100)
            {
                TempData["MessageLabel"] = "Your title exceeds the 100 characters length.";
                return RedirectToAction("NewThread", "Forum", new { section_id = section_id });
            }

            if (text.Length > 5000)
            {
                TempData["MessageLabel"] = "Your thread exceeds the 5000 characters length.";
                return RedirectToAction("NewThread", "Forum", new { section_id = section_id });
            }
            
            //update the Threads table;
            newThread.SectionID = section_id;
            newThread.Title = Server.HtmlEncode(title);
            newThread.UserID = user_id;
            newThread.PostTime = DateTime.Now;
            using (GamblingDBContext db = new GamblingDBContext())
            {
            db.Threads.Add(newThread);
            db.SaveChanges();        //maybe SubmitChanges()???
            }
            

            //update the Posts table;
            newPost.ThreadID = newThread.ID;   //get the threadid
            newPost.UserID = user_id;
            newPost.Text = Server.HtmlEncode(text);
            newPost.ReplyTime = DateTime.Now;
            using (GamblingDBContext db = new GamblingDBContext())
            {
            db.Posts.Add(newPost);
            db.SaveChanges(); 
            }
            
            //redirect to section;
            return RedirectToAction("Section", "forum", new { id = section_id });
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
                    if (users.Count() == 1)
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