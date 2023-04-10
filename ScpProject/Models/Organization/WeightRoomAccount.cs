using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Organization
{
    public class WeightRoomAccount
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }
        public int OrganizationId { get; set; }
        [ForeignKey("OrganizationId")]
        public virtual Organization OwnerOrganization { get; set; }
    }
}
