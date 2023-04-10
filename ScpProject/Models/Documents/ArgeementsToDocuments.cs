using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Documents
{
    public class ArgeementsToDocuments
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int AgreementId { get; set; }
        public DateTime AssignedTime { get; set; }

        [ForeignKey("DocumentId")]
        public virtual Document TargetDocument { get; set; }
        [ForeignKey("AgreementId")]
        public virtual Agreement TargetAgreement { get; set; }
    }
}
