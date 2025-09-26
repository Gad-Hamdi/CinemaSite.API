using Microsoft.AspNetCore.Mvc.Rendering;

namespace CinemaSite.API.Models
{
    public class cinemawithcategoryVM
    {
        public List<Cinema> cinemas { get; set; } = null!;
        public List<SelectListItem> categories { get; set; } = null!;
        public Movie? movie { get; set; }
    }
}
