namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addedNullDateTIme : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AthleteProgramHistories", "StartDate", c => c.DateTime());
            AlterColumn("dbo.AthleteProgramHistories", "EndDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AthleteProgramHistories", "EndDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.AthleteProgramHistories", "StartDate", c => c.DateTime(nullable: false));
        }
    }
}
