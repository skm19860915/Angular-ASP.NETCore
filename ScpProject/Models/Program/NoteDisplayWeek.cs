using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Program
{
    public class NoteDisplayWeek
    {
        public int Id { get; set; }
        public int ProgramDayItemNoteId { get; set; }
        public int DisplayWeek { get; set; }

        [ForeignKey("ProgramDayItemNoteId")]
        public virtual ProgramDayItemNote Note { get; set; }

    }
}
