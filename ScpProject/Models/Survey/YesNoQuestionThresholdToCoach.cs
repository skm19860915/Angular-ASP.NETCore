using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Survey
{
    public class YesNoQuestionThresholdToCoach
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int YesNoThresholdId { get; set; }

        [ForeignKey("UserId")]
        public virtual User.User Coach { get; set; }
        [ForeignKey("YesNoThresholdId")]
        public virtual YesNoQuestionThreshold ScaleThreshold { get; set; }
    }
}
