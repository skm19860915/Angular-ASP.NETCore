using Models.Metric;
using System.Collections.Generic;
using t = Controllers.ViewModels.Tag;
using DAL.DTOs;

namespace Controllers.ViewModels.Roster
{
    public class AthleteVM
    {
        public AthleteDTO Athlete { get; set; }
        public List<AddedMetric> Metrics { get; set; }
        public List<t.Tag> AthleteTags { get; set; }
    }

}
