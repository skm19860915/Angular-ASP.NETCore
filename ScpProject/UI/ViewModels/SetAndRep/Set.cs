namespace Controllers.ViewModels.SetAndRep
{
    public class Set
    {
        public int Id { get; set; }
        public int Position { get; set; }
        public double? Percent { get; set; }
        public int? Sets { get; set; }
        public int? Reps { get; set; }
        public int? Weight { get; set; }
        public int? Seconds { get; set; }
        public int? Minutes { get; set; }
        public string Distance { get; set; }
        public bool RepsAchieved { get; set; }
        public string Other { get; set; }
        public int ParentWeekId { get; set; }

    }
}