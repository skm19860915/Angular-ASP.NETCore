using System.ComponentModel.DataAnnotations.Schema;

namespace Models.MultiMedia
{
    [Table("TagsToMovies")]
    public class TagToMovie
    {
        public int Id { get; set; }
        public int TagId { get; set; }
        public int MovieId { get; set; }

        [ForeignKey("TagId")]
        public virtual MovieTag Tag { get; set; }

        [ForeignKey("MovieId")]
        public virtual Movie TaggedMovie { get; set; }

    }
}
