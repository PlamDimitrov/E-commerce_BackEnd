#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ecommerce_API.Data;
using ecommerce_API.Models;
using ecommerce_API.Entities;
using ecommerce_API.Helpers;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using ecommerce_API.Interfaces;
using ecommerce_API.Services;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace ecommerce_API.Controllers

{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ecommerce_APIContext _context;
        private readonly JwtSettings _jwtSettings;
        private readonly ImageService _imageService;
        private readonly IUserRepository _userRepository;

        public UsersController(ecommerce_APIContext context, JwtSettings jwtSettings, IUserRepository userRepository)
        {
            _context = context;
            _jwtSettings = jwtSettings;
            _imageService = new ImageService(context);
            _userRepository = userRepository;
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetUsers()
        {
            var user = _userRepository.GetUsers();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(user);
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetUser(int id)
        {
            var user = _userRepository.GetUser(id);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutUser( User user)
        {
            User updatedUser = await _userRepository.UpdateUser(user);
            if (updatedUser == null)
            {
                return NotFound();
            }
            return Ok(updatedUser);
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("register")]

        public async Task<ActionResult<IUser>> PostUser(User user)
        {
            User userWithHashedPassword = user;
            IUser checkUserName = await _context.Users
                .Where(u => u.UserName == user.UserName)
                .FirstOrDefaultAsync();
            IUser checkEmail = await _context.Users
                .Where(u => u.Email == user.Email)
                .FirstOrDefaultAsync();
            if (checkUserName != null)
            {
                return BadRequest(new ResponseError { ErrorCode = 400, ErrorMessage = "Username already in use!" });
                throw new Exception("Error: Username already in use!");
            }
            else if (checkEmail != null)
            {
                return BadRequest(new ResponseError { ErrorCode = 400, ErrorMessage = "Email already in use!" });
                throw new Exception("Error: Email already in use!");
            }
            else
            {
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);
                userWithHashedPassword.Password = passwordHash;
                try
                {
                    _context.Users.Add(userWithHashedPassword);
                    await _context.SaveChangesAsync();
                    userWithHashedPassword.Password = null;
                    return Ok(userWithHashedPassword);

                }
                catch (SqlException sqlEx)
                {
                    Console.WriteLine("SQL Exception: " + sqlEx);
                    return BadRequest(sqlEx.Errors);
                    // throw new Exception("Error: Users not created!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: " + ex);
                    return BadRequest(ex.Data.Values);
                    // throw new Exception("Error: Users not created!");
                }
            }



        }

        [HttpPost]
        [Route("auth")]
        [Authorize]

        public async Task<ActionResult<IUser>> AuthorizeUser(User user)
        {
            int id = user.Id;
            string userName = user.UserName;
            IUser userFromDataBase = await _context.Users
                    .Where(u => u.Id == id)
                    .FirstOrDefaultAsync();
            if (userFromDataBase != null && userFromDataBase.UserName == userName)
            {
                userFromDataBase.Password = "****";
                return Ok(userFromDataBase);
            }
            else
            {
                return Unauthorized("You have no authorization!");
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<IUser>> LogInUser(User userLogin)
        {
            bool verified = false;
            try
            {
                User userFromDataBase = await _context.Users
                    .Where(u => u.UserName == userLogin.UserName)
                    .FirstOrDefaultAsync();
                if (userFromDataBase != null)
                {
                    verified = BCrypt.Net.BCrypt.Verify(userLogin.Password, userFromDataBase.Password);
                    userFromDataBase.Password = "*****";
                }
                if (userFromDataBase != null && verified == true)
                {
                    UserForClientCookie userForClientCookie = new UserForClientCookie();
                    userForClientCookie.Id = userFromDataBase.Id;
                    userForClientCookie.userName = userFromDataBase.UserName;
                    userForClientCookie.password = userFromDataBase.Password;
                    userForClientCookie.email = userFromDataBase.Email;

                    var token = JwtHelpers.JwtHelpers.SetUserToken(_jwtSettings, userFromDataBase);
                    CookieHelper.CreateTokenCookie(Response, token);
                    CookieHelper.CreateUserCookie(Response, userForClientCookie);
                    return Ok(userFromDataBase);
                }
                else
                {
                    return BadRequest(new ResponseError { ErrorCode = 400, ErrorMessage = "Wrong username or Password!" });
                    throw new Exception("Error: Wrong username or Password!");
                }


            }
            catch (Exception)
            {
                throw new Exception("Error: Wrong username or Password!");
            }

        }

        [HttpPost]
        [Route("logout")]
        public async Task<ActionResult> LogoutUser()
        {
            string tokenValue = Request.Cookies["ecom-auth-token"];
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
            User user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            try
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception)
            {

                throw new Exception("Error: Users not deleted!");
            }
        }

        [HttpPost]
        [Route("uploadProfilePicture/{id}")]
        [Authorize]
        public async Task<ActionResult<byte[]>> UploadProfilePicture(int id)
        {
            if (Request.Form.Files.Count == 0)
            {
                var updatedUser = await _imageService.RemoveFromUser(id);
                if (updatedUser == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(updatedUser);
                }
            }
            else
            {
                IFormFile file = Request.Form.Files[0];
                var updatedUser = await _imageService.AddToUser(id, file);
                if (updatedUser == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(updatedUser);
                }
            }
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
