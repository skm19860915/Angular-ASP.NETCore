namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addedAddedDateForWeights : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AthleteHeightWeights", "AddedDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AthleteHeightWeights", "AddedDate");
        }
    }
}
