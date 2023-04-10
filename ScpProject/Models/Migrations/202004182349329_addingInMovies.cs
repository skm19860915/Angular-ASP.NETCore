namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addingInMovies : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Movies", new[] { "CreatedUserId" });
            AddColumn("dbo.Movies", "Name", c => c.String(maxLength: 200));
            AddColumn("dbo.Movies", "CanModify", c => c.Boolean(nullable: false));
            AddColumn("dbo.Movies", "OrganizationId", c => c.Int(nullable: false));
            AddColumn("dbo.Movies", "IsDeleted", c => c.Boolean(nullable: false));
            CreateIndex("dbo.Movies", new[] { "CreatedUserId", "Name" }, unique: true, name: "IX_CreatedUserId_Name_Movies");
            DropColumn("dbo.Movies", "Title");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Movies", "Title", c => c.String());
            DropIndex("dbo.Movies", "IX_CreatedUserId_Name_Movies");
            DropColumn("dbo.Movies", "IsDeleted");
            DropColumn("dbo.Movies", "OrganizationId");
            DropColumn("dbo.Movies", "CanModify");
            DropColumn("dbo.Movies", "Name");
            CreateIndex("dbo.Movies", "CreatedUserId");
        }
    }
}
