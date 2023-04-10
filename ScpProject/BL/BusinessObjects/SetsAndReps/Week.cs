using System.Collections.Generic;

namespace BL.BusinessObjects.SetsAndReps
{
    public class Week
    {
        public int Id { get; set; }
        public int Position { get; set; }
        public int ParentWorkoutId { get; set; }
        public virtual List<Set> SetsAndReps { get; set; }
    }
}
