using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Program
{
    [Table("TagsToPrograms")]
    public class TagToProgram
    {
        public int Id { get; set; }
        public int TagId { get; set; }
        public int ProgramId { get; set; }

        [ForeignKey("TagId")]
        public virtual ProgramTag Tag { get; set; }

        [ForeignKey("ProgramId")]
        public virtual Program TaggedProgram { get; set; }
    }
}
