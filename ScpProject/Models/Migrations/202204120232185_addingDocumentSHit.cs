namespace Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingDocumentSHit : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Documents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Contents = c.String(),
                        CreatedUserId = c.Int(nullable: false),
                        OrganizationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedUserId)
                .ForeignKey("dbo.Organizations", t => t.OrganizationId)
                .Index(t => t.CreatedUserId)
                .Index(t => t.OrganizationId);
            
            CreateTable(
                "dbo.DocumentTags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 200),
                        OrganizationId = c.Int(nullable: false),
                        Notes = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => new { t.Name, t.OrganizationId }, unique: true, name: "IX_UniqueTag");
            
            CreateTable(
                "dbo.TagsToDocuments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TagId = c.Int(nullable: false),
                        DocumentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DocumentTags", t => t.TagId)
                .ForeignKey("dbo.Documents", t => t.DocumentId)
                .Index(t => t.TagId)
                .Index(t => t.DocumentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TagsToDocuments", "DocumentId", "dbo.Documents");
            DropForeignKey("dbo.TagsToDocuments", "TagId", "dbo.DocumentTags");
            DropForeignKey("dbo.Documents", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.Documents", "CreatedUserId", "dbo.Users");
            DropIndex("dbo.TagsToDocuments", new[] { "DocumentId" });
            DropIndex("dbo.TagsToDocuments", new[] { "TagId" });
            DropIndex("dbo.DocumentTags", "IX_UniqueTag");
            DropIndex("dbo.Documents", new[] { "OrganizationId" });
            DropIndex("dbo.Documents", new[] { "CreatedUserId" });
            DropTable("dbo.TagsToDocuments");
            DropTable("dbo.DocumentTags");
            DropTable("dbo.Documents");
        }
    }
}
