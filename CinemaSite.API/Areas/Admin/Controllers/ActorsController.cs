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
    public class ActorsController : ControllerBase
    {
        private readonly IRepository<Actor> _ActorRepository;
        public ActorsController(IRepository<Actor> ActorRepository)
        {
            _ActorRepository = ActorRepository;
        }
        //get all actors

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var actors = await _ActorRepository.GetAsync();
            return Ok(actors);
        }

        //get Actor by id (Edit)

        [HttpGet("{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var Actor = await _ActorRepository.GetOneAsync(c => c.Id == id);
            if (Actor == null)
            {
                return NotFound();
            }
            return Ok(Actor);
        }
        //(post)create Actor
        [HttpPost]
        public async Task<IActionResult> Create(ActorRequest actorRequest)
        {

            await _ActorRepository.CreateAsync(actorRequest.Adapt<Actor>());
            await _ActorRepository.CommitAsync();
            return Ok(new { msg = "Actor has been saved successfully" });
        }
        //(put)update Actor
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ActorRequest actorRequest)
        {
            var Actor = await _ActorRepository.GetOneAsync(c => c.Id == id);
            if (Actor == null)
            {
                return NotFound();
            }
            Actor.FirstName = actorRequest.FirstName;
            Actor.LastName = actorRequest.LastName;
            Actor.Bio = actorRequest.Bio;
             Actor.ProfilePicture = actorRequest.ProfilePicture;
            Actor.News = actorRequest.News;


            _ActorRepository.Update(Actor);
            await _ActorRepository.CommitAsync();
            return Ok(new { msg = "Actor has been updated successfully" });
        }
        //(delete)delete Actor
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var Actor = await _ActorRepository.GetOneAsync(c => c.Id == id);
            if (Actor == null)
            {
                return NotFound();
            }
            _ActorRepository.Delete(Actor);
            await _ActorRepository.CommitAsync();
            return Ok(new { msg = "Actor has been deleted successfully" });
        }
    }
}
