using AuthenticationPlugin;
using CinemaAPI.Data;
using CinemaAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CinemaAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly CinemaDbContext cinemaDbContext;
        private readonly AuthService _auth;
        public UserController(CinemaDbContext _cinemaDbContext,IConfiguration configuration)
        {
            cinemaDbContext = _cinemaDbContext;
            _auth = new AuthService(configuration);
        }
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            var exitUser = cinemaDbContext.Users.SingleOrDefault(x => x.Email == user.Email);
            if (!(exitUser is null))
            {
                return BadRequest("User already exst with the same email.");
            }
            else
            {
                user.Role = "Users";
                user.Password = SecurePasswordHasherHelper.Hash(user.Password);
                await cinemaDbContext.Users.AddAsync(user);
                await cinemaDbContext.SaveChangesAsync();
            }
            return StatusCode(StatusCodes.Status201Created);

        }
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            var exitUser = await cinemaDbContext.Users.SingleOrDefaultAsync(x => x.Email == user.Email);
            if (exitUser is null)
            {
                return NotFound();
            }
            if (!SecurePasswordHasherHelper.Verify(user.Password, exitUser.Password))
            {
                return Unauthorized();
            }
            var claims = new[]
                            {
                             new Claim(JwtRegisteredClaimNames.Email, user.Email),
                             new Claim(ClaimTypes.Email, user.Email),
                             new Claim(ClaimTypes.Role, user.Role)
                           };
            var token = _auth.GenerateAccessToken(claims);
            return StatusCode(StatusCodes.Status200OK,token);


        }
    }
}
