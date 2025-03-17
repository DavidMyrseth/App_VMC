namespace Kutse_App.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeletedRequeredWillAttend : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Guests", "WillAttend", c => c.Boolean());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Guests", "WillAttend", c => c.Boolean(nullable: false));
        }
    }
}
