using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Survey
{
    public class ScaleThresholdToCoach
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ScaleThresholdId { get; set; }

        [ForeignKey("UserId")]
        public virtual User.User Coach { get; set; }
        [ForeignKey("ScaleThresholdId")]
        public virtual ScaleQuestionThreshold ScaleThreshold { get; set; }
    }
}
