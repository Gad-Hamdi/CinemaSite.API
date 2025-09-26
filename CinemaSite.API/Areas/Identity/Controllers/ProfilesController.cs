using cinemaSite.API.DTOs.Request;
using cinemaSite.API.DTOs.Response;
using CinemaSite.API.DTOs.Request;
using CinemaSite.API.Models;
using CinemaSite.API.Utitlity;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CinemaSite.API.Areas.Identity.Controllers
{
    [Authorize]
    [Area(SD.IdentityArea)]
    [Route("api/[Area]/[controller]")]
    [ApiController]
    public class ProfilesController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfilesController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        [HttpGet("Welcome")]    
        public async Task<IActionResult> Welcome()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            //PersonalInformationVM personalInformationVM = new()
            //{
            //    Email = user.Email,
            //    Id = user.Id,
            //    Name = user.Name,
            //    PhoneNumber = user.PhoneNumber,
            //    State = user.State,
            //    City = user.City,
            //    Street = user.Street,
            //    ZipCode = user.ZipCode
            //};

            var personalInformationResponse = user.Adapt<PersonalInformationResponse>();

            return Ok(personalInformationResponse);
        }
        [HttpPost("UpdateInfo/{userID}")]
        public async Task<IActionResult> UpdateInfo(string userID,PersonalInformationRequest personalInformationRequest)
        {
            

            var user = await _userManager.FindByIdAsync(userID);

            if (user is null)
                return NotFound();

            user.Name = personalInformationRequest.Name;
            user.Email = personalInformationRequest.Email;
            user.PhoneNumber = personalInformationRequest.PhoneNumber;
            user.Street = personalInformationRequest.Street;
            user.City = personalInformationRequest.City;
            user.State = personalInformationRequest.State;
            user.ZipCode = personalInformationRequest.ZipCode;

            await _userManager.UpdateAsync(user);

            return NoContent();
        }
        [HttpPut("ChangePassword/{userID}")]
        public async Task<IActionResult> ChangePassword(string userID, ChangePasswordRequest changePasswordRequest)
        {
           

            var user = await _userManager.FindByIdAsync(userID);

            if (user is null)
                return NotFound();

            var result = await _userManager.ChangePasswordAsync(user, changePasswordRequest.CurrentPassword, changePasswordRequest.NewPassword);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            else
            {
                return Ok(new { msg = "Password Changed Successfully" });
            }

        }



    }
}
