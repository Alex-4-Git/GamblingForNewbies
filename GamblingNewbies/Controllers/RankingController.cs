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
    public class RankingController : Controller
    {
        private GamblingDBContext db = new GamblingDBContext();

        protected const int PAGE_VOLUMN = 3;
        public static int UserRank {get; set;}
        public static int TotalPage { get; set; }
        public static List<User> WholeList { get; set; }

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
        public List<User> showCurrentList(int pageNum)
        {
            //user is active, update the LastActive
            string currentUserID = User.Identity.GetUserId();
            UpdateLastActiveDate(currentUserID);
            List<User> result = null;

            if(WholeList.Count() > pageNum * PAGE_VOLUMN) {
                result = WholeList.GetRange((pageNum - 1) * PAGE_VOLUMN, PAGE_VOLUMN);
            }
            else
            {
                result = WholeList.GetRange((pageNum - 1) * PAGE_VOLUMN,
                                       WholeList.Count() - (pageNum - 1) * PAGE_VOLUMN);
            }
            return result;
        }

        public static List<User> refreshWholeList()
        {
            using (GamblingDBContext db = new GamblingDBContext())
            {
                WholeList = (from item in db.Users
                             orderby item.Coins descending
                             select item).ToList();
                return WholeList;
            }
        }
        
        // GET: Ranking
        public ActionResult Index()
        {
            //user is active, update the LastActive
            string currentUserID = User.Identity.GetUserId();
            UpdateLastActiveDate(currentUserID);
            string id = User.Identity.Name;
            List<User> rankList = refreshWholeList();
                TotalPage = (rankList.Count() - 1) / PAGE_VOLUMN + 1;

                UserRank = rankList.FindIndex(x => x.Username == id) + 1;
                ViewBag.UserRank = UserRank;
                ViewBag.PAGE_VOLUMN = PAGE_VOLUMN;

                List<User> list = showCurrentList(1);

                return View(list);
                      
        }

        // GET: Ranking/Page/[id]
        public ActionResult Page(int? id)
        {
            //user is active, update the LastActive
            string currentUserID = User.Identity.GetUserId();
            UpdateLastActiveDate(currentUserID);
            ViewBag.PAGE_VOLUMN = PAGE_VOLUMN;
            ViewBag.TotalPage = TotalPage;
            string usernamein = User.Identity.Name;
            List<User> list = refreshWholeList();
            ViewBag.UserRank = WholeList.FindIndex(x => x.Username == usernamein) + 1;

            int pageNum = 0;
            if (id == null) pageNum = 1;
            else pageNum = Int32.Parse(id.ToString());
            ViewBag.Message = pageNum;

            if (pageNum > TotalPage)
            {
                return Redirect("/Ranking/Page/1");
            }
            else
            {
                list = showCurrentList(pageNum);
                return View(list);
            }
        }
       
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
