using System.Collections.Generic;

namespace BL.BusinessObjects.MultiMedia
{
    public class Movie
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
        public List<Tag> Tags { get; set; }
        public bool CanModify { get; set; }
        public bool IsDeleted { get; set; }
    }
}
