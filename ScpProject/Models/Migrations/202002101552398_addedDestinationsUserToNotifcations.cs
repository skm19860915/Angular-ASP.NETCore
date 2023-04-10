namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addedDestinationsUserToNotifcations : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Notifications", "DestinationUserId", c => c.Int(nullable: false));
            AddColumn("dbo.Notifications", "SentDate", c => c.DateTime(nullable: false));
            CreateIndex("dbo.Notifications", "DestinationUserId");
            AddForeignKey("dbo.Notifications", "DestinationUserId", "dbo.Users", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Notifications", "DestinationUserId", "dbo.Users");
            DropIndex("dbo.Notifications", new[] { "DestinationUserId" });
            DropColumn("dbo.Notifications", "SentDate");
            DropColumn("dbo.Notifications", "DestinationUserId");
        }
    }
}
