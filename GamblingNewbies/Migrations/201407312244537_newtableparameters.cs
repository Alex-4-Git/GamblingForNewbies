namespace GamblingNewbies.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newtableparameters : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tables", "Name", c => c.String());
            AddColumn("dbo.Tables", "Choice1", c => c.Int(nullable: false));
            AddColumn("dbo.Tables", "Choice2", c => c.Int(nullable: false));
            AddColumn("dbo.Tables", "Cost", c => c.Int(nullable: false));
            AlterColumn("dbo.Tables", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tables", "Status", c => c.String());
            DropColumn("dbo.Tables", "Cost");
            DropColumn("dbo.Tables", "Choice2");
            DropColumn("dbo.Tables", "Choice1");
            DropColumn("dbo.Tables", "Name");
        }
    }
}
