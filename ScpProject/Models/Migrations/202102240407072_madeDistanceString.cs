namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class madeDistanceString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ProgramDayItemSuperSet_Set", "Distance", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ProgramDayItemSuperSet_Set", "Distance", c => c.Double());
        }
    }
}
