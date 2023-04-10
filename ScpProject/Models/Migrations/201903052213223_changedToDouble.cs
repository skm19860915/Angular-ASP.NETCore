namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class changedToDouble : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CompletedMetrics", "Value", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CompletedMetrics", "Value", c => c.Int(nullable: false));
        }
    }
}
