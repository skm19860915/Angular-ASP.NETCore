namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addedEmailTokenValidation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Athletes", "EmailValidationToken", c => c.String());
            AddColumn("dbo.Athletes", "TokenIssued", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Athletes", "TokenIssued");
            DropColumn("dbo.Athletes", "EmailValidationToken");
        }
    }
}
