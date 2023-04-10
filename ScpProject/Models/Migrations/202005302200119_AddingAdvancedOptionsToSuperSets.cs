namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddingAdvancedOptionsToSuperSets : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProgramDayItemSuperSet_Set", "Minutes", c => c.Int(nullable: false));
            AddColumn("dbo.ProgramDayItemSuperSet_Set", "Seconds", c => c.Int(nullable: false));
            AddColumn("dbo.ProgramDayItemSuperSet_Set", "Distance", c => c.Double(nullable: false));
            AddColumn("dbo.ProgramDayItemSuperSet_Set", "RepsAchieved", c => c.Boolean(nullable: false));
            AddColumn("dbo.ProgramDayItemSuperSet_Set", "Rest", c => c.Int(nullable: false));
            AddColumn("dbo.ProgramDayItemSuperSet_Set", "Other", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProgramDayItemSuperSet_Set", "Other");
            DropColumn("dbo.ProgramDayItemSuperSet_Set", "Rest");
            DropColumn("dbo.ProgramDayItemSuperSet_Set", "RepsAchieved");
            DropColumn("dbo.ProgramDayItemSuperSet_Set", "Distance");
            DropColumn("dbo.ProgramDayItemSuperSet_Set", "Seconds");
            DropColumn("dbo.ProgramDayItemSuperSet_Set", "Minutes");
        }
    }
}
