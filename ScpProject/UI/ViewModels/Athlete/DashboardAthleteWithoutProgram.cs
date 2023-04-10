using System.Collections.Generic;
using b = BL.BusinessObjects;

namespace Controllers.ViewModels.Athlete
{
    public class DashboardAthleteWithoutProgram
    {
        public int AthleteCount { get; set; }
        public List<b.Athlete> Athletes { get; set; }
    }
}