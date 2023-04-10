using System.ComponentModel.DataAnnotations.Schema;

namespace Models.SetsAndReps
{
    [Table("TagsToWorkouts")]
    public class TagToWorkout  
    {

        public int Id { get; set; }
        public int WorkoutId { get; set; }
        public int TagId { get; set; }
        

        [ForeignKey("TagId")]
        public virtual WorkoutTag Tag { get; set; }
        [ForeignKey("WorkoutId")]
        public virtual Workout Workout { get; set; }
    }
}
