namespace GamblingNewbies.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addPostTime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Threads", "PostTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Threads", "PostTime");
        }
    }
}
