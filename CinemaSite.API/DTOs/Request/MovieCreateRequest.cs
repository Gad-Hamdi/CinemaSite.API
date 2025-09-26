using CinemaSite.API.Models;
using System.ComponentModel.DataAnnotations;

namespace CinemaSite.API.DTOs.Request
{
    public class MovieCreateRequest
    {
        public int CategoryId { get; set; }
        public int CinemaId { get; set; }
        [Required]
        [MaxLength(50)]
        [MinLength(2)]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "Movie name must contain only letters, numbers, and spaces.")]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        [Required]

        public string TrailerUrl { get; set; } = string.Empty;
        
        [Required]

        public IFormFile ImgUrl { get; set; } = null!;
        public List<IFormFile> MultiImageUrl { get; set; } = new List<IFormFile>();

    }
}
