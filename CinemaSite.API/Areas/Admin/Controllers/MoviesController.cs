using CinemaSite.API.DTOs.Request;
using CinemaSite.API.DTOs.Response;
using CinemaSite.API.Models;
using CinemaSite.API.Repositories;
using CinemaSite.API.Repositories.IRepositories;
using CinemaSite.API.Utitlity;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CinemaSite.API.Areas.Admin.Controllers
{
    [Area(SD.AdminArea)]

    [Route("api/[area]/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
          

        private readonly IRepository<Movie> _movieRepository;
        
        public MoviesController(IRepository<Movie> movieRepository)
        {
            _movieRepository = movieRepository;            
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var movies = await _movieRepository.GetAsync(includes: [e => e.Cinema, e => e.Category]);

             var movieResponse=movies.Adapt<List<MovieResponse>>();
            return Ok(movieResponse);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var movie = await _movieRepository.GetOneAsync(m => m.Id == id);
            if (movie == null) return NotFound();

            return Ok(movie.Adapt<MovieResponse>());

        }


        [HttpPost]
        public async Task<IActionResult> Create([FromForm] MovieCreateRequest movieCreateRequest)
        {
            if (movieCreateRequest.ImgUrl is not null && movieCreateRequest.ImgUrl.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(movieCreateRequest.ImgUrl.FileName);

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    await movieCreateRequest.ImgUrl.CopyToAsync(stream);
                }

                var movie = movieCreateRequest.Adapt<Movie>();
                movie.ImgUrl = fileName;

                var MovieReturned = await _movieRepository.CreateAsync(movie);
                await _movieRepository.CommitAsync();

                
                return CreatedAtAction(nameof(Details), new { id = MovieReturned.Id }, new
                {
                    msg = "Created Movie Successfully"
                });
            }

            return BadRequest();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromForm] MovieUpdateRequest MovieUpdateRequest)
        {

            var MovieInDB = await _movieRepository.GetOneAsync(e => e.Id == id, tracked: false);

            if (MovieInDB is null)
                return BadRequest();

            var Movie = MovieUpdateRequest.Adapt<Movie>();
            Movie.Id = id;

            if (MovieUpdateRequest.ImgUrl is not null && MovieUpdateRequest.ImgUrl.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(MovieUpdateRequest.ImgUrl.FileName);

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

                // Save Img in wwwroot
                using (var stream = System.IO.File.Create(filePath))
                {
                    await MovieUpdateRequest.ImgUrl.CopyToAsync(stream);
                }

                // Delete old img from wwwroot
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", MovieInDB.ImgUrl);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }

                // Update img name in DB
                Movie.ImgUrl = fileName;
            }
            else
            {
                Movie.ImgUrl = MovieInDB.ImgUrl;
            }

            // Update in DB
            _movieRepository.Update(Movie);
            await _movieRepository.CommitAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var movie = await _movieRepository.GetOneAsync(e => e.Id == id);

            if (movie is null)
                return NotFound();

            // Delete old img from wwwroot
            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", movie.ImgUrl);
            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }

            // Remove in DB
            _movieRepository.Delete(movie);
            await _movieRepository.CommitAsync();

            return NoContent();
        }
    }

}
