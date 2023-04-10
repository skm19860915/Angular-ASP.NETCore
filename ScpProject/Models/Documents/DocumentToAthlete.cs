using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Documents
{
    public class DocumentToAthlete
    {
        public int Id { get; set; }
        public int AthleteId { get; set; }
        public int DocumentId { get; set; }
        public DateTime AssignedDate { get; set; }
        public int AssignedByUser { get; set; }
    }
}
