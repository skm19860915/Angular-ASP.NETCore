using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Tag
{
    public abstract class Tag 
    {
        public int Id { get; set; }
        [Index("IX_UniqueTag",1,IsUnique = true,Order =1)]
        [StringLength(200)]
        public string Name { get; set; }
        
        [Index("IX_UniqueTag", 2, IsUnique = true, Order = 2)]
        public int OrganizationId { get; set; }
        public string Notes { get; set; }
        public bool IsDeleted { get; set; }

        
    }
}
