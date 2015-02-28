namespace GamblingNewbies.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Ptime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tables", "Ptime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tables", "Ptime");
        }
    }
}
