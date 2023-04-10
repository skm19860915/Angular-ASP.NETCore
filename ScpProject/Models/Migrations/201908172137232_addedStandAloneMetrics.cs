namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addedStandAloneMetrics : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AddedMetrics",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MetricId = c.Int(nullable: false),
                        Value = c.Double(nullable: false),
                        CompletedDate = c.DateTime(nullable: false),
                        AthleteId = c.Int(nullable: false),
                        EnteredByUserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Athletes", t => t.AthleteId)
                .ForeignKey("dbo.Users", t => t.EnteredByUserId)
                .ForeignKey("dbo.Metrics", t => t.MetricId)
                .Index(t => t.MetricId)
                .Index(t => t.AthleteId)
                .Index(t => t.EnteredByUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AddedMetrics", "MetricId", "dbo.Metrics");
            DropForeignKey("dbo.AddedMetrics", "EnteredByUserId", "dbo.Users");
            DropForeignKey("dbo.AddedMetrics", "AthleteId", "dbo.Athletes");
            DropIndex("dbo.AddedMetrics", new[] { "EnteredByUserId" });
            DropIndex("dbo.AddedMetrics", new[] { "AthleteId" });
            DropIndex("dbo.AddedMetrics", new[] { "MetricId" });
            DropTable("dbo.AddedMetrics");
        }
    }
}
