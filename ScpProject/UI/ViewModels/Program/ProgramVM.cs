using System.Collections.Generic;
using t = Controllers.ViewModels.Tag;

namespace Controllers.ViewModels.Program
{
    public class ProgramVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<t.Tag> Tags { get; set; }
    }
}