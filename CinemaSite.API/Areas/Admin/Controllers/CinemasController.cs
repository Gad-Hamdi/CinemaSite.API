using cinemaSite.API.DTOs.Request;
using CinemaSite.API.Models;
using CinemaSite.API.Repositories.IRepositories;
using CinemaSite.API.Utitlity;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CinemaSite.API.Areas.Admin.Controllers
{
    [Area(SD.AdminArea)]

    [Route("api/[Area]/[controller]")]
    [ApiController]
    public class CinemasController : ControllerBase
    {
        private readonly IRepository<Cinema> _CinemaRepository;
        public CinemasController(IRepository<Cinema> CinemaRepository)
        {
            _CinemaRepository = CinemaRepository;
        }
        //get all Cinemas

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var Cinemas = await _CinemaRepository.GetAsync();
            return Ok(Cinemas);
        }

        //get Cinema by id (Edit)

        [HttpGet("{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var Cinema = await _CinemaRepository.GetOneAsync(c => c.Id == id);
            if (Cinema == null)
            {
                return NotFound();
            }
            return Ok(Cinema);
        }
        //(post)create Cinema
        [HttpPost]
        public async Task<IActionResult> Create(CinemaRequest CinemaRequest)
        {

            await _CinemaRepository.CreateAsync(CinemaRequest.Adapt<Cinema>());
            await _CinemaRepository.CommitAsync();
            return Ok(new { msg = "Cinema has been saved successfully" });
        }
        //(put)update Cinema
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CinemaRequest CinemaRequest)
        {
            var Cinema = await _CinemaRepository.GetOneAsync(c => c.Id == id);
            if (Cinema == null)
            {
                return NotFound();
            }
            Cinema.Name = CinemaRequest.Name;
            Cinema.CinemaLogo = CinemaRequest.CinemaLogo;
            Cinema.Description = CinemaRequest.Description;
            Cinema.Address = CinemaRequest.Address;

            _CinemaRepository.Update(Cinema);
            await _CinemaRepository.CommitAsync();
            return Ok(new { msg = "Cinema has been updated successfully" });
        }
        //(delete)delete Cinema
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var Cinema = await _CinemaRepository.GetOneAsync(c => c.Id == id);
            if (Cinema == null)
            {
                return NotFound();
            }
            _CinemaRepository.Delete(Cinema);
            await _CinemaRepository.CommitAsync();
            return Ok(new { msg = "Cinema has been deleted successfully" });
        }
    }
}
