namespace Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removedConstraintsFromAssignedProgramTables : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.AssignedProgram_Program", "IX_CreatedUserId_Name_Programs");
            AlterColumn("dbo.AssignedProgram_Program", "Name", c => c.String());
            CreateIndex("dbo.AssignedProgram_Program", "CreatedUserId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.AssignedProgram_Program", new[] { "CreatedUserId" });
            AlterColumn("dbo.AssignedProgram_Program", "Name", c => c.String(maxLength: 400));
            CreateIndex("dbo.AssignedProgram_Program", new[] { "CreatedUserId", "Name" }, unique: true, name: "IX_CreatedUserId_Name_Programs");
        }
    }
}
