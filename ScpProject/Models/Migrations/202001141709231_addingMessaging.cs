namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addingMessaging : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Content = c.String(),
                        Title = c.String(),
                        SentTime = c.DateTime(nullable: false),
                        CreatedUserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MessageToUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DestinationUserId = c.Int(nullable: false),
                        ReadTime = c.DateTime(),
                        MessageId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.DestinationUserId)
                .ForeignKey("dbo.Messages", t => t.MessageId)
                .Index(t => t.DestinationUserId)
                .Index(t => t.MessageId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MessageToUsers", "MessageId", "dbo.Messages");
            DropForeignKey("dbo.MessageToUsers", "DestinationUserId", "dbo.Users");
            DropIndex("dbo.MessageToUsers", new[] { "MessageId" });
            DropIndex("dbo.MessageToUsers", new[] { "DestinationUserId" });
            DropTable("dbo.MessageToUsers");
            DropTable("dbo.Messages");
        }
    }
}
