namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addingImagesAndColorsToOrganizations : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Organizations", "ProfilePictureId", c => c.Int());
            AddColumn("dbo.Organizations", "PrimaryColorHex", c => c.String());
            AddColumn("dbo.Organizations", "SecondaryColorHex", c => c.String());
            CreateIndex("dbo.Organizations", "ProfilePictureId");
            AddForeignKey("dbo.Organizations", "ProfilePictureId", "dbo.Pictures", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Organizations", "ProfilePictureId", "dbo.Pictures");
            DropIndex("dbo.Organizations", new[] { "ProfilePictureId" });
            DropColumn("dbo.Organizations", "SecondaryColorHex");
            DropColumn("dbo.Organizations", "PrimaryColorHex");
            DropColumn("dbo.Organizations", "ProfilePictureId");
        }
    }
}
