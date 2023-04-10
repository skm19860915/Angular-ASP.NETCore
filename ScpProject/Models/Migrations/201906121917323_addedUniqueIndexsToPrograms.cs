namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addedUniqueIndexsToPrograms : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.ProgramDays", new[] { "ProgramId" });
            DropIndex("dbo.ProgramDayItems", new[] { "ProgramDayId" });
            DropIndex("dbo.ProgramSets", new[] { "ParentProgramWeekId" });
            DropIndex("dbo.ProgramWeeks", new[] { "ProgramDayItemExerciseId" });
            CreateIndex("dbo.ProgramDays", new[] { "ProgramId", "Position" }, unique: true, name: "IX_ProgramDay_Position_ProgramId");
            CreateIndex("dbo.ProgramDayItems", new[] { "ProgramDayId", "Position" }, unique: true, name: "IX_ProgramDayItem_ProgramDayId_Position");
            CreateIndex("dbo.ProgramSets", new[] { "ParentProgramWeekId", "Position" }, unique: true, name: "IX_ProgramSet_ParentProgramWeekId_Position");
            CreateIndex("dbo.ProgramWeeks", new[] { "ProgramDayItemExerciseId", "Position" }, unique: true, name: "IX_ProgramWeek_ProgramDayItemExerciseId_Position");
        }
        
        public override void Down()
        {
            DropIndex("dbo.ProgramWeeks", "IX_ProgramWeek_ProgramDayItemExerciseId_Position");
            DropIndex("dbo.ProgramSets", "IX_ProgramSet_ParentProgramWeekId_Position");
            DropIndex("dbo.ProgramDayItems", "IX_ProgramDayItem_ProgramDayId_Position");
            DropIndex("dbo.ProgramDays", "IX_ProgramDay_Position_ProgramId");
            CreateIndex("dbo.ProgramWeeks", "ProgramDayItemExerciseId");
            CreateIndex("dbo.ProgramSets", "ParentProgramWeekId");
            CreateIndex("dbo.ProgramDayItems", "ProgramDayId");
            CreateIndex("dbo.ProgramDays", "ProgramId");
        }
    }
}
