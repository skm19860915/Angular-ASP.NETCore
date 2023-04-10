using System.Collections.Generic;
using t = Controllers.ViewModels.Tag;

namespace Controllers.ViewModels.MultiMedia
{
    public class MovieVM
    {
        public int Id { get; set; }
        public string URL { get; set; }
        public string Name { get; set; }
        public int CreatedUserId { get; set; }
        public List<t.Tag> Tags { get; set; }
    }
}


