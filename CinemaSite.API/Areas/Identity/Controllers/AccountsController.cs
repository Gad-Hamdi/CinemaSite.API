using cinemaSite.API.DTOs.Request;
using CinemaSite.API.DTOs.Request;
using CinemaSite.API.Models;
using CinemaSite.API.Repositories.IRepositories;
using CinemaSite.API.Utitlity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using RegisterRequest = CinemaSite.API.DTOs.Request.RegisterRequest;
namespace CinemaSite.API.Areas.Customer.Controllers
{
    [Area(SD.IdentityArea)]
    [Route("api/[Area]/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly IEmailSender _emailSender;
            private readonly SignInManager<ApplicationUser> _signInManager;
            private readonly IRepository<UserOTP> _userOTP;

            public AccountsController(UserManager<ApplicationUser> userManager, IEmailSender emailSender, SignInManager<ApplicationUser> signInManager, IRepository<UserOTP> userOTP)
            {
                _userManager = userManager;
                _emailSender = emailSender;
                _signInManager = signInManager;
                _userOTP = userOTP;
            }
        [HttpPost]
        [Route("Register")] 
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            

            ApplicationUser applicationUser = new()
            {
                UserName = registerRequest.UserName,
                Name = registerRequest.Name,
                Email = registerRequest.Email,
            };

            var result = await _userManager.CreateAsync(applicationUser, registerRequest.Password);
            // Password must contain: min 6 chars, digits, special chars, capital chars, small chars

            if (!result.Succeeded)
            {
                
                return BadRequest(result.Errors);
            }

            // Send Email confirmation
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser);

            var link = Url.Action(nameof(ConfirmEmail), "Account", new { area = "Identity", userId = applicationUser.Id, Token = token }, Request.Scheme);
            await _emailSender.SendEmailAsync(applicationUser.Email, "Confirm Your Account!", $"<h1>Confirm Your Account By Clicking <a href='{link}'>here</a></h1>");

            return Ok(new { msg = "Add User successfully, Please Confirm Your Account" });
        
        }


        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail( string userId,  string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return BadRequest(new { message = "User ID and token are required" });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // Handle URL encoding issues
            token = token.Replace(" ", "+");

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(new { message = "Email confirmed successfully. You can now login." });
        }

        [HttpPost ("Login")]
        public async Task<IActionResult> Login(cinemaSite.API.DTOs.Request.LoginRequest loginRequest)
        {
           

            var user = await _userManager.FindByNameAsync(loginRequest.EmailORUserName) ?? await _userManager.FindByEmailAsync(loginRequest.EmailORUserName);

            if (user is null)
            {
                return BadRequest(new { msg = "Invalid User Name/Email Or Password" });
            }

            var result = await _userManager.CheckPasswordAsync(user, loginRequest.Password);

            if (!result)
            {
                return BadRequest(new { msg = "Invalid User Name/Email Or Password" });
            }

            if (!user.EmailConfirmed)
            {
                return BadRequest(new { msg = "Please Confirm Your Email" }); 
            }

            if (!user.LockoutEnabled)
            {
                 
                return BadRequest(new { msg = $"You have a block till {user.LockoutEnd}" });
            }

            await _signInManager.SignInAsync(user, loginRequest.RememberMe);
            return Ok(new { msg = "Login Successfully" });
        }

        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { msg = "Logout Successfully" });
        }
        [HttpPost("ResendEmailConfirmation")]
        public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfirmationRequest resendEmailConfirmationRequest)
        {
            
            var user = await _userManager.FindByNameAsync(resendEmailConfirmationRequest.EmailORUserName) ?? await _userManager.FindByEmailAsync(resendEmailConfirmationRequest.EmailORUserName);

            if (user is null)
            {
               return BadRequest(new { msg = "Invalid User Name/Email" });
            }

            // Send Email confirmation
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var link = Url.Action(nameof(ConfirmEmail), "Account", new { area = "Identity", userId = user.Id, Token = token }, Request.Scheme);
            await _emailSender.SendEmailAsync(user.Email!, "Confirm Your Account!", $"<h1>Confirm Your Account By Clicking <a href='{link}'>here</a></h1>");

            return Ok(new { msg = "Please Confirm Your Account" });

        }
        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordRequest forgetPasswordRequest)
        {
            

            var user = await _userManager.FindByNameAsync(forgetPasswordRequest.EmailORUserName) ?? await _userManager.FindByEmailAsync(forgetPasswordRequest.EmailORUserName);

            if (user is null)
            {
                return BadRequest(new { msg = "Invalid User Name/Email" });
            }

            // Send Email confirmation
            //var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var OTPNumber = new Random().Next(1000, 9999);

            await _emailSender.SendEmailAsync(user.Email!, "Reset Your Account!", $"Use this OTP Number: <b>{OTPNumber}</b> to reset your account. Don't share it.");

            await _userOTP.CreateAsync(new UserOTP()
            {
                ApplicationUserId = user.Id,
                OTPNumber = OTPNumber.ToString(),
                ValidTo = DateTime.UtcNow.AddDays(1)
            });
            await _userOTP.CommitAsync();

            return Ok(new 
            { 
                msg = "Please Check Your Email", 
                userId=user.Id
            });
        }
        [HttpPost("ConfirmOTP")]
        public async Task<IActionResult> ConfirmOTP(ConfirmOTPRequest confirmOTPRequest)
        {
           
            var user = await _userManager.FindByIdAsync(confirmOTPRequest.ApplicationUserId);

            if (user is null)
                return NotFound();

            var lstOTP = (await _userOTP.GetAsync(e => e.ApplicationUserId == confirmOTPRequest.ApplicationUserId)).OrderBy(e => e.Id).LastOrDefault();

            if (lstOTP is null)
                return NotFound();

            if (lstOTP.OTPNumber == confirmOTPRequest.OTPNumber && lstOTP.ValidTo > DateTime.UtcNow)
            {
                return Ok(new { msg = "OTP is valid", userId = user.Id });
            }

            return BadRequest(new{ msg = "Invalid OTP" });
        }

        [HttpPost("NewPassword")]
        public async Task<IActionResult> NewPassword(NewPasswordRequest newPasswordRequest)
        {
            
            var user = await _userManager.FindByIdAsync(newPasswordRequest.ApplicationUserId);

            if (user is null)
                return NotFound();

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPasswordRequest.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }


            return Ok(new { msg = "Password Changed Successfully" });
        }




    }

}
