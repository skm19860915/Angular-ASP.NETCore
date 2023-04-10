using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Program
{
    public class ProgramDayItemMovie
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public int ProgramDayItemId { get; set; }

        [ForeignKey("MovieId")]
        public virtual Models.MultiMedia.Movie Movie { get; set; }
        [ForeignKey("ProgramDayItemId")]
        public virtual ProgramDayItem TargetProgramDayItem { get; set; }
    }
}
