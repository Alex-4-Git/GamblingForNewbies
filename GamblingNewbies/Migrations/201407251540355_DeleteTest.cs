namespace GamblingNewbies.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteTest : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Posts", "test");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Posts", "test", c => c.String());
        }
    }
}
