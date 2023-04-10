namespace Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingNotesToAssignedProgram : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AssignedProgram_ProgramDayItemNote",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Note = c.String(),
                        ProgramDayItemId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.ProgramDayItemId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.AssignedProgram_ProgramDayItemNote");
        }
    }
}
