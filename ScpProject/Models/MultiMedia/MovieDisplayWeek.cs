using Models.Program;
using System.ComponentModel.DataAnnotations.Schema;


namespace Models.MultiMedia
{
   public  class MovieDisplayWeek
    {
        public int Id { get; set; }
        public int ProgramDayItemMovieId { get; set; }
        public int DisplayWeek { get; set; }
        [ForeignKey("ProgramDayItemMovieId")]
        public virtual  ProgramDayItemMovie ProgramDayItemMovie{ get; set; }
    }
}
