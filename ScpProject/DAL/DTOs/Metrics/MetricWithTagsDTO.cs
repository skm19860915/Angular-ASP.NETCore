using System.Collections.Generic;

namespace DAL.DTOs.Metrics
{
   public class MetricWithTagsDTO
    {
        public int MetricId { get; set; }
        public List<TagDTO> Tags { get; set; }
    }
}
