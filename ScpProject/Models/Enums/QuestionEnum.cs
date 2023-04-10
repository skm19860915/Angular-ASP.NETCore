using System.ComponentModel.DataAnnotations.Schema;
using Extensions;

namespace Models.Enums
{
    public class QuestionType
    {
        public QuestionType()
        { }
        private QuestionType(QuestionTypeEnum @enum)
        {
            Id = (int)@enum;
            Name = @enum.ToString();
            Description = @enum.GetEnumDescription();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public static implicit operator QuestionType(QuestionTypeEnum @enum) => new QuestionType(@enum);

        public static implicit operator QuestionTypeEnum(QuestionType questionType) => (QuestionTypeEnum) questionType.Id;
    }
    public enum QuestionTypeEnum
    {
        Boolean = 1,
        Scale = 2,
        OpenTest = 3
    }
}
