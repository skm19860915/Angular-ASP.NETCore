namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addingPasswordRecovery : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PasswordResets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        PasswordResetToken = c.Guid(nullable: false),
                        IssuedInUTC = c.DateTime(nullable: false),
                        ExpiresInUTC = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PasswordResets", "UserId", "dbo.Users");
            DropIndex("dbo.PasswordResets", new[] { "UserId" });
            DropTable("dbo.PasswordResets");
        }
    }
}
