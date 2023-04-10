namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class allowDoublesintoCompleted : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CompletedSets", "Percent", c => c.Double(nullable: false));
            AlterColumn("dbo.CompletedSets", "Weight", c => c.Double(nullable: false));
            AlterColumn("dbo.CompletedSuperSet_Set", "Percent", c => c.Double(nullable: false));
            AlterColumn("dbo.CompletedSuperSet_Set", "Weight", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CompletedSuperSet_Set", "Weight", c => c.Int(nullable: false));
            AlterColumn("dbo.CompletedSuperSet_Set", "Percent", c => c.Int(nullable: false));
            AlterColumn("dbo.CompletedSets", "Weight", c => c.Int(nullable: false));
            AlterColumn("dbo.CompletedSets", "Percent", c => c.Int(nullable: false));
        }
    }
}
