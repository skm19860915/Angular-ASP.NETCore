namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddedPercentToExercise : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Exercises", "Percent", c => c.Int());
            AddColumn("dbo.Exercises", "PercentMetricCalculation", c => c.Int());
            CreateIndex("dbo.Exercises", "PercentMetricCalculation");
            AddForeignKey("dbo.Exercises", "PercentMetricCalculation", "dbo.Metrics", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Exercises", "PercentMetricCalculation", "dbo.Metrics");
            DropIndex("dbo.Exercises", new[] { "PercentMetricCalculation" });
            DropColumn("dbo.Exercises", "PercentMetricCalculation");
            DropColumn("dbo.Exercises", "Percent");
        }
    }
}
