using System.Collections.Generic;

namespace DAL.DTOs
{
    public class SurveyWithTagsDTO
    {
        public int SurveyId { get; set; }
        public List<TagDTO> Tags { get; set; }
    }
}
