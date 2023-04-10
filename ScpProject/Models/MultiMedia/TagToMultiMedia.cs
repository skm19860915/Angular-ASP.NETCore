using System.ComponentModel.DataAnnotations.Schema;


namespace Models.MultiMedia
{
    [Table("TagsToMultiMedia")]
    public class TagToMultiMedia
    {
        public int Id { get; set; }
        public int MultiMediaId { get; set; }
        public int TagId { get; set; }
        public int MultiMediaTypeId { get; set; }

        [ForeignKey("TagId")]
        public virtual MultiMediaTag Tag { get; set; }
    }
}
