
namespace BL.BusinessObjects.SetsAndReps
{
    public class Set
    {
        public int Id { get; set; }
        public int Position { get; set; }
        public int? Sets { get; set; }
        public int? Reps { get; set; }
        public double? Percent { get; set; }
        public double? Weight { get; set; }
        public int ParentWeekId { get; set; }
        public int? Seconds { get; set; }
        public int? Minutes { get; set; }
        public string Distance { get; set; }
        public bool? RepsAchieved { get; set; }
        public string Other { get; set; }
    }
}
