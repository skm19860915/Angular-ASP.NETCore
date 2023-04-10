namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class fuckedItUpDontNeedToMapThemToAssignedPrograms : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MetricDisplayWeeks", "AssignedProgramId", "dbo.AssignedPrograms");
            DropForeignKey("dbo.NoteDisplayWeeks", "AssignedProgramId", "dbo.AssignedPrograms");
            DropForeignKey("dbo.SurveyDisplayWeeks", "AssignedProgramId", "dbo.AssignedPrograms");
            DropIndex("dbo.MetricDisplayWeeks", new[] { "AssignedProgramId" });
            DropIndex("dbo.NoteDisplayWeeks", new[] { "AssignedProgramId" });
            DropIndex("dbo.SurveyDisplayWeeks", new[] { "AssignedProgramId" });
            DropColumn("dbo.MetricDisplayWeeks", "AssignedProgramId");
            DropColumn("dbo.NoteDisplayWeeks", "AssignedProgramId");
            DropColumn("dbo.SurveyDisplayWeeks", "AssignedProgramId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SurveyDisplayWeeks", "AssignedProgramId", c => c.Int(nullable: false));
            AddColumn("dbo.NoteDisplayWeeks", "AssignedProgramId", c => c.Int(nullable: false));
            AddColumn("dbo.MetricDisplayWeeks", "AssignedProgramId", c => c.Int(nullable: false));
            CreateIndex("dbo.SurveyDisplayWeeks", "AssignedProgramId");
            CreateIndex("dbo.NoteDisplayWeeks", "AssignedProgramId");
            CreateIndex("dbo.MetricDisplayWeeks", "AssignedProgramId");
            AddForeignKey("dbo.SurveyDisplayWeeks", "AssignedProgramId", "dbo.AssignedPrograms", "Id");
            AddForeignKey("dbo.NoteDisplayWeeks", "AssignedProgramId", "dbo.AssignedPrograms", "Id");
            AddForeignKey("dbo.MetricDisplayWeeks", "AssignedProgramId", "dbo.AssignedPrograms", "Id");
        }
    }
}
