using System.ComponentModel.DataAnnotations.Schema;
namespace Models.MultiMedia
{
    public class Picture
    {
        public int Id { get; set; }
        public string URL { get; set; }
        public string FileName { get; set; }
        public string Thumbnail { get; set; }
        public string Profile { get; set; }
        public int CreatedUserId { get; set; }
        public bool IsDeleted { get; set; }
        [ForeignKey("CreatedUserId")]
        public virtual User.User  CreatedUser { get; set; }

    }
}
