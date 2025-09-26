using System.ComponentModel.DataAnnotations;

namespace cinemaSite.API.DTOs.Request
{
    public class ConfirmOTPRequest
    {
        [Required]
        public string OTPNumber { get; set; } = string.Empty;
        public string ApplicationUserId { get; set; } = string.Empty;
    }
}
