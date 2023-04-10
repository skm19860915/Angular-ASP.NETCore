namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class tertiaryColorHex : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Organizations", "TertiaryColorHex", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Organizations", "TertiaryColorHex");
        }
    }
}
