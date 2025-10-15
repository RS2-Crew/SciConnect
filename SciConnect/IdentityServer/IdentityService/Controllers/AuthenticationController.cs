using AutoMapper;
using IdentityServer.Entities;
using IdentityService.Controllers.Base;
using IdentityService.DTOs;
using IdentityService.Services;
using MassTransit;
using MassTransit.Transports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace IdentityService.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]

    public class AuthenticationController : RegistrationController
    {
        private readonly IAuthenticationService _authService;
        private readonly IConfiguration _configuration;
        private readonly IPublishEndpoint _publishEndpoint;
        public AuthenticationController(ILogger<AuthenticationController> logger, IMapper mapper, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IDistributedCache cache, IAuthenticationService authService, IConfiguration configuration, IPublishEndpoint publishEndpoint)
       : base(logger, mapper, userManager, roleManager, cache)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }
        [Authorize(Roles = "PM")]
        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GenerateVerificationCode([FromBody] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new { error = "Email is required." });
            }

            var verificationCode = Guid.NewGuid().ToString().Substring(0, 6); // Generisanje koda
            await StoreVerificationCode(email, verificationCode);

            // Simulacija slanja koda (možete povezati sa email servisom)
            _logger.LogInformation($"Verification code for {email}: {verificationCode}");

            await _publishEndpoint.Publish(new EmailMessage
            {
                To = email,
                Subject = "SciConnect Verification Code",
                Body = $"Your verification code is: <b>{verificationCode}</b><br>Please enter this code during registration."
            });


            return Ok(new { message = "Verification code generated, stored, and sent to the admin." });
        }


        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterGuest([FromBody] NewUserDTO newUser)
        {
            return await RegisterNewUserWithRoles(newUser, new string[] { "Guest" });
        }



        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RequestAdminRegistration([FromBody] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest(new { error = "Email is required." });

            try
            {
                var allUsers = await _userManager.Users.ToListAsync();
                var pmEmails = new List<string>();

                foreach (var user in allUsers)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains("PM"))
                    {
                        pmEmails.Add(user.Email);
                    }
                }

                if (!pmEmails.Any())
                {
                    _logger.LogWarning("No PMs found to send admin registration request.");
                    return NotFound(new { error = "No PM users found to notify." });
                }

                var subject = "New Admin Registration Request";
                var body = $"An Admin with email <b>{email}</b> has requested access. Please generate and send them a verification code.";

                foreach (var pmEmail in pmEmails)
                {
                    await _publishEndpoint.Publish(new EmailMessage
                    {
                        To = pmEmail,
                        Subject = subject,
                        Body = body
                    });
                }

                return Ok(new { message = "Registration request sent to PMs." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending registration request to PMs.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Internal server error occurred." });
            }
        }

        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterAdministrator([FromBody] NewUserDTO newUser)
        {

            if (!await ValidateVerificationCode(newUser.Email, newUser.VerificationCode))
            {
                return BadRequest(new { error = "Invalid or expired verification code." });
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
                return BadRequest(new { error = "Invalid registration password." });
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
    }
}
