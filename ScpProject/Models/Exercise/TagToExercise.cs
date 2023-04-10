using System.ComponentModel.DataAnnotations.Schema;
namespace Models.Exercise

{
    [Table("TagsToExercises")]
    public class TagToExercise
    {
        public int Id { get; set; }
        public int TagId { get; set; }
        public int ExerciseId { get; set; }

        [ForeignKey("TagId")]
        public virtual ExerciseTag Tag { get; set; }

        [ForeignKey("ExerciseId")]
        public virtual Exercise TaggedExercise { get; set; }
    }
}
