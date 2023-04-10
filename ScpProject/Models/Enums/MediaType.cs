using System.ComponentModel.DataAnnotations.Schema;
using Extensions;

namespace Models.Enums
{
    public class MediaType
    {
        public MediaType() { }
        private MediaType(MediaTypeEnum @enum)
        {

            Id = (int)@enum;
            Name = @enum.ToString();
            Description = @enum.GetEnumDescription();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public static implicit operator MediaType(MediaTypeEnum @enum) => new MediaType(@enum);

        public static implicit operator MediaTypeEnum(MediaType questionType) => (MediaTypeEnum)questionType.Id;
    }


    public enum MediaTypeEnum
    {
        Note = 1,
        Image = 2,
        Video = 3
    }
}