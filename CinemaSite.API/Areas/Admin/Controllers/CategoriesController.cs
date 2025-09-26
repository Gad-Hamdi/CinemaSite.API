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
    public class CategoriesController : ControllerBase
    {
        private readonly IRepository<Category> _categoryRepository;
        public CategoriesController(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        //get all categories

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepository.GetAsync();
            return Ok(categories);
        }

        //get category by id (Edit)

        [HttpGet("{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryRepository.GetOneAsync(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }
        //(post)create category
        [HttpPost]
        public async Task<IActionResult> Create(CategoryRequest categoryRequest)
        {

            await _categoryRepository.CreateAsync(categoryRequest.Adapt<Category>());
            await _categoryRepository.CommitAsync();
            return Ok(new { msg = "Category has been saved successfully" });
        }
        //(put)update category
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CategoryRequest categoryRequest)
        {
            var category = await _categoryRepository.GetOneAsync(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            category.Name = categoryRequest.Name;
            _categoryRepository.Update(category);
            await _categoryRepository.CommitAsync();
            return Ok(new { msg = "Category has been updated successfully" });
        }
        //(delete)delete category
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryRepository.GetOneAsync(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            _categoryRepository.Delete(category);
            await _categoryRepository.CommitAsync();
            return Ok(new { msg = "Category has been deleted successfully" });
        }
    }
}
