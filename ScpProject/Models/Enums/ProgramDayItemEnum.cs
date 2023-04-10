using System.ComponentModel.DataAnnotations.Schema;
using Extensions;

namespace Models.Enums
{
    public class ProgramDayItemType
    {
        public ProgramDayItemType() { }
        private ProgramDayItemType(ProgramDayItemEnum @enum)
        {
            Id = (int)@enum;
            Name = @enum.ToString();
            Description = @enum.GetEnumDescription();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public static implicit operator ProgramDayItemType(ProgramDayItemEnum @enum) => new ProgramDayItemType(@enum);

        public static implicit operator ProgramDayItemEnum(ProgramDayItemType questionType) => (ProgramDayItemEnum)questionType.Id;
    }

    public enum ProgramDayItemEnum
    {
        Workout = 1,
        Survey = 2,
        Photo = 3,
        Video = 4,
        Note = 5,
        Metric = 6,
        SuperSet = 7
    }
}
