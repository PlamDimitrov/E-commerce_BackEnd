
using Microsoft.AspNetCore.Mvc;
using ecommerce_API.Models;
using ecommerce_API.Helpers;
using Microsoft.AspNetCore.Authorization;
using ecommerce_API.Interfaces;
using ecommerce_API.Dto;

namespace ecommerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminsController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IUserRepository<Admin> _adminRepository;

        public AdminsController(JwtSettings jwtSettings, IUserRepository<Admin> adminRepository)
        {
            _jwtSettings = jwtSettings;
            _adminRepository = adminRepository; 
        }

        // GET: api/Admins
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<IUser>>> GetAll()
        {
            try
            {
                var admins = await _adminRepository.GetAll();
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                return Ok(admins);
            }
            catch (Exception)
            {
                throw new Exception("Error: Not possible to get all users!");
            }
        }

        // GET: api/Admins/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<IUser>> GetAdmin(int id)
        {
            var admin = await _adminRepository.GetOne(id);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (admin == null)
            {
                return NotFound();
            }

            return Ok(admin);

        }

        // PUT: api/Admins/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateAdmin(Admin admin)
        {
            Admin updatedAdmin = await _adminRepository.Update(admin);
            if (updatedAdmin == null)
            {
                return NotFound();
            }
            return Ok(updatedAdmin);
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<IUser>> RegisterAdmin(Admin admin)
        {
            if (_adminRepository.CheckIfExists(admin.UserName))
            {
                return BadRequest(new ResponseError { ErrorCode = 400, ErrorMessage = "Username already in use!" });
                throw new Exception("Error: Username already in use!");
            }
            else if (_adminRepository.CheckIfExists(admin.Email) && admin.Email != "")
            {
                return BadRequest(new ResponseError { ErrorCode = 400, ErrorMessage = "Email already in use!" });
                throw new Exception("Error: Email already in use!");
            }
            else
            {
                try
                {
                    UserDto adminDto = await _adminRepository.Create(admin);
                    if (adminDto != null)
                    {
                        return Ok(adminDto);
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

        public async Task<ActionResult<IUser>> AuthorizeAdmin(UserDto adminAuth)
        {
            UserDto user = await _adminRepository.GetDto(adminAuth.Id);
            if (user != null)
            {
                return Ok(user);
            }
            else
            {
                return Unauthorized("Admin don't exist!");
            }
        }


        // POST: api/Admins
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("login")]

        public async Task<ActionResult> LoginAdmin(Admin adminLogin)
        {
            try
            {
                UserDto admin = await _adminRepository.LogIn(adminLogin);
                if (admin != null)
                {
                    var token = JwtHelpers.JwtHelpers.SetUserToken(_jwtSettings, admin);
                    CookieHelper.CreateTokenCookie(Response, token);
                    CookieHelper.CreateAdminCookie(Response, admin);
                    return Ok(admin);
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
        public async Task<ActionResult> LogoutAdmin()
        {
          //  var tokenValue = Request.Cookies["ecom-auth-token"];
          //  var handler = new JwtSecurityTokenHandler();
          //  var tokenValidTo = handler.ReadJwtToken(tokenValue).ValidTo;
          //  var expiredToken = new ExpiredToken();
          //  expiredToken.ExpiredTokenValue = tokenValue;
          //  expiredToken.ExpiredTime = tokenValidTo;
            try
            {
             //   _context.ExpiredTokens.Add(expiredToken);
             //   await _context.SaveChangesAsync();
                CookieHelper.RemoveTokenCookie(Response);
                CookieHelper.RemoveAdminCookie(Response);
                return Ok();

            }
            catch (Exception)
            {
                throw new Exception("Error: Token not send to database!");
            }
        }

        // DELETE: api/Admins/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            try
            {
                bool adminIsDeleted = await _adminRepository.Delete(id);
                if (adminIsDeleted)
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
                UserDto admin = await _adminRepository.RemoveImage(id);
                if (admin == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(admin);
                }
            }
            else
            {
                IFormFile file = Request.Form.Files[0];
                UserDto admin = await _adminRepository.AddImage(id, file);
                if (admin == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(admin);
                }
            }
        }
    }
}
