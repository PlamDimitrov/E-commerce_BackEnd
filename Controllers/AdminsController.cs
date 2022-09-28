
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ecommerce_API.Data;
using ecommerce_API.Models;
using ecommerce_API.Helpers;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using ecommerce_API.Entities;
using System.Text.Json;
using System.Net;
using NuGet.Common;

namespace ecommerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminsController : ControllerBase
    {
        private readonly ecommerce_APIContext _context;
        private readonly JwtSettings _jwtSettings;

        public AdminsController(ecommerce_APIContext context, JwtSettings jwtSettings)
        {
            _context = context;
            _jwtSettings = jwtSettings;
        }

        // GET: api/Admins
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Admin>>> GetAllAdmins()
        {
            if (_context.Admin == null)
            {
                return NotFound();
            }
            try
            {
                return await _context.Admin.ToListAsync();
            }
            catch (Exception)
            {
                throw new Exception("Error: Not possible to get all admins!");
            }
        }

        // GET: api/Admins/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Admin>> GetAdmin(int id)
        {
            if (_context.Admin == null)
            {
                return NotFound();
            }
            string cookieValue = Request.Cookies["admin-info"];
            if (cookieValue == null)
            {
                return NotFound();
            }
            else
            {
                Admin userFromCookie = JsonSerializer.Deserialize<Admin>(cookieValue);
                var admin = await _context.Admin.FindAsync(id);
                admin.password = "****";
                return admin;
                if (admin.userName != userFromCookie.userName)
                {
                    return NotFound();
                }
                if (admin == null)
                {
                    return NotFound();
                }

            }

        }

        // PUT: api/Admins/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateAdmin(int id, Admin admin)
        {
            if (id != admin.Id)
            {
                return BadRequest();
            }

            _context.Entry(admin).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdminExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<Admin>> RegisterAdmin(Admin admin)
        {
            var adminWithHashedPassword = admin;
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(admin.password);
            adminWithHashedPassword.password = passwordHash;
            try
            {
                _context.Admin.Add(adminWithHashedPassword);
                await _context.SaveChangesAsync();
                adminWithHashedPassword.password = "****";
                return Ok(adminWithHashedPassword);

            }
            catch (Exception)
            {

                throw new Exception("Error: Admin not created!");
            }

        }
        [HttpPost]
        [Route("auth")]
        [Authorize]

        public async Task<ActionResult<Admin>> AuthorizeAdmin(Admin admin)
        {
            var id = admin.Id;
            var userName = admin.userName;
            var adminFromDataBase = await _context.Admin
                    .Where(u => u.Id == id)
                    .FirstOrDefaultAsync();
            if (adminFromDataBase != null && adminFromDataBase.userName == userName)
            {
                adminFromDataBase.password = "****";
                return Ok(adminFromDataBase);
            }
            else
            {
                return Unauthorized("You have no authorization!");
            }
        }


        // POST: api/Admins
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("login")]

        public async Task<ActionResult<Admin>> LoginAdmin(Admin userLogin)
        {
            bool verified = false;
            try
            {
                Admin? adminFromDataBase = await _context.Admin
                    .Where(u => u.userName == userLogin.userName)
                    .FirstOrDefaultAsync();

                if (adminFromDataBase != null)
                {
                    string passwordHash = BCrypt.Net.BCrypt.HashPassword(userLogin.password);
                    verified = BCrypt.Net.BCrypt.Verify(userLogin.password, adminFromDataBase.password);
                    adminFromDataBase.password = "*****";
                }
                if (adminFromDataBase != null && verified == true)
                {
                    UserForClientCookie userForClientCookie = new UserForClientCookie();
                    userForClientCookie.Id = adminFromDataBase.Id;
                    userForClientCookie.userName = adminFromDataBase.userName;
                    userForClientCookie.password = adminFromDataBase.password;
                    userForClientCookie.email = adminFromDataBase.email;

                    UserTokens token = JwtHelpers.JwtHelpers.SetAdminToken(_jwtSettings, adminFromDataBase);
                    CookieHelper.CreateTokenCookie(Response, token);
                    CookieHelper.CreateAdminCookie(Response, userForClientCookie);
                    return Ok(adminFromDataBase);
                }
                else
                {
                    return BadRequest(new ResponseError { ErrorCode = 400, ErrorMessage = "Wrong username or password!" });
                    throw new Exception("Error: Wrong username or password!");
                }


            }
            catch (Exception)
            {
                throw new Exception("Error: User was not found in the database!");
            }
        }

        [HttpPost]
        [Route("logout")]
        public async Task<ActionResult<Admin>> LogoutAdmin()
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
            if (_context.Admin == null)
            {
                return NotFound();
            }
            var admin = await _context.Admin.FindAsync(id);
            if (admin == null)
            {
                return NotFound();
            }

            _context.Admin.Remove(admin);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost]
        [Route("uploadProfilePicture/{id}")]
        [Authorize]
        public async Task<ActionResult<byte[]>> UploadProfilePicture()
        {
            IFormFile file = Request.Form.Files[0];
            string? cookieValue = Request.Cookies["admin-info"];
            if (cookieValue != null)
            {
                Admin? adminFromCookie = JsonSerializer.Deserialize<Admin>(cookieValue);
                try
                {
                    Admin? adminFromDataBase = await _context.Admin
                            .Where(u => u.Id == adminFromCookie.Id)
                            .FirstOrDefaultAsync();
                    if (adminFromDataBase == null)
                    {
                        return BadRequest();
                    }
                    else
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            byte[] fileBytes = ms.ToArray();

                            adminFromDataBase.image = fileBytes;
                            await _context.SaveChangesAsync();
                            Admin? adminFromDataBaseResponse = await _context.Admin
                                    .Where(u => u.Id == adminFromCookie.Id)
                                    .FirstOrDefaultAsync();
                            return Ok(adminFromDataBaseResponse);
                        };
                    }
                }
                catch (Exception)
                {
                    throw new Exception("Error: User not found!");
                }
            }
            else
            {
                throw new Exception("Error: No cookie with Admin info was found!");
            }

        }
        private bool AdminExists(int id)
        {
            return (_context.Admin?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
