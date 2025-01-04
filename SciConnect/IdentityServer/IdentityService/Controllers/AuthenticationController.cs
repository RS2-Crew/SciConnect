using AutoMapper;
using IdentityServer.Entities;
using IdentityService.Controllers.Base;
using IdentityService.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace IdentityService.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]

    public class AuthenticationController : RegistrationController
    {
        public AuthenticationController(ILogger<AuthenticationController> logger, IMapper mapper, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IMemoryCache memoryCache) : 
            base(logger, mapper, userManager, roleManager, memoryCache)
        {
        }

        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GenerateVerificationCode([FromBody] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest("Email is required.");
            }

            var verificationCode = Guid.NewGuid().ToString().Substring(0, 6); // Generisanje koda
            StoreVerificationCode(email, verificationCode);

            // Simulacija slanja koda (možete povezati sa email servisom)
            _logger.LogInformation($"Verification code for {email}: {verificationCode}");

            return Ok("Verification code generated and stored.");
        }


        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterGuest([FromBody] NewUserDTO newUser)
        {
            return await RegisterNewUserWithRoles(newUser, new string[] { "Guest" });
        }

        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterAdministrator([FromBody] NewUserDTO newUser)
        {

            if (!ValidateVerificationCode(newUser.Email, newUser.VerificationCode))
            {
                return BadRequest("Invalid or expired verification code.");
            }

            return await RegisterNewUserWithRoles(newUser, new[] { "Administrator" });
        }

    }

}
