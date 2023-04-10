namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class toChangeIntsToDoubles : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ProgramDayItemSuperSet_Set", "Percent", c => c.Double(nullable: false));
            AlterColumn("dbo.ProgramDayItemSuperSet_Set", "Weight", c => c.Double(nullable: false));
            AlterColumn("dbo.Sets", "Percent", c => c.Double(nullable: false));
            AlterColumn("dbo.Sets", "Weight", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Sets", "Weight", c => c.Int(nullable: false));
            AlterColumn("dbo.Sets", "Percent", c => c.Int(nullable: false));
            AlterColumn("dbo.ProgramDayItemSuperSet_Set", "Weight", c => c.Int(nullable: false));
            AlterColumn("dbo.ProgramDayItemSuperSet_Set", "Percent", c => c.Int(nullable: false));
        }
    }
}
