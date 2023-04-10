using System;
using System.Collections.Generic;
using t = Controllers.ViewModels.Tag;

namespace Controllers.ViewModels.SetAndRep
{
    public class Workout
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public String Notes { get; set; }
        public List<t.Tag> Tags { get; set; }
    }
}