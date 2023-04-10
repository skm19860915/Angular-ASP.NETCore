namespace Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingSurveyDisplayWeeks : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AssignedProgram_SurveyDisplayWeeks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AssignedProgram_ProgramDayItemSurveyId = c.Int(nullable: false),
                        DisplayWeek = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AssignedProgram_ProgramDayItemSurvey", t => t.AssignedProgram_ProgramDayItemSurveyId)
                .Index(t => t.AssignedProgram_ProgramDayItemSurveyId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AssignedProgram_SurveyDisplayWeeks", "AssignedProgram_ProgramDayItemSurveyId", "dbo.AssignedProgram_ProgramDayItemSurvey");
            DropIndex("dbo.AssignedProgram_SurveyDisplayWeeks", new[] { "AssignedProgram_ProgramDayItemSurveyId" });
            DropTable("dbo.AssignedProgram_SurveyDisplayWeeks");
        }
    }
}
