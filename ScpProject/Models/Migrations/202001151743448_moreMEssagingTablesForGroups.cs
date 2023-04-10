namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class moreMEssagingTablesForGroups : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.MessageToUsers", newName: "MessagesToUsers");
            CreateTable(
                "dbo.MessageGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GroupTitle = c.String(),
                        CreatedUserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MessageGroupsToUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MessageGroupId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId)
                .ForeignKey("dbo.MessageGroups", t => t.MessageGroupId)
                .Index(t => t.MessageGroupId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.MessagesToUsersInGroup",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DestinationUserId = c.Int(nullable: false),
                        MessageId = c.Int(nullable: false),
                        GroupId = c.Int(nullable: false),
                        ReadTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.DestinationUserId)
                .Index(t => t.MessageId);
            
            AddColumn("dbo.Messages", "ReadOnly", c => c.Boolean(nullable: false));
            AddColumn("dbo.Messages", "Pause", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MessageGroupsToUsers", "MessageGroupId", "dbo.MessageGroups");
            DropForeignKey("dbo.MessageGroupsToUsers", "UserId", "dbo.Users");
            DropIndex("dbo.MessagesToUsersInGroup", new[] { "MessageId" });
            DropIndex("dbo.MessagesToUsersInGroup", new[] { "DestinationUserId" });
            DropIndex("dbo.MessageGroupsToUsers", new[] { "UserId" });
            DropIndex("dbo.MessageGroupsToUsers", new[] { "MessageGroupId" });
            DropColumn("dbo.Messages", "Pause");
            DropColumn("dbo.Messages", "ReadOnly");
            DropTable("dbo.MessagesToUsersInGroup");
            DropTable("dbo.MessageGroupsToUsers");
            DropTable("dbo.MessageGroups");
            RenameTable(name: "dbo.MessagesToUsers", newName: "MessageToUsers");
        }
    }
}
