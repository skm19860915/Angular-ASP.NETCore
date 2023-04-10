namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addingLogging : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LogMessages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Message = c.String(),
                        StackTrace = c.String(),
                        UserId = c.Int(nullable: false),
                        LoggedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.LogMessages");
        }
    }
}
