using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Documents
{
    public class Document
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Contents { get; set; }
        public int CreatedUserId { get; set; }
        public int OrganizationId { get; set; }
        public bool IsLocked { get; set; }

        [ForeignKey("OrganizationId")]
        public virtual Organization.Organization OwnerOrganization { get; set; }

        [ForeignKey("CreatedUserId")]
        public virtual User.User CreatedUser { get; set; }
    }
}
