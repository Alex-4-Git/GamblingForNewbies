namespace GamblingNewbies.Migrations
{
    using GamblingNewbies.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Data;
    using System.Net;
    using System.Web;
    using System.Web.Mvc;


    internal sealed class Configuration : DbMigrationsConfiguration<GamblingNewbies.Models.GamblingDBContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "GamblingNewbies.Models.GamblingDBContext";
        }

        protected override void Seed(GamblingNewbies.Models.GamblingDBContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            var sections = new List<Section>() {
                new Section { ID = 1, Name = "Announcements" },
                new Section { ID = 2, Name = "General Chat" },
                new Section { ID = 3, Name = "Questions" }
            };
            sections.ForEach(s => context.Sections.AddOrUpdate(p => p.ID, s));
            context.SaveChanges();

        }
    }
}
