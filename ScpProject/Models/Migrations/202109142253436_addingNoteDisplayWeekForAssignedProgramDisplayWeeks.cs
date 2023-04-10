namespace Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingNoteDisplayWeekForAssignedProgramDisplayWeeks : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AssignedProgram_NoteDisplayWeek",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AssignedProgram_ProgramDayItemNoteId = c.Int(nullable: false),
                        DisplayWeek = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AssignedProgram_ProgramDayItemNote", t => t.AssignedProgram_ProgramDayItemNoteId)
                .Index(t => t.AssignedProgram_ProgramDayItemNoteId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AssignedProgram_NoteDisplayWeek", "AssignedProgram_ProgramDayItemNoteId", "dbo.AssignedProgram_ProgramDayItemNote");
            DropIndex("dbo.AssignedProgram_NoteDisplayWeek", new[] { "AssignedProgram_ProgramDayItemNoteId" });
            DropTable("dbo.AssignedProgram_NoteDisplayWeek");
        }
    }
}
