namespace Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingCalculatedLoadsTOSnapSHot : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AssignedProgram_ProgramDayItemSuperSet_Set", "PercentMaxCalc", c => c.Double());
            AddColumn("dbo.AssignedProgram_ProgramDayItemSuperSet_Set", "PercentMaxCalcSubPercent", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AssignedProgram_ProgramDayItemSuperSet_Set", "PercentMaxCalcSubPercent");
            DropColumn("dbo.AssignedProgram_ProgramDayItemSuperSet_Set", "PercentMaxCalc");
        }
    }
}
