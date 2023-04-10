namespace Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class madeLastCompletedUpdateTimeNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AssignedProgram_ProgramDayItemSuperSet_Set", "LastCompletedUpdateTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AssignedProgram_ProgramDayItemSuperSet_Set", "LastCompletedUpdateTime", c => c.DateTime(nullable: false));
        }
    }
}
