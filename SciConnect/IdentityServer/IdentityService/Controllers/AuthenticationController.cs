using AutoMapper;
using IdentityServer.Entities;
using IdentityService.Controllers.Base;
using IdentityService.DTOs;
using IdentityService.Services;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IAuthenticationService _authService;
        private readonly IConfiguration _configuration;
        public AuthenticationController(ILogger<AuthenticationController> logger, IMapper mapper, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IMemoryCache memoryCache, IAuthenticationService authService, IConfiguration configuration)
       : base(logger, mapper, userManager, roleManager, memoryCache)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(_configuration));
        }
        [Authorize(Roles = "PM")]
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
        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterPM([FromBody] NewUserDTO newUser)
        {
            var hashedPasswordFromSettings = _configuration.GetValue<string>("PMSettings:RegistrationPassword");
            var hasher = new PasswordHasher<string>();
            var result = hasher.VerifyHashedPassword(null, hashedPasswordFromSettings, newUser.Password);

            if (result != PasswordVerificationResult.Success)
            {
                return BadRequest("Invalid registration password.");
            }

            return await RegisterNewUserWithRoles(newUser, new[] { "PM" });
        }


        [HttpPost("[action]")]
        [ProducesResponseType(typeof(AuthenticationModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] UserCredentialsDTO userCredentials)
        {
            var user = await _authService.ValidateUser(userCredentials);
            if (user is null)
            {
                _logger.LogWarning("{Login}: Authentication failed. Wrong username or password.", nameof(Login));
                return Unauthorized();
            }

            return Ok(await _authService.CreateAuthenticationModel(user));
        }

        [HttpPost("[action]")]
        [ProducesResponseType(typeof(AuthenticationModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<AuthenticationModel>> Refresh([FromBody] RefreshTokenModel refreshTokenCredentials)
        {
            var user = await _userManager.FindByNameAsync(refreshTokenCredentials.UserName);
            if (user is null)
            {
                _logger.LogWarning("{Refresh}: Refreshing token failed. Unknown username {UserName}.", nameof(Refresh), refreshTokenCredentials.UserName);
                return Forbid();
            }

            var refreshToken = user.RefreshTokens.FirstOrDefault(r => r.Token == refreshTokenCredentials.RefreshToken);
            if (refreshToken is null)
            {
                _logger.LogWarning("{Refresh}: Refreshing token failed. The refresh token is not found.", nameof(Refresh));
                return Unauthorized();
            }

            if (refreshToken.ExpiryTime < DateTime.UtcNow)
            {
                _logger.LogWarning("{Refresh}: Refreshing token failed. The refresh token is not valid.", nameof(Refresh));
                _logger.LogWarning("{Refresh}: ExpiryTime", nameof(refreshToken.ExpiryTime));
                return Unauthorized();
            }

            return Ok(await _authService.CreateAuthenticationModel(user));
        }

        [Authorize]
        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenModel refreshTokenCredentials)
        {
            var user = await _userManager.FindByNameAsync(refreshTokenCredentials.UserName);
            if (user is null)
            {
                _logger.LogWarning("{Logout}: Logout failed. Unknown username {UserName}.", nameof(Logout), refreshTokenCredentials.UserName);
                return Forbid();
            }

            await _authService.RemoveRefreshToken(user, refreshTokenCredentials.RefreshToken);

            return Accepted();
        }

        [HttpGet("[action]")]
        public IActionResult GeneratePasswordHash([FromQuery] string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return BadRequest("Password is required.");
            }

            var hasher = new PasswordHasher<IdentityUser>();
            var hashedPassword = hasher.HashPassword(null, password);
            return Ok(new { Password = password, HashedPassword = hashedPassword });
        }

    }

}
