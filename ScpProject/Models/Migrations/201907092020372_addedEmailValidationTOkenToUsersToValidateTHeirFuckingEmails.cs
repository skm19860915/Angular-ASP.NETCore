namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addedEmailValidationTOkenToUsersToValidateTHeirFuckingEmails : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "EmailValidationToken", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "EmailValidationToken");
        }
    }
}
