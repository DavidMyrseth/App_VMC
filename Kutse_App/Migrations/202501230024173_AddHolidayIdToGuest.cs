namespace Kutse_App.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddHolidayIdToGuest : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Guests", "HolidayId", c => c.Int());
            CreateIndex("dbo.Guests", "HolidayId");
            AddForeignKey("dbo.Guests", "HolidayId", "dbo.Holidays", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Guests", "HolidayId", "dbo.Holidays");
            DropIndex("dbo.Guests", new[] { "HolidayId" });
            DropColumn("dbo.Guests", "HolidayId");
        }
    }
}
