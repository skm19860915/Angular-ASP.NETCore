using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Documents
{
    public class Agreement
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedUserId { get; set; }
        public int DocumentId { get; set; }
        public int OrganizationId { get; set; }

        [ForeignKey("DocumentId")]
        public virtual  Document targetDocument { get; set; }
        [ForeignKey("OrganizationId")]
        public virtual Organization.Organization targetOrg { get; set; }
        [ForeignKey("CreatedUserId")]
        public virtual User.User targetUser{ get; set; }
    }
}
