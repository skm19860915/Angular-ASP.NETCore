namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class removedIXFromProgramDay : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.ProgramDays", "IX_ProgramDay_Position_ProgramId");
            CreateIndex("dbo.ProgramDays", "ProgramId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.ProgramDays", new[] { "ProgramId" });
            CreateIndex("dbo.ProgramDays", new[] { "ProgramId", "Position" }, unique: true, name: "IX_ProgramDay_Position_ProgramId");
        }
    }
}
