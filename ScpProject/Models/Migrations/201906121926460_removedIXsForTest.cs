namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class removedIXsForTest : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.ProgramSets", "IX_ProgramSet_ParentProgramWeekId_Position");
            DropIndex("dbo.ProgramWeeks", "IX_ProgramWeek_ProgramDayItemExerciseId_Position");
            CreateIndex("dbo.ProgramSets", "ParentProgramWeekId");
            CreateIndex("dbo.ProgramWeeks", "ProgramDayItemExerciseId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.ProgramWeeks", new[] { "ProgramDayItemExerciseId" });
            DropIndex("dbo.ProgramSets", new[] { "ParentProgramWeekId" });
            CreateIndex("dbo.ProgramWeeks", new[] { "ProgramDayItemExerciseId", "Position" }, unique: true, name: "IX_ProgramWeek_ProgramDayItemExerciseId_Position");
            CreateIndex("dbo.ProgramSets", new[] { "ParentProgramWeekId", "Position" }, unique: true, name: "IX_ProgramSet_ParentProgramWeekId_Position");
        }
    }
}
