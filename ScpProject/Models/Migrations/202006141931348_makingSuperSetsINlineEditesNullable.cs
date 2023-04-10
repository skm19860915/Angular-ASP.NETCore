namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class makingSuperSetsINlineEditesNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ProgramDayItemSuperSet_Set", "Sets", c => c.Int());
            AlterColumn("dbo.ProgramDayItemSuperSet_Set", "Reps", c => c.Int());
            AlterColumn("dbo.ProgramDayItemSuperSet_Set", "Percent", c => c.Double());
            AlterColumn("dbo.ProgramDayItemSuperSet_Set", "Weight", c => c.Double());
            AlterColumn("dbo.ProgramDayItemSuperSet_Set", "Minutes", c => c.Int());
            AlterColumn("dbo.ProgramDayItemSuperSet_Set", "Seconds", c => c.Int());
            AlterColumn("dbo.ProgramDayItemSuperSet_Set", "Distance", c => c.Double());
            AlterColumn("dbo.ProgramDayItemSuperSet_Set", "RepsAchieved", c => c.Boolean());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ProgramDayItemSuperSet_Set", "RepsAchieved", c => c.Boolean(nullable: false));
            AlterColumn("dbo.ProgramDayItemSuperSet_Set", "Distance", c => c.Double(nullable: false));
            AlterColumn("dbo.ProgramDayItemSuperSet_Set", "Seconds", c => c.Int(nullable: false));
            AlterColumn("dbo.ProgramDayItemSuperSet_Set", "Minutes", c => c.Int(nullable: false));
            AlterColumn("dbo.ProgramDayItemSuperSet_Set", "Weight", c => c.Double(nullable: false));
            AlterColumn("dbo.ProgramDayItemSuperSet_Set", "Percent", c => c.Double(nullable: false));
            AlterColumn("dbo.ProgramDayItemSuperSet_Set", "Reps", c => c.Int(nullable: false));
            AlterColumn("dbo.ProgramDayItemSuperSet_Set", "Sets", c => c.Int(nullable: false));
        }
    }
}
