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

                return CreatedAtAction("GetUser", new { id = user.Id }, user);

            }
            catch (Exception)
            {

                throw new Exception("Error: User not created!");
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
                    .Where(u =>  u.userName == userLogin.userName )
                    .ToListAsync();
                if (userFromDataBase.Count != 0)
                {
                    verified = BCrypt.Net.BCrypt.Verify(userLogin.password, userFromDataBase.FirstOrDefault().password);
                }
                if (userFromDataBase != null && verified == true)
                {
                   var token = JwtHelpers.JwtHelpers.SetToken(_jwtSettings, userFromDataBase.FirstOrDefault());
                   CookieHelper.CreateTokenCookie(Response, token);
                    userFromDataBase.FirstOrDefault().password = null;
                   return Ok(userFromDataBase);
                }
                else
                {
                    throw new Exception("Error: Wrong username or password!");
                }
                

            }
            catch (Exception)
            {
                throw new Exception("Error: Wrong username or password!");
            }

        }

        [HttpGet]
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
            Response.Cookies.Delete("ecom-auth-token");
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

        [HttpGet]
        [Route("auth")]
        public async Task<ActionResult<User>> Authenticate()
        {
            string token = Request.Cookies["ecom-auth-token"];
            var validation = JwtHelpers.JwtHelpers.ValidateJwtToken(token, _jwtSettings);
            var isTokenObsolete = await JwtHelpers.JwtHelpers.CheckObsoleteToken(token, _context);
            if (validation != null && !isTokenObsolete)
            {
                var userId = validation;
                try
                {
                    var user = await _context.User.FindAsync(userId);
                    user.password = null;
                    return user;
                }
                catch (Exception)
                {

                    throw new Exception("Error: User not found!"); ;
                }
                
            } else
            {
                throw new Exception("Error: User not authenticated!");
            }

        }
        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}
