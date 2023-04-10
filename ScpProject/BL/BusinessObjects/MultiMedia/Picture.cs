using System.Collections.Generic;

namespace BL.BusinessObjects.MultiMedia
{
    public  class Picture
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string URL { get; set; }
        public  List<int> TagIds { get; set; }
    }
}
