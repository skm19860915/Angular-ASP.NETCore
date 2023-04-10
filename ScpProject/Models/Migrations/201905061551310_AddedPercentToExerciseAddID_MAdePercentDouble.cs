namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddedPercentToExerciseAddID_MAdePercentDouble : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Exercises", name: "PercentMetricCalculation", newName: "PercentMetricCalculationId");
            RenameIndex(table: "dbo.Exercises", name: "IX_PercentMetricCalculation", newName: "IX_PercentMetricCalculationId");
            AlterColumn("dbo.Exercises", "Percent", c => c.Double());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Exercises", "Percent", c => c.Int());
            RenameIndex(table: "dbo.Exercises", name: "IX_PercentMetricCalculationId", newName: "IX_PercentMetricCalculation");
            RenameColumn(table: "dbo.Exercises", name: "PercentMetricCalculationId", newName: "PercentMetricCalculation");
        }
    }
}
