using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Controllers.ViewModels.Documents
{
    public class DocumentDTO
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public List<Agreement> Agreements { get; set; }
    }
    public class Agreement
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
    }
}