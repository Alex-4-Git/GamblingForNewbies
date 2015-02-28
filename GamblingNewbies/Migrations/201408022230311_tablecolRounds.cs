namespace GamblingNewbies.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tablecolRounds : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tables", "User1Wins", c => c.Int(nullable: false));
            AddColumn("dbo.Tables", "User2Wins", c => c.Int(nullable: false));
            AddColumn("dbo.Tables", "TotalRounds", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tables", "TotalRounds");
            DropColumn("dbo.Tables", "User2Wins");
            DropColumn("dbo.Tables", "User1Wins");
        }
    }
}
