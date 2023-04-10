using System.Collections.Generic;
using b = BL.BusinessObjects;

namespace BL.BusinessObjects.Survey
{
    public class Survey
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CreatedUserId { get; set; }
        public bool CanModify { get; set; }
        public bool IsDeleted { get; set; }
        public List<b.Tag> Tags { get; set; }
    }
}
