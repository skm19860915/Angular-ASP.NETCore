namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class updatedLengthOfQuestionsTo300 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Questions", "IX_QuestionDisplayText_CreatedUserId_IsDeleted");
            AlterColumn("dbo.Questions", "QuestionDisplayText", c => c.String(maxLength: 300));
            CreateIndex("dbo.Questions", new[] { "CreatedUserId", "QuestionDisplayText", "IsDeleted" }, unique: true, name: "IX_QuestionDisplayText_CreatedUserId_IsDeleted");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Questions", "IX_QuestionDisplayText_CreatedUserId_IsDeleted");
            AlterColumn("dbo.Questions", "QuestionDisplayText", c => c.String(maxLength: 100));
            CreateIndex("dbo.Questions", new[] { "CreatedUserId", "QuestionDisplayText", "IsDeleted" }, unique: true, name: "IX_QuestionDisplayText_CreatedUserId_IsDeleted");
        }
    }
}
