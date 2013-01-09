using System.ComponentModel.DataAnnotations;

namespace SimpleMembership.Models.Login
{
    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }
        public string AccessToken { get; set; }
        public string ExternalLoginData { get; set; }
    }
}