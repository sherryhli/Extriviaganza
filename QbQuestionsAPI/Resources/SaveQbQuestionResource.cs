using System.ComponentModel.DataAnnotations;

namespace QbQuestionsAPI.Resources
{
    public class SaveQbQuestionResource
    {
        [Required]
        [Range(1, 4)]
        public int Level { get; set; }

        [Required]
        [MaxLength(50)]
        public string Tournament { get; set; }

        [Required]
        public int Year { get; set; }
        
        public string Power { get; set; }

        [Required]
        public string Body { get; set; }

        [Required]
        [MaxLength(50)]
        public string Answer { get; set; }

        [MaxLength(250)]
        public string Notes { get; set; }
    }
}