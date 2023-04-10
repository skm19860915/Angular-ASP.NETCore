namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class renameValueToThresholdValue : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ScaleQuestionThresholds", "ThresholdValue", c => c.Int(nullable: false));
            DropColumn("dbo.ScaleQuestionThresholds", "Value");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ScaleQuestionThresholds", "Value", c => c.Int(nullable: false));
            DropColumn("dbo.ScaleQuestionThresholds", "ThresholdValue");
        }
    }
}
