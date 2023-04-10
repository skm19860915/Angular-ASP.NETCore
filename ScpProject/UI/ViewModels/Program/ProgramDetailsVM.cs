using System.Collections.Generic;
using t = Controllers.ViewModels.Tag;
using b = BL.BusinessObjects;


namespace Controllers.ViewModels.Program
{
    public class ProgramDetailsVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<t.Tag> Tags { get; set; }
        public List<b.Program.ProgramDay> Days { get; set; }
        public int WeekCount { get; set; }
    }
}