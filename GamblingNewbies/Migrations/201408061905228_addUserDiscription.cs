namespace GamblingNewbies.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addUserDiscription : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "UserDiscription", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "UserDiscription");
        }
    }
}
