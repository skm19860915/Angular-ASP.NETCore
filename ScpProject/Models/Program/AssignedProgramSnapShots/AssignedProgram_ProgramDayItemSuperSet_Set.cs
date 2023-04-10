using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Program.AssignedProgramSnapShots
{
    public class AssignedProgram_ProgramDayItemSuperSet_Set
    {
        public int Id { get; set; }
        public int Position { get; set; }
        public int? Sets { get; set; }
        public int? Reps { get; set; }
        public double? Percent { get; set; }
        public double? Weight { get; set; }
        public int? Minutes { get; set; }
        public int? Seconds { get; set; }
        public string Distance { get; set; }
        public bool? RepsAchieved { get; set; }
        public string Other { get; set; }
        public int? Completed_Sets { get; set; }
        public int? Completed_Reps { get; set; }
        public double? Completed_Weight { get; set; }
        public int? Completed_RepsAchieved { get; set; }
        public DateTime? LastCompletedUpdateTime { get; set; }
        public double? PercentMaxCalc { get; set; }
        public double? PercentMaxCalcSubPercent { get; set; }
        //public bool DisplayLoad { get; set; }


        public int AssignedProgram_ProgramDayItemSuperSetWeekId { get; set; }
        [ForeignKey("AssignedProgram_ProgramDayItemSuperSetWeekId")]
        public virtual AssignedProgram_ProgramDayItemSuperSetWeek ProgramDayItemSuperSetWeek { get; set; }
    }
}
