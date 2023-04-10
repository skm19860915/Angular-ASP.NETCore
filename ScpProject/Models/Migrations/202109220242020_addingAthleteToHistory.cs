namespace Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingAthleteToHistory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AssignedProgram_AssignedProgramHistory", "AthleteId", c => c.Int(nullable: false));
            CreateIndex("dbo.AssignedProgram_AssignedProgramHistory", "AthleteId");
            AddForeignKey("dbo.AssignedProgram_AssignedProgramHistory", "AthleteId", "dbo.Athletes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AssignedProgram_AssignedProgramHistory", "AthleteId", "dbo.Athletes");
            DropIndex("dbo.AssignedProgram_AssignedProgramHistory", new[] { "AthleteId" });
            DropColumn("dbo.AssignedProgram_AssignedProgramHistory", "AthleteId");
        }
    }
}
