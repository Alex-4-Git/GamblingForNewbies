using GamblingNewbies.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

///This controller (and associated views) is primarily written by Vangie.
namespace GamblingNewbies.Controllers
{

    public class ThreadController : Controller
    {
        private GamblingDBContext db = new GamblingDBContext();
        // GET: Thread

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
            return RedirectToAction("Index", "Forum");
        }

        [ValidateInput(false)]
        public ActionResult Post(string UserId, string ThreadId, string reply_text)
        {
            //user is active, update the LastActive
            string currentUserID = User.Identity.GetUserId();
            UpdateLastActiveDate(currentUserID);
            // Else you can attempt to create the new post
            Post newreply = new Post();

            // First validate thread ID
            int thread_id;
            if (int.TryParse(ThreadId, out thread_id))
            {
                var threads = from p in db.Threads
                              where p.ID == thread_id
                              select p;
                int threadcounter = threads.Count();
                if (threadcounter == 1) { newreply.ThreadID = thread_id; }
            }
            else
            {
                return RedirectToAction("Index", "Thread");
            }

            // Validate User making the post
            var users = from p in db.Users
                        where p.UserID == UserId
                        select p;
            int usercounter = users.Count();
            if (usercounter != 1)
            {
                TempData["MessageLabel"] = "You're not logged in!";
                return RedirectToAction("Topic", "Thread", new { id = ThreadId });
            }
            newreply.UserID = UserId;

            // Set the reply_text and Time
            // Check reply length, if too long, go back to thread and set error label.
            if (reply_text.Length > 200)
            {
                TempData["MessageLabel"] = "Your reply exceeded the 500 character length.";
                TempData["TextAreaText"] = reply_text;
                return RedirectToAction("Topic", "Thread", new { id = ThreadId });
            }
            newreply.Text = Server.HtmlEncode(reply_text);
            newreply.ReplyTime = DateTime.UtcNow;

            // Add & Save to database, this redirect back to original Thread
            db.Posts.Add(newreply);
            db.SaveChanges();
            return RedirectToAction("Topic", "Thread", new { id = ThreadId });
        }


        public ActionResult Topic(string id)
        {
            //user is active, update the LastActive
            string currentUserID = User.Identity.GetUserId();
            UpdateLastActiveDate(currentUserID);
            // Parse input string into an int if possible
            int threadid;
            if (int.TryParse(id, out threadid))
            {
                // Pull the thread record from DB that has that ID
                var threads = db.Threads.Find(threadid);
                if (threads == null) // If not found, redirect to Forum
                {
                    return RedirectToAction("Index", "Thread");
                }
                ViewBag.Title = Server.HtmlDecode(threads.Title); // Pull Thread Title from record
                ViewBag.TopicID = id; // Return topic id, used to determine Post Action param ThreadId
                
                // Determine the parent section of this thread
                var sect = db.Sections.Find(threads.SectionID);
                if (sect == null)
                {
                    return RedirectToAction("Index", "Forum");
                }
                ViewBag.ParentSectName = sect.Name;
                ViewBag.ParentSectID = threads.SectionID;

                // Pull all posts for that thread ID
                var posts = from p in db.Posts
                            where p.ThreadID == threadid
                            orderby p.ReplyTime
                            select p;
                var postlist = posts.ToList();
                foreach(Post x in postlist)
                {
                    x.Text = Server.HtmlDecode(x.Text);
                }
                foreach (var p in postlist)
                {
                    var x = from u in db.Users
                                where u.UserID == p.UserID
                                select u;
                    if (x.Count() == 1)
                    {
                        p.UserID = x.First().Username;
                    }
                }

                // This label is visible if Post action redirected due to improper input
                if (TempData["MessageLabel"] != null)
                {
                    ViewBag.MessageLabel = TempData["MessageLabel"].ToString();
                }
                if (TempData["TextAreaText"] != null)
                {
                    ViewBag.TextAreaText = TempData["TextAreaText"].ToString();
                }

                return View(postlist);
            }
            else // invalid action param leads to redirect
            {
                return RedirectToAction("Index", "Forum");
            }

            
        }
    }
}