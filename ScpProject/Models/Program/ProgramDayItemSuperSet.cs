using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Program
{
    public class ProgramDayItemSuperSet
    {
        public int Id { get; set; }
        public int ProgramDayItemId { get; set; }

        public string SuperSetDisplayTitle {get;set;}

        [ForeignKey("ProgramDayItemId")]
        public virtual ProgramDayItem TargetProgramDayItem { get; set; }
    }
}
