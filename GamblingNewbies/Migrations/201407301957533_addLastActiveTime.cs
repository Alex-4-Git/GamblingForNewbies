namespace GamblingNewbies.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addLastActiveTime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "LastActiveTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "LastActiveTime");
        }
    }
}
