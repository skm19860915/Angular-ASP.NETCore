using System.Collections.Generic;
using t = Controllers.ViewModels.Tag;

namespace Controllers.ViewModels.Survey
{
    public class SurveyVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<t.Tag> Tags { get; set; }

    }
}