using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Athlete;

namespace Models.Documents
{
    public class AssignedDocuments
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int AthleteId { get; set; }
        public DateTime AssignedDate { get; set; }
        public int AssignedByUser { get; set; }

        [ForeignKey("AthleteId")]
        public virtual Athlete.Athlete targetAthlete { get; set; }
        [ForeignKey("DocumentId")]
        public virtual Document targetDocument { get; set; }
        [ForeignKey("UserId")]
        public virtual User.User AssignedUser { get; set; }
    }
}
