using System;

namespace DAL.DTOs.Program
{
    public class ProgramHistory
    {
        public int ProgramId { get; set; }
        public int AssignedprogramId { get; set; }
        public int AssignedProgramHistoryId { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
