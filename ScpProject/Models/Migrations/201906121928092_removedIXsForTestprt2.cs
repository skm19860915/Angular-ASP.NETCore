namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class removedIXsForTestprt2 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.ProgramDayItems", "IX_ProgramDayItem_ProgramDayId_Position");
            CreateIndex("dbo.ProgramDayItems", "ProgramDayId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.ProgramDayItems", new[] { "ProgramDayId" });
            CreateIndex("dbo.ProgramDayItems", new[] { "ProgramDayId", "Position" }, unique: true, name: "IX_ProgramDayItem_ProgramDayId_Position");
        }
    }
}
