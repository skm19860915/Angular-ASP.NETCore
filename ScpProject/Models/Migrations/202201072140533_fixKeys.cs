namespace Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixKeys : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.AssignedProgram_ProgramDayItemNote", "AssignedProgram_ProgramDayItemId", name:"IX_2_AssignedProgram_ProgramDayItemId");
            AddForeignKey("dbo.AssignedProgram_ProgramDayItemNote", "AssignedProgram_ProgramDayItemId", "dbo.AssignedProgram_ProgramDayItem", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AssignedProgram_ProgramDayItemNote", "AssignedProgram_ProgramDayItemId", "dbo.AssignedProgram_ProgramDayItem");
            DropIndex("dbo.AssignedProgram_ProgramDayItemNote", new[] { "AssignedProgram_ProgramDayItemId" });
        }
    }
}
