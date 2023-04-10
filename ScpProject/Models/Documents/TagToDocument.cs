using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Documents
{
    [Table("TagsToDocuments")]
    public class TagToDocument
    {
        public int Id { get; set; }
        public int TagId { get; set; }
        public int DocumentId { get; set; }

        [ForeignKey("TagId")]
        public virtual DocumentTag Tag { get; set; }

        [ForeignKey("DocumentId")]
        public virtual Document TaggedDocument { get; set; }
    }
}
