using System;

namespace DAL.DTOs.AthleteAssignedPrograms
{
    public class HistoricProgram
    {
        public int AssignedProgramId { get; set; }
        public int ProgramId { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
    }
}
