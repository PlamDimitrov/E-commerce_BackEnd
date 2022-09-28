using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ecommerce_API.Models;
using ecommerce_API.Data;

namespace ecommerce_API.JwtHelpers
{

    public static class JwtHelpers
    {
        public static IEnumerable<Claim> GetClaims(this UserTokens userAccounts, Guid Id)
        {
            IEnumerable<Claim> claims = new Claim[] {
                new Claim("Id", userAccounts.Id.ToString()),
                    new Claim(ClaimTypes.Name, userAccounts.UserName),
                    new Claim(ClaimTypes.NameIdentifier, Id.ToString()),
            };
            return claims;
        }
        public static IEnumerable<Claim> GetClaims(this UserTokens userAccounts, out Guid Id)
        {
            Id = Guid.NewGuid();
            return GetClaims(userAccounts, Id);
        }
        public static UserTokens GenTokenkey(UserTokens model, JwtSettings jwtSettings)
        {
            try
            {
                var UserToken = new UserTokens();
                if (model == null) throw new ArgumentException(nameof(model));
                var key = System.Text.Encoding.ASCII.GetBytes(jwtSettings.IssuerSigningKey);
                Guid Id = Guid.Empty;
                DateTime expireTime = DateTime.Now.AddDays(1);
                UserToken.Validaty = expireTime.TimeOfDay;
                var JWToken = new JwtSecurityToken(
                    issuer: jwtSettings.ValidIssuer,
                    audience: jwtSettings.ValidAudience,
                    claims: GetClaims(model, out Id),
                    notBefore: new DateTimeOffset(DateTime.Now).DateTime,
                    expires: new DateTimeOffset(expireTime).DateTime,
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256));
                UserToken.Token = new JwtSecurityTokenHandler().WriteToken(JWToken);
                UserToken.UserName = model.UserName;
                UserToken.Id = model.Id;
                UserToken.GuidId = Id;
                return UserToken;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static UserTokens SetUserToken(JwtSettings jwtSettings, User userFromDataBase)
        {
            var userData = userFromDataBase;
            var token = GenTokenkey(new UserTokens()
            {
                GuidId = Guid.NewGuid(),
                UserName = userData.userName,
                Id = userFromDataBase.Id,
            }, jwtSettings);

            return token;
        }
        public static UserTokens SetAdminToken(JwtSettings jwtSettings, Admin adminFromDataBase)
        {
            var adminData = adminFromDataBase;
            var token = GenTokenkey(new UserTokens()
            {
                GuidId = Guid.NewGuid(),
                UserName = adminData.userName,
                Id = adminFromDataBase.Id,
            }, jwtSettings);

            return token;
        }
        public static int? ValidateJwtToken(string token, JwtSettings jwtSettings)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = System.Text.Encoding.ASCII.GetBytes(jwtSettings.IssuerSigningKey);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var accountId = int.Parse(jwtToken.Claims.First(x => x.Type == "Id").Value);

                return accountId;
            }
            catch
            {
                return null;
            }
        }

        public async static Task<bool> CheckObsoleteToken(string token, ecommerce_APIContext _context)
        {
            try
            {
                var obsoleteToken =  _context.ExpiredTokens
                    .Where(x => x.ExpiredTokenValue == token)
                    .FirstOrDefault(); ;
                    
                if (obsoleteToken != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw new Exception("Error: Can't connect to obsolete token database.");
            }

        }
        public static void CleanJwtObsoleteTokenFromDatabase()
        {
            Console.WriteLine("test!!!"); // Logic to be implemented - clean obsolite JWT tokens from database 
        }
    }
}