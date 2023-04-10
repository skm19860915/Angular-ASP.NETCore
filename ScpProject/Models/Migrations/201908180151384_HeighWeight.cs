namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class HeighWeight : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AthleteHeightWeights",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AthleteId = c.Int(nullable: false),
                        HeightPrimary = c.Double(),
                        HeightSecondary = c.Double(),
                        Weight = c.Double(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Athletes", t => t.AthleteId)
                .Index(t => t.AthleteId);
            
            AddColumn("dbo.Athletes", "Birthday", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AthleteHeightWeights", "AthleteId", "dbo.Athletes");
            DropIndex("dbo.AthleteHeightWeights", new[] { "AthleteId" });
            DropColumn("dbo.Athletes", "Birthday");
            DropTable("dbo.AthleteHeightWeights");
        }
    }
}
