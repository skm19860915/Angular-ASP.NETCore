namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addingFontPickers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Organizations", "PrimaryFontColorHex", c => c.String());
            AddColumn("dbo.Organizations", "SecondaryFontColorHex", c => c.String());
            DropColumn("dbo.Organizations", "TertiaryColorHex");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Organizations", "TertiaryColorHex", c => c.String());
            DropColumn("dbo.Organizations", "SecondaryFontColorHex");
            DropColumn("dbo.Organizations", "PrimaryFontColorHex");
        }
    }
}
