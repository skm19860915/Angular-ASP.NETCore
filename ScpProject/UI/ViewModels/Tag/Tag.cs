using m = Models.Enums;

namespace Controllers.ViewModels.Tag
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public m.TagEnum Type { get; set; }
    }
}