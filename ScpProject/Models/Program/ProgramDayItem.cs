using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Program
{
    public class ProgramDayItem
    {
        public int Id { get; set; }
        public int ProgramDayId { get; set; }
        public int Position { get; set; }
        public Models.Enums.ProgramDayItemEnum ItemEnum { get; set; }

        [ForeignKey("ProgramDayId")]
        public virtual ProgramDay ParentProgramDay { get; set; }

    }
}
