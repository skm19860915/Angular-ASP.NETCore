using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Program
{
    public class ProgramDayItemSuperSet_Set
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
        //public bool DisplayLoad { get; set; }


        public int ProgramDayItemSuperSetWeekId { get; set; }
        [ForeignKey("ProgramDayItemSuperSetWeekId")]
        public virtual ProgramDayItemSuperSetWeek ProgramDayItemSuperSetWeek { get; set; }
    }
}
