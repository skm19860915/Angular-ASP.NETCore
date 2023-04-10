using System.Collections.Generic;

namespace BL.BusinessObjects.MultiMedia
{
    public class Note
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string NoteText { get; set; }
        public List<int> TagIds { get; set; }
    }
}
