using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Program.AssignedProgramSnapShots
{
    public class AssignedProgram_ProgramDayItemSurvey
    {
        public int Id { get; set; }
        public int SurveyId { get; set; }
        public int AssignedProgram_ProgramDayItemId { get; set; }

        [ForeignKey("SurveyId")]
        public virtual Models.Survey.Survey Survey { get; set; }

        [ForeignKey("AssignedProgram_ProgramDayItemId")]
        public virtual AssignedProgram_ProgramDayItem TargetProgramDayItem { get; set; }
    }
}
