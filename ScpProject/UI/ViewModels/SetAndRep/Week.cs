using System.Collections.Generic;

namespace Controllers.ViewModels.SetAndRep
{
    public class Week
    {
        public int Id { get; set; }
        public int Position { get; set; }
        public List<Set> SetsAndReps { get; set; }
        public int ParentWorkoutId { get; set; }
    }
}