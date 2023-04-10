using Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Enums
{
    public class QuestionThreshold
    {
        public QuestionThreshold()
        { }
        private QuestionThreshold(QuestionThresholdEnum @enum)
        {
            Id = (int)@enum;
            Name = @enum.ToString();
            Description = @enum.GetEnumDescription();
        }


        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public static implicit operator QuestionThreshold(QuestionThresholdEnum @enum) => new QuestionThreshold(@enum);

        public static implicit operator QuestionThresholdEnum(QuestionThreshold questionType) => (QuestionThresholdEnum)questionType.Id;
    }
    public enum QuestionThresholdEnum
    {
        Equal = 1,
        GreaterThan = 2,
        LessThan = 3,
        EqualToOrGreaterThan = 4,
        EqualToOrLessThan = 5
    }
}
