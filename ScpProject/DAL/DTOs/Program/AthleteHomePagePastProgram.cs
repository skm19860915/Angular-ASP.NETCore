using System;

namespace DAL.DTOs.Program
{
    public class AthleteHomePagePastProgram
    {
        public string ProgramName { get; set; }
        public int ProgramId { get; set; }
        public int AssignedProgramId { get; set; }
        public DateTime ProgramStartDate { get; set; }
        public DateTime ProgramEndDate { get; set; }
        public bool IsSnapShot { get; set; }
    }
}
