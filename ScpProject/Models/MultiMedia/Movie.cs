using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.MultiMedia
{
    public class Movie
    {
        public int Id { get; set; }
        public string URL { get; set; }
        [Index("IX_CreatedUserId_Name_Movies", IsUnique = true, Order = 2), StringLength(200)]
        public string Name { get; set; }
        [Index("IX_CreatedUserId_Name_Movies", IsUnique = true, Order = 1)]
        public int CreatedUserId { get; set; }
        public bool CanModify { get; set; }
        public int OrganizationId { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey("CreatedUserId")]
        public virtual User.User CreatedUser { get; set; }
    }
}
