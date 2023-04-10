namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addedNotifications : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Notifications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Description = c.String(),
                        URL = c.String(),
                        GeneratedUserId = c.Int(nullable: false),
                        Type_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.GeneratedUserId)
                .ForeignKey("dbo.NotificationTypes", t => t.Type_Id)
                .Index(t => t.GeneratedUserId)
                .Index(t => t.Type_Id);
            
            CreateTable(
                "dbo.NotificationTypes",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Notifications", "Type_Id", "dbo.NotificationTypes");
            DropForeignKey("dbo.Notifications", "GeneratedUserId", "dbo.Users");
            DropIndex("dbo.Notifications", new[] { "Type_Id" });
            DropIndex("dbo.Notifications", new[] { "GeneratedUserId" });
            DropTable("dbo.NotificationTypes");
            DropTable("dbo.Notifications");
        }
    }
}
