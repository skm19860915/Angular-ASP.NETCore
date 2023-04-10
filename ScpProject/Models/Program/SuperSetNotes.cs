using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Program
{
    public class SuperSetNote
    {
        public int Id { get; set; }
        public string Note { get; set; }
        public int ProgramDayItemSuperSetId { get; set; }
        public int Position { get; set; }


        [ForeignKey("ProgramDayItemSuperSetId")]
        public virtual ProgramDayItemSuperSet TargetProgramDayItemSuperSet { get; set; }
    }
    public class SuperSetNoteDisplayWeek
    {
        public int Id { get; set; }
        public int SuperSetNoteId { get; set; }
        public int DisplayWeek { get; set; }
        [ForeignKey("SuperSetNoteId")]
        public virtual SuperSetNote TargetNote { get; set; }
    }
}
