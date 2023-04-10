namespace Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class makingCompletedShitOptional : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AssignedProgram_ProgramDayItemSuperSet_Set", "Completed_Sets", c => c.Int());
            AlterColumn("dbo.AssignedProgram_ProgramDayItemSuperSet_Set", "Completed_Reps", c => c.Int());
            AlterColumn("dbo.AssignedProgram_ProgramDayItemSuperSet_Set", "Completed_Weight", c => c.Double());
            AlterColumn("dbo.AssignedProgram_ProgramDayItemSuperSet_Set", "Completed_RepsAchieved", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AssignedProgram_ProgramDayItemSuperSet_Set", "Completed_RepsAchieved", c => c.Int(nullable: false));
            AlterColumn("dbo.AssignedProgram_ProgramDayItemSuperSet_Set", "Completed_Weight", c => c.Double(nullable: false));
            AlterColumn("dbo.AssignedProgram_ProgramDayItemSuperSet_Set", "Completed_Reps", c => c.Int(nullable: false));
            AlterColumn("dbo.AssignedProgram_ProgramDayItemSuperSet_Set", "Completed_Sets", c => c.Int(nullable: false));
        }
    }
}
