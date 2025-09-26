using CinemaSite.API.Models;
using System.ComponentModel.DataAnnotations;

namespace CinemaSite.API.DTOs.Response
{
    public class MovieResponse
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = null!;
        public string CinemaName { get; set; } = null!;
        
        [Required]
        [MaxLength(50)]
        [MinLength(2)]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "Movie name must contain only letters, numbers, and spaces.")]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        [Required]

        public string TrailerUrl { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double Price { get; set; }
        public int MovieStatus { get; set; }
        [Required]

        public IFormFile ImgUrl { get; set; } = null!;
        public List<IFormFile>? MultiImageUrl { get; set; } = new List<IFormFile>();


    }
}
