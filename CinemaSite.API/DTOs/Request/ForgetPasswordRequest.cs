using System.ComponentModel.DataAnnotations;

namespace cinemaSite.API.DTOs.Request
{
    public class ForgetPasswordRequest
    {
        [Required]
        public string EmailORUserName { get; set; } = string.Empty;
    }
}
