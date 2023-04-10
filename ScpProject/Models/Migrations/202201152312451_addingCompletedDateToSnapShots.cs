namespace Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingCompletedDateToSnapShots : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AssignedProgram_AssignedProgramHistory", "CompletedDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AssignedProgram_AssignedProgramHistory", "CompletedDate");
        }
    }
}
