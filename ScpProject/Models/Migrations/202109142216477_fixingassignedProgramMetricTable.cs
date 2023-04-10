namespace Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixingassignedProgramMetricTable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AssignedProgram_ProgramDayItemMetric", "Value", c => c.Double());
            AlterColumn("dbo.AssignedProgram_ProgramDayItemMetric", "CompletedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AssignedProgram_ProgramDayItemMetric", "CompletedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.AssignedProgram_ProgramDayItemMetric", "Value", c => c.Double(nullable: false));
        }
    }
}
