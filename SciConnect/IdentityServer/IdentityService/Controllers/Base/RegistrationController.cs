using AutoMapper;
using IdentityServer.Entities;
using IdentityService.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;


namespace IdentityService.Controllers.Base
{
    public class RegistrationController : ControllerBase
    {
        protected readonly ILogger<AuthenticationController> _logger;
        protected readonly IMapper _mapper;
        protected readonly UserManager<User> _userManager;
        protected readonly RoleManager<IdentityRole> _roleManager;
        protected readonly IDistributedCache _cache;

        public RegistrationController(ILogger<AuthenticationController> logger, IMapper mapper, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IDistributedCache cache)
        {
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
            _cache = cache;
        }

        protected async Task<IActionResult> RegisterNewUserWithRoles(NewUserDTO newUser, IEnumerable<string> roles)
        {
            var user = _mapper.Map<User>(newUser);

            var result = await _userManager.CreateAsync(user, newUser.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }

                return BadRequest(ModelState);
            }

            _logger.LogInformation($"Successfully registered user: {user.UserName}.");

            foreach (var role in roles)
            {
                var roleExists = await _roleManager.RoleExistsAsync(role);
                if (roleExists)
                {
                    await _userManager.AddToRoleAsync(user, role);
                    _logger.LogInformation($"Added a role {role} to user: {user.UserName}.");
                }
                else
                {
                    _logger.LogInformation($"Role {role} does not exist.");
                }
            }

            return StatusCode(StatusCodes.Status201Created);
        }
        protected async Task StoreVerificationCode(string email, string verificationCode)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
            };
            await _cache.SetStringAsync(email.Trim().ToLowerInvariant(), verificationCode, options);
        }

        protected async Task<bool> ValidateVerificationCode(string email, string verificationCode)
        {
            var storedCode = await _cache.GetStringAsync(email.Trim().ToLowerInvariant());
            return storedCode != null && storedCode == verificationCode;
        }
    }




}
