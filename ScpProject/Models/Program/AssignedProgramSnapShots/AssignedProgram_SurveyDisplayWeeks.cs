using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Program.AssignedProgramSnapShots
{
    public class AssignedProgram_SurveyDisplayWeeks
    {
        public int Id { get; set; }
        public int AssignedProgram_ProgramDayItemSurveyId { get; set; }
        public int DisplayWeek { get; set; }

        [ForeignKey("AssignedProgram_ProgramDayItemSurveyId")]
        public virtual AssignedProgram_ProgramDayItemSurvey ProgramDayItemSurvey { get; set; }
    }
}
