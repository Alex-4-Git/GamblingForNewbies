namespace GamblingNewbies.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class winnerfield : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tables", "Winner", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tables", "Winner");
        }
    }
}
