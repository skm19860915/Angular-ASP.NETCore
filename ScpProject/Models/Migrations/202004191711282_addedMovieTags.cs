namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addedMovieTags : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MovieTags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedUserId = c.Int(nullable: false),
                        Notes = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedUserId)
                .Index(t => t.CreatedUserId);
            
            CreateTable(
                "dbo.TagsToMovies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TagId = c.Int(nullable: false),
                        MovieId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MovieTags", t => t.TagId)
                .ForeignKey("dbo.Movies", t => t.MovieId)
                .Index(t => t.TagId)
                .Index(t => t.MovieId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TagsToMovies", "MovieId", "dbo.Movies");
            DropForeignKey("dbo.TagsToMovies", "TagId", "dbo.MovieTags");
            DropForeignKey("dbo.MovieTags", "CreatedUserId", "dbo.Users");
            DropIndex("dbo.TagsToMovies", new[] { "MovieId" });
            DropIndex("dbo.TagsToMovies", new[] { "TagId" });
            DropIndex("dbo.MovieTags", new[] { "CreatedUserId" });
            DropTable("dbo.TagsToMovies");
            DropTable("dbo.MovieTags");
        }
    }
}
