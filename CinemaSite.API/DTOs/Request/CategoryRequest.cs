using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace cinemaSite.API.DTOs.Request
{
    public class CategoryRequest
    {
        [Required]
        [MaxLength(50)]
        [MinLength(2)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Category name must contain only letters.")]
        public string Name { get; set; }= string.Empty;
    }
}