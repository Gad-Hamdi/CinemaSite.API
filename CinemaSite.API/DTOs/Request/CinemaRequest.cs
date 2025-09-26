using CinemaSite.API.Models;
using System.ComponentModel.DataAnnotations;

namespace cinemaSite.API.DTOs.Request
{
    public class CinemaRequest
    {
        public string? Name { get; set; }
        [Required]
        [MaxLength(50)]
        [MinLength(2)]
        public string Description { get; set; } = string.Empty;
        [Required]

        public string CinemaLogo { get; set; } = string.Empty;
        [Required]
        [MaxLength(50)]
        [MinLength(2)]
        public string Address { get; set; } = string.Empty;
    }
}