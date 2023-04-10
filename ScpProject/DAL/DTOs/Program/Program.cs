using System.Collections.Generic;

namespace DAL.DTOs.Program
{
    public class Program
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CreatedUserId { get; set; }
        public List<ProgramDay> Days { get; set; }
        public int CurrentWorkoutDayId { get; set; }
        public int CurrentWorkoutWeekId { get; set; }
        public int WeekCount { get; set; }

        public bool IsDeleted { get; set; }
        public bool CanModify  { get; set; }
    }
}
