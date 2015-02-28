namespace GamblingNewbies.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeUserID : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "UserID", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "UserID", c => c.Int(nullable: false));
        }
    }
}
