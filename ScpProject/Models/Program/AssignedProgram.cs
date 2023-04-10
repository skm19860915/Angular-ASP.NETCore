using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Program
{
    public class AssignedProgram

    {
        [Index("IX_AssignedProgram", 1,IsUnique =true)]
        public int Id { get; set; }
        [Index("IX_AssignedProgram", 2,IsUnique = true)]
        public DateTime AssignedDate {get;set;}
        public int ProgramId { get; set; }
        [ForeignKey("ProgramId")]
        public virtual Program TargetProgram { get; set; }
    }
}
