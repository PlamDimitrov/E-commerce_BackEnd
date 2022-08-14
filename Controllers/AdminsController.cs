﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ecommerce_API;
using ecommerce_API.Data;
using ecommerce_API.Models;
using ecommerce_API.Helpers;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using ecommerce_API.Entities;
using System.Text.Json;

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
        public async Task<ActionResult<Admin>> GetAdmin(int id)
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

            return admin;
        }

        // PUT: api/Admins/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
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
                adminWithHashedPassword.password = null;
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
                return Ok(admin);
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
        public async Task<ActionResult<Admin>> LoginAdmin(Admin admin)
        {
            bool verified = false;
            try
            {
                var adminFromDataBase = await _context.Admin
                    .Where(u => u.userName == admin.userName)
                    .FirstOrDefaultAsync();
                if (adminFromDataBase != null)
                {
                    string passwordHash = BCrypt.Net.BCrypt.HashPassword(admin.password);
                    verified = BCrypt.Net.BCrypt.Verify(admin.password, adminFromDataBase.password);
                    adminFromDataBase.password = "*******";
                }
                if (adminFromDataBase != null && verified == true)
                {
                    var token = JwtHelpers.JwtHelpers.SetAdminToken(_jwtSettings, adminFromDataBase);
                    CookieHelper.CreateTokenCookie(Response, token);
                    CookieHelper.CreateAdminCookie(Response, adminFromDataBase);

                    return Ok(adminFromDataBase);
                }
                else
                {
                    return Unauthorized();
                    throw new Exception("Error: Wrong username or password!");
                }


            }
            catch (Exception)
            {
                throw new Exception("Error: Somethin went wrong with the login!");
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
        [Route("uploadProfilePicture")]
        [Authorize]
        public async Task<ActionResult<byte[]>> UploadProfilePicture()
        {
            IFormFile file = Request.Form.Files[0];
            string? cookieValue = Request.Cookies["admin-info"];
            Admin adminFromCookie = JsonSerializer.Deserialize<Admin>(cookieValue);
            try
            {
                var adminFromDataBase = await _context.Admin
                        .Where(u => u.Id == adminFromCookie.Id)
                        .FirstOrDefaultAsync();
                if (adminFromDataBase == null)
                {
                    return BadRequest();
                }
                else
                {
                    using (var ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        string s = Convert.ToBase64String(fileBytes);

                        adminFromDataBase.image = fileBytes;
                        await _context.SaveChangesAsync();
                        var adminFromDataBaseResponse = await _context.Admin
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
        private bool AdminExists(int id)
        {
            return (_context.Admin?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
