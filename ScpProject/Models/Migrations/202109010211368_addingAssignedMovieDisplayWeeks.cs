namespace Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingAssignedMovieDisplayWeeks : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AssignedProgram_MovieDisplayWeek",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AssignedProgram_ProgramDayItemMovieId = c.Int(nullable: false),
                        DisplayWeek = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AssignedProgram_ProgramDayItemMovie", t => t.AssignedProgram_ProgramDayItemMovieId)
                .Index(t => t.AssignedProgram_ProgramDayItemMovieId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AssignedProgram_MovieDisplayWeek", "AssignedProgram_ProgramDayItemMovieId", "dbo.AssignedProgram_ProgramDayItemMovie");
            DropIndex("dbo.AssignedProgram_MovieDisplayWeek", new[] { "AssignedProgram_ProgramDayItemMovieId" });
            DropTable("dbo.AssignedProgram_MovieDisplayWeek");
        }
    }
}
