using System.Collections.Generic;
using e = Models.Enums;
using b = BL.BusinessObjects;

namespace BL.BusinessObjects.MultiMedia
{
    public class MultiMedia
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public e.MediaTypeEnum MediaType  { get; set; }
        public List<b.Tag> Tags { get; set; }
    }
}
