using System.ComponentModel.DataAnnotations.Schema;

namespace Models.MultiMedia
{
    public class Note
    {
        public int Id { get; set; }
        public string NoteText { get; set; }
        public string Title { get; set; }
        public int CreatedUserId { get; set; }

        [ForeignKey("CreatedUserId")]
        public virtual User.User CreatedUser { get; set; }
    }
}
