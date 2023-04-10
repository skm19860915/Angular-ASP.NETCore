using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Program.AssignedProgramSnapShots
{
   public class AssignedProgram_ProgramDayItemNote
    {

            public int Id { get; set; }
            public string Name { get; set; }
            public string Note { get; set; }
            public int AssignedProgram_ProgramDayItemId { get; set; }



        [ForeignKey("AssignedProgram_ProgramDayItemId")]
        public virtual AssignedProgram_ProgramDayItem TargetProgramDayItem { get; set; }

    }
}
