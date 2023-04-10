namespace Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedAssignedProgramMetricDisplaayWeeks : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AssignedProgram_MetricsDisplayWeek",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AssignedProgram_ProgramDayItemMetricId = c.Int(nullable: false),
                        DisplayWeek = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AssignedProgram_ProgramDayItemMetric", t => t.AssignedProgram_ProgramDayItemMetricId)
                .Index(t => t.AssignedProgram_ProgramDayItemMetricId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AssignedProgram_MetricsDisplayWeek", "AssignedProgram_ProgramDayItemMetricId", "dbo.AssignedProgram_ProgramDayItemMetric");
            DropIndex("dbo.AssignedProgram_MetricsDisplayWeek", new[] { "AssignedProgram_ProgramDayItemMetricId" });
            DropTable("dbo.AssignedProgram_MetricsDisplayWeek");
        }
    }
}
