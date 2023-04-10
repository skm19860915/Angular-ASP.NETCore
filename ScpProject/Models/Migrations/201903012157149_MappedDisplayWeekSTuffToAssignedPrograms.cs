namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class MappedDisplayWeekSTuffToAssignedPrograms : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MetricDisplayWeeks", "AssignedProgramId", c => c.Int(nullable: false));
            AddColumn("dbo.NoteDisplayWeeks", "AssignedProgramId", c => c.Int(nullable: false));
            AddColumn("dbo.SurveyDisplayWeeks", "AssignedProgramId", c => c.Int(nullable: false));
            CreateIndex("dbo.MetricDisplayWeeks", "AssignedProgramId");
            CreateIndex("dbo.NoteDisplayWeeks", "AssignedProgramId");
            CreateIndex("dbo.SurveyDisplayWeeks", "AssignedProgramId");
            AddForeignKey("dbo.MetricDisplayWeeks", "AssignedProgramId", "dbo.AssignedPrograms", "Id");
            AddForeignKey("dbo.NoteDisplayWeeks", "AssignedProgramId", "dbo.AssignedPrograms", "Id");
            AddForeignKey("dbo.SurveyDisplayWeeks", "AssignedProgramId", "dbo.AssignedPrograms", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SurveyDisplayWeeks", "AssignedProgramId", "dbo.AssignedPrograms");
            DropForeignKey("dbo.NoteDisplayWeeks", "AssignedProgramId", "dbo.AssignedPrograms");
            DropForeignKey("dbo.MetricDisplayWeeks", "AssignedProgramId", "dbo.AssignedPrograms");
            DropIndex("dbo.SurveyDisplayWeeks", new[] { "AssignedProgramId" });
            DropIndex("dbo.NoteDisplayWeeks", new[] { "AssignedProgramId" });
            DropIndex("dbo.MetricDisplayWeeks", new[] { "AssignedProgramId" });
            DropColumn("dbo.SurveyDisplayWeeks", "AssignedProgramId");
            DropColumn("dbo.NoteDisplayWeeks", "AssignedProgramId");
            DropColumn("dbo.MetricDisplayWeeks", "AssignedProgramId");
        }
    }
}
