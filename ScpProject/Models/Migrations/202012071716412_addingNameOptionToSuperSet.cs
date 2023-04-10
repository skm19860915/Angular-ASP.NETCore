namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addingNameOptionToSuperSet : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProgramDayItemSuperSets", "SuperSetDisplayTitle", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProgramDayItemSuperSets", "SuperSetDisplayTitle");
        }
    }
}
