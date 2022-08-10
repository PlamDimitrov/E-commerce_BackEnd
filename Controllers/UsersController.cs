#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ecommerce_API.Data;
using ecommerce_API.Models;
using ecommerce_API.Entities;
using ecommerce_API.Helpers;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;

namespace ecommerce_API.Controllers

{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ecommerce_APIContext _context;
        private readonly JwtSettings _jwtSettings;

        public UsersController(ecommerce_APIContext context, JwtSettings jwtSettings)
        {
            _context = context;
            _jwtSettings = jwtSettings;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            try
            {
                return await _context.User.ToListAsync();
            }
            catch (Exception)
            {
                throw new Exception("Error: Not possible to get all users!");
            }
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            try
            {
                var user = await _context.User.FindAsync(id);

                if (user == null)
                {
                    return NotFound();
                }
                return user;
            }
            catch (Exception)
            {
                throw new Exception("Error: Not possible to get the user!");
            }
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw new Exception("Error: User not eddited!");
                }
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            var userWithHashedPassword = user;
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(user.password);
            userWithHashedPassword.password = passwordHash;
            try
            {
                _context.User.Add(userWithHashedPassword);
                await _context.SaveChangesAsync();
                userWithHashedPassword.password = null;
                return Ok(userWithHashedPassword);

            }
            catch (Exception)
            {

                throw new Exception("Error: User not created!");
            }

        }

        [HttpPost]
        [Route("auth")]
        [Authorize]

        public async Task<ActionResult<User>> AuthorizeUser(User user)
        {
            var id = user.Id;
            var userName = user.userName;
            var userFromDataBase = await _context.User
                    .Where(u => u.Id == id)
                    .FirstOrDefaultAsync();
            if (userFromDataBase != null && userFromDataBase.userName == userName)
            {
                return Ok(user);
            }
            else
            {
                return Unauthorized("You have no authorization!");
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<UserLogin>> LogInUser(UserLogin userLogin)
        {
            bool verified = false;
            try
            {
                var userFromDataBase = await _context.User
                    .Where(u => u.userName == userLogin.userName)
                    .FirstOrDefaultAsync();
                if (userFromDataBase != null)
                {
                    verified = BCrypt.Net.BCrypt.Verify(userLogin.password, userFromDataBase.password);
                    userFromDataBase.password = "*******";
                }
                if (userFromDataBase != null && verified == true)
                {
                    var token = JwtHelpers.JwtHelpers.SetUserToken(_jwtSettings, userFromDataBase);
                    CookieHelper.CreateTokenCookie(Response, token);
                    CookieHelper.CreateUserCookie(Response, userFromDataBase);
                    return Ok(userFromDataBase);
                }
                else
                {
                    return Unauthorized();
                    throw new Exception("Error: Wrong username or password!");
                }


            }
            catch (Exception)
            {
                throw new Exception("Error: Wrong username or password!");
            }

        }

        [HttpPost]
        [Route("logout")]
        public async Task<ActionResult> LogoutUser()
        {
            var tokenValue = Request.Cookies["ecom-auth-token"];
            var handler = new JwtSecurityTokenHandler();
            var tokenValidTo = handler.ReadJwtToken(tokenValue).ValidTo;
            var expiredToken = new ExpiredToken();
            expiredToken.ExpiredTokenValue = tokenValue;
            expiredToken.ExpiredTime = tokenValidTo;

            try
            {
                _context.ExpiredTokens.Add(expiredToken);
                await _context.SaveChangesAsync();
                CookieHelper.RemoveTokenCookie(Response);
                CookieHelper.RemoveUserCookie(Response);
                return Ok();

            }
            catch (Exception)
            {
                throw new Exception("Error: Token not send to database!");
            }

        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            try
            {
                _context.User.Remove(user);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception)
            {

                throw new Exception("Error: User not deleted!");
            }
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}
