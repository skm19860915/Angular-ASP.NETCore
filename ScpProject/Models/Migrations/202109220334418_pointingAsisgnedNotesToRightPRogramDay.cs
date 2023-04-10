namespace Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class pointingAsisgnedNotesToRightPRogramDay : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.AssignedProgram_ProgramDayItemNote", name: "ProgramDayItemId", newName: "AssignedProgram_ProgramDayItemId");
            RenameIndex(table: "dbo.AssignedProgram_ProgramDayItemNote", name: "IX_ProgramDayItemId", newName: "IX_AssignedProgram_ProgramDayItemId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.AssignedProgram_ProgramDayItemNote", name: "IX_AssignedProgram_ProgramDayItemId", newName: "IX_ProgramDayItemId");
            RenameColumn(table: "dbo.AssignedProgram_ProgramDayItemNote", name: "AssignedProgram_ProgramDayItemId", newName: "ProgramDayItemId");
        }
    }
}
