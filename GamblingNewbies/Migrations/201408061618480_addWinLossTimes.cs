namespace GamblingNewbies.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addWinLossTimes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "WinTimes", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "LossTimes", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "LossTimes");
            DropColumn("dbo.Users", "WinTimes");
        }
    }
}
