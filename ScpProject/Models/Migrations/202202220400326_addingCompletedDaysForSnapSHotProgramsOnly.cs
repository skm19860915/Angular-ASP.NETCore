namespace Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingCompletedDaysForSnapSHotProgramsOnly : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AssignedProgram_CompletedDay",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AssignedProgram_ProgramDayId = c.Int(nullable: false),
                        WeekId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AssignedProgram_ProgramDay", t => t.AssignedProgram_ProgramDayId)
                .Index(t => t.AssignedProgram_ProgramDayId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AssignedProgram_CompletedDay", "AssignedProgram_ProgramDayId", "dbo.AssignedProgram_ProgramDay");
            DropIndex("dbo.AssignedProgram_CompletedDay", new[] { "AssignedProgram_ProgramDayId" });
            DropTable("dbo.AssignedProgram_CompletedDay");
        }
    }
}
