using System;
using System.Collections.Generic;

namespace Controllers.ViewModels.Roster
{
    public class AssignProgramVM
    {
        public List<int> AthleteIds { get; set; }
        public int ProgramId { get; set; }

        public DateTime StartDate { get; set; }
    }
}