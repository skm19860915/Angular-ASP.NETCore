using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.SetsAndReps
{
    public class Week
    {
        public int Id { get; set; }
        public int Position { get; set; }
        public int ParentWorkoutId { get; set; }
        public virtual List<Set> SetsAndReps { get; set; }
        [ForeignKey("ParentWorkoutId")]
        public virtual Workout ParentWorkout { get; set; }
    }
}
