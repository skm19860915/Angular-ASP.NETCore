namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class fixedSpellingError : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.MetricDisplayWeeks", name: "ProgramDayItemMeticId", newName: "ProgramDayItemMetricId");
            RenameIndex(table: "dbo.MetricDisplayWeeks", name: "IX_ProgramDayItemMeticId", newName: "IX_ProgramDayItemMetricId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.MetricDisplayWeeks", name: "IX_ProgramDayItemMetricId", newName: "IX_ProgramDayItemMeticId");
            RenameColumn(table: "dbo.MetricDisplayWeeks", name: "ProgramDayItemMetricId", newName: "ProgramDayItemMeticId");
        }
    }
}
