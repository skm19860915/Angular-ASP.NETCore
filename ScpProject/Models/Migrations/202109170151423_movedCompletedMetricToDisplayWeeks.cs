namespace Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class movedCompletedMetricToDisplayWeeks : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AssignedProgram_MetricsDisplayWeek", "Value", c => c.Double());
            AddColumn("dbo.AssignedProgram_MetricsDisplayWeek", "CompletedDate", c => c.DateTime());
            DropColumn("dbo.AssignedProgram_ProgramDayItemMetric", "Value");
            DropColumn("dbo.AssignedProgram_ProgramDayItemMetric", "CompletedDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AssignedProgram_ProgramDayItemMetric", "CompletedDate", c => c.DateTime());
            AddColumn("dbo.AssignedProgram_ProgramDayItemMetric", "Value", c => c.Double());
            DropColumn("dbo.AssignedProgram_MetricsDisplayWeek", "CompletedDate");
            DropColumn("dbo.AssignedProgram_MetricsDisplayWeek", "Value");
        }
    }
}
