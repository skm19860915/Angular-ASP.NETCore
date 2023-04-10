namespace Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingAggreements : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Agreements",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedUserId = c.Int(nullable: false),
                        DocumentId = c.Int(nullable: false),
                        OrganizationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Documents", t => t.DocumentId)
                .ForeignKey("dbo.Organizations", t => t.OrganizationId)
                .ForeignKey("dbo.Users", t => t.CreatedUserId)
                .Index(t => t.CreatedUserId)
                .Index(t => t.DocumentId)
                .Index(t => t.OrganizationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Agreements", "CreatedUserId", "dbo.Users");
            DropForeignKey("dbo.Agreements", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.Agreements", "DocumentId", "dbo.Documents");
            DropIndex("dbo.Agreements", new[] { "OrganizationId" });
            DropIndex("dbo.Agreements", new[] { "DocumentId" });
            DropIndex("dbo.Agreements", new[] { "CreatedUserId" });
            DropTable("dbo.Agreements");
        }
    }
}
