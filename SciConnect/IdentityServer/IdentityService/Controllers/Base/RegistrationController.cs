using AutoMapper;
using IdentityServer.Entities;
using IdentityService.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;


namespace IdentityService.Controllers.Base
{
    public class RegistrationController : ControllerBase
    {
        protected readonly ILogger<AuthenticationController> _logger;
        protected readonly IMapper _mapper;
        protected readonly UserManager<User> _userManager;
        protected readonly RoleManager<IdentityRole> _roleManager;
        protected readonly IMemoryCache _memoryCache;

        public RegistrationController(ILogger<AuthenticationController> logger, IMapper mapper, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IMemoryCache memoryCache)
        {
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
            _memoryCache = memoryCache;

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
        protected void StoreVerificationCode(string email, string verificationCode)
        {
            _memoryCache.Set(email, verificationCode, TimeSpan.FromMinutes(60));
        }

        protected bool ValidateVerificationCode(string email, string verificationCode)
        {
            if (_memoryCache.TryGetValue(email, out string storedCode))
            {
                return storedCode == verificationCode;
            }
            return false;
        }
    }




}
