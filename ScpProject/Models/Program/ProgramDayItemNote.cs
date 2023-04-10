using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Program
{
    public class ProgramDayItemNote
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public int ProgramDayItemId { get; set; }


        [ForeignKey("ProgramDayItemId")]
        public virtual ProgramDayItem TargetProgramDayItem { get; set; }
    }

   
}
