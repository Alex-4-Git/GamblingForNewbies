using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;


namespace GamblingNewbies.Models
{
    public class Section
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class Thread
    {
        public int ID { get; set; }
        public int SectionID { get; set; }
        public string UserID { get; set; }
        public string Title { get; set; }
        public DateTime PostTime { get; set; }
    }

    public class Post
    {
        public int ID { get; set; }
        public int ThreadID { get; set; }
        public string UserID { get; set; }
        public DateTime ReplyTime { get; set; }
        public string Text { get; set; }
    }

    public class User
    {
        public int ID { get; set; }
        public string UserID { get; set; }
        public string Username { get; set; }
        public int Coins { get; set; }
        public int WinTimes { get; set; }
        public int LossTimes { get; set; }
        public DateTime LastLogin { get; set; }
        public DateTime LastActiveTime { get; set; }
        public string UserDiscription { get; set; }
    }

    public class Message
    {
        public int ID { get; set; }
        public string  UserName  { get; set; }
        public DateTime ReplyTime { get; set; }
        public string PostUserName { get; set; }
        public string Text { get; set; }

    }

    public class Table
    {
        public int ID { get; set; }
        public string Username1 { get; set; } //These are actually userIDs?
        public string Username2 { get; set; } //These are actually userIDs?
        public int Status { get; set; }
        public string Name { get; set; }
        public int Choice1 { get; set; }
        public int Choice2 { get; set; }
        public int Cost { get; set; }
        public string Winner { get; set; } // This is actually a userID?
        public DateTime Ptime { get; set; }
        public int User1Wins { get; set; }
        public int User2Wins { get; set; }
        public int TotalRounds { get; set; }
    }

    //public class GamblingDBContext
    //{
    //    public DbSet<Section> Sections { get; set; }
    //    public DbSet<Thread> Threads { get; set; }
    //    public DbSet<Post> Posts { get; set; }
    //}

    public class GamblingDBContext : DbContext
    {
        public GamblingDBContext()
            : base("name=GamblingDBContext")
        {

        }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Thread> Threads { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Wall { get; set; }
        public DbSet<Table> Tables { get; set; }

    }
}