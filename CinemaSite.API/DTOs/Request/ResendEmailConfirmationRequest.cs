using System.ComponentModel.DataAnnotations;

namespace cinemaSite.API.DTOs.Request
{
    public class ResendEmailConfirmationRequest
    {
        [Required]
        public string EmailORUserName { get; set; } = string.Empty;
    }
}
