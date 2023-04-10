namespace Models.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addingVideosToPrograms : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MovieDisplayWeeks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProgramDayItemMovieId = c.Int(nullable: false),
                        DisplayWeek = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProgramDayItemMovies", t => t.ProgramDayItemMovieId)
                .Index(t => t.ProgramDayItemMovieId);
            
            CreateTable(
                "dbo.ProgramDayItemMovies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MovieId = c.Int(nullable: false),
                        ProgramDayItemId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Movies", t => t.MovieId)
                .ForeignKey("dbo.ProgramDayItems", t => t.ProgramDayItemId)
                .Index(t => t.MovieId)
                .Index(t => t.ProgramDayItemId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MovieDisplayWeeks", "ProgramDayItemMovieId", "dbo.ProgramDayItemMovies");
            DropForeignKey("dbo.ProgramDayItemMovies", "ProgramDayItemId", "dbo.ProgramDayItems");
            DropForeignKey("dbo.ProgramDayItemMovies", "MovieId", "dbo.Movies");
            DropIndex("dbo.ProgramDayItemMovies", new[] { "ProgramDayItemId" });
            DropIndex("dbo.ProgramDayItemMovies", new[] { "MovieId" });
            DropIndex("dbo.MovieDisplayWeeks", new[] { "ProgramDayItemMovieId" });
            DropTable("dbo.ProgramDayItemMovies");
            DropTable("dbo.MovieDisplayWeeks");
        }
    }
}
