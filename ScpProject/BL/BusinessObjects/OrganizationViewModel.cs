using m = Models;
namespace BL.BusinessObjects
{
    public class OrganizationViewModel
    {
        public m.Organization.Organization Org { get; set; }
        public string profilePictureURL { get; set; }
        public string thumbnailPictureURL { get; set; }
    }
}
