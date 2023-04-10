namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class settingsNOtesToString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SuperSetNotes", "Note", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SuperSetNotes", "Note", c => c.Int(nullable: false));
        }
    }
}
