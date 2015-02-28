namespace GamblingNewbies.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ToStringUserID : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Posts", "UserID", c => c.String());
            AlterColumn("dbo.Threads", "UserID", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Threads", "UserID", c => c.Int(nullable: false));
            AlterColumn("dbo.Posts", "UserID", c => c.Int(nullable: false));
        }
    }
}
