namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class FixedCompletedMetric : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.CompletedMetrics", name: "OriginalMetricId", newName: "MetricId");
            RenameIndex(table: "dbo.CompletedMetrics", name: "IX_OriginalMetricId", newName: "IX_MetricId");
            AddColumn("dbo.CompletedMetrics", "ProgramDayItemMetricId", c => c.Int(nullable: false));
            CreateIndex("dbo.CompletedMetrics", "ProgramDayItemMetricId");
            AddForeignKey("dbo.CompletedMetrics", "ProgramDayItemMetricId", "dbo.ProgramDayItemMetrics", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CompletedMetrics", "ProgramDayItemMetricId", "dbo.ProgramDayItemMetrics");
            DropIndex("dbo.CompletedMetrics", new[] { "ProgramDayItemMetricId" });
            DropColumn("dbo.CompletedMetrics", "ProgramDayItemMetricId");
            RenameIndex(table: "dbo.CompletedMetrics", name: "IX_MetricId", newName: "IX_OriginalMetricId");
            RenameColumn(table: "dbo.CompletedMetrics", name: "MetricId", newName: "OriginalMetricId");
        }
    }
}
