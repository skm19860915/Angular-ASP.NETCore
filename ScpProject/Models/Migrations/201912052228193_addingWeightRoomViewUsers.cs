namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addingWeightRoomViewUsers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WeightRoomAccounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Token = c.String(),
                        OrganizationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Organizations", t => t.OrganizationId)
                .Index(t => t.OrganizationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WeightRoomAccounts", "OrganizationId", "dbo.Organizations");
            DropIndex("dbo.WeightRoomAccounts", new[] { "OrganizationId" });
            DropTable("dbo.WeightRoomAccounts");
        }
    }
}
