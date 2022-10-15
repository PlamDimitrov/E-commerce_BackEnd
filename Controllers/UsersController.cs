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
using ecommerce_API.Dto;

namespace ecommerce_API.Controllers

{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IUserRepository<User> _userRepository;

        public UsersController(JwtSettings jwtSettings, IUserRepository<User> userRepository)
        {
            _jwtSettings = jwtSettings;
            _userRepository = userRepository;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<IUser>>> GetAll()
        {
            try
            {
                var users = await _userRepository.GetAll();
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                return Ok(users);
            }
            catch (Exception)
            {
                throw new Exception("Error: Not possible to get all users!");
            }
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<IUser>> GetUser(int id)
        {
            var user = await _userRepository.GetOne(id);
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
        public async Task<IActionResult> PutUser(User user)
        {
            User updatedUser = await _userRepository.Update(user);
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
            if (_userRepository.CheckIfExists(user.UserName))
            {
                return BadRequest(new ResponseError { ErrorCode = 400, ErrorMessage = "Username already in use!" });
                throw new Exception("Error: Username already in use!");
            }
            else if (_userRepository.CheckIfExists(user.Email) && user.Email != "")
            {
                return BadRequest(new ResponseError { ErrorCode = 400, ErrorMessage = "Email already in use!" });
                throw new Exception("Error: Email already in use!");
            }
            else
            {
                try
                {
                    UserDto userDto = await _userRepository.Create(user);
                    if (userDto != null)
                    {
                        return Ok(userDto);
                    }
                    else
                    {
                        return BadRequest("User was not created!");
                    }
                }
                catch (Exception)
                {
                    return BadRequest("Connection error!");
                }
            }
        }

        [HttpPost]
        [Route("auth")]
        [Authorize]

        public async Task<ActionResult<IUser>> AuthorizeUser(UserDto userAuth)
        {
            UserDto user = await _userRepository.GetDto(userAuth.Id);
            if (user != null)
            {
                return Ok(user);
            }
            else
            {
                return Unauthorized("User don't exist!");
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<IUser>> LogInUser(User userLogin)
        {
            try
            {
                UserDto user = await _userRepository.LogIn(userLogin);
                if (user != null)
                {
                    var token = JwtHelpers.JwtHelpers.SetUserToken(_jwtSettings, user);
                    CookieHelper.CreateTokenCookie(Response, token);
                    CookieHelper.CreateUserCookie(Response, user);
                    return Ok(user);
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
            //  string tokenValue = Request.Cookies["ecom-auth-token"];
            //   var handler = new JwtSecurityTokenHandler();
            //  var tokenValidTo = handler.ReadJwtToken(tokenValue).ValidTo;
            // var expiredToken = new ExpiredToken();
            // expiredToken.ExpiredTokenValue = tokenValue;
            //  expiredToken.ExpiredTime = tokenValidTo;

            try
            {
                //  TokensRepository to be implemented
                //  _context.ExpiredTokens.Add(expiredToken);
                //  await _context.SaveChangesAsync();
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
            try
            {
                bool userIsDeleted = await _userRepository.Delete(id);
                if (userIsDeleted)
                {
                    return Ok();
                }
                else
                {
                    return NotFound();
                }
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
                UserDto user = await _userRepository.RemoveImage(id);
                if (user == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(user);
                }
            }
            else
            {
                IFormFile file = Request.Form.Files[0];
                UserDto user = await _userRepository.AddImage(id, file);
                if (user == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(user);
                }
            }
        }
    }
}
