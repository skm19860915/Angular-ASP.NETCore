namespace Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixKeysParttrois : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.AssignedProgram_CompletedQuestionOpenEnded", "AssignedProgram_ProgramId");
            CreateIndex("dbo.AssignedProgram_CompletedQuestionScale", "AssignedProgram_ProgramId");
            AddForeignKey("dbo.AssignedProgram_CompletedQuestionOpenEnded", "AssignedProgram_ProgramId", "dbo.AssignedProgram_Program", "Id");
            AddForeignKey("dbo.AssignedProgram_CompletedQuestionScale", "AssignedProgram_ProgramId", "dbo.AssignedProgram_Program", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AssignedProgram_CompletedQuestionScale", "AssignedProgram_ProgramId", "dbo.AssignedProgram_Program");
            DropForeignKey("dbo.AssignedProgram_CompletedQuestionOpenEnded", "AssignedProgram_ProgramId", "dbo.AssignedProgram_Program");
            DropIndex("dbo.AssignedProgram_CompletedQuestionScale", new[] { "AssignedProgram_ProgramId" });
            DropIndex("dbo.AssignedProgram_CompletedQuestionOpenEnded", new[] { "AssignedProgram_ProgramId" });
        }
    }
}
