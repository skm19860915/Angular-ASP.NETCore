using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Program
{
    public class ProgramDay
    {
        public int Id { get; set; }
        public int Position { get; set; }
        public int ProgramId { get; set; }

        [ForeignKey("ProgramId")]
        public virtual Program ParentProgram { get; set; }
    }
}
