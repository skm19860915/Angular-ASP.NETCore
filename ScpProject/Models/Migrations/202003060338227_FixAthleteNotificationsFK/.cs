namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class FixAthleteNotificationsFK : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Notifications", "GeneratedUserId", "dbo.Users");
            DropIndex("dbo.Notifications", new[] { "GeneratedUserId" });
            AddColumn("dbo.Notifications", "GeneratedAthleteId", c => c.Int(nullable: false));
            CreateIndex("dbo.Notifications", "GeneratedAthleteId");
            AddForeignKey("dbo.Notifications", "GeneratedAthleteId", "dbo.Athletes", "Id");
            DropColumn("dbo.Notifications", "GeneratedUserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Notifications", "GeneratedUserId", c => c.Int(nullable: false));
            DropForeignKey("dbo.Notifications", "GeneratedAthleteId", "dbo.Athletes");
            DropIndex("dbo.Notifications", new[] { "GeneratedAthleteId" });
            DropColumn("dbo.Notifications", "GeneratedAthleteId");
            CreateIndex("dbo.Notifications", "GeneratedUserId");
            AddForeignKey("dbo.Notifications", "GeneratedUserId", "dbo.Users", "Id");
        }
    }
}
