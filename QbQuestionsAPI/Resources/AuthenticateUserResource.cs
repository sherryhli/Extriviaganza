using System.ComponentModel.DataAnnotations;

namespace QbQuestionsAPI.Resources
{
    public class AuthenticateUserResource
    {
        [Required]
        [MaxLength(30)]
        public string Username { get; set; }

        [Required]
        [MaxLength(50)]
        public string Password { get; set; }
    }
}