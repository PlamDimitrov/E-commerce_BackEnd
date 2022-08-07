using ecommerce_API.Models;
using System.Text.Json;

namespace ecommerce_API.Helpers
{
    public class CookieHelper
    {
        public static void CreateTokenCookie(HttpResponse response, UserTokens token)
        {
            response.Cookies.Append("ecom-auth-token", token.Token, new CookieOptions()
            {
                Expires = DateTimeOffset.Now.AddHours(24),
                Path = "/",
                SameSite = SameSiteMode.None,
                HttpOnly = true,
                Secure = true,
            });
        }
        public static void RemoveTokenCookie(HttpResponse response)
        {
            response.Cookies.Delete("ecom-auth-token", new CookieOptions()
            {
                Expires = DateTimeOffset.Now.AddDays(-1),
                Path = "/",
                SameSite = SameSiteMode.None,
                HttpOnly = true,
                Secure = true,
            });
        }
                public static void CreateAdminCookie(HttpResponse response, Admin admin)
        {
            response.Cookies.Append("admin-info", JsonSerializer.Serialize(admin), new CookieOptions()
            {
                Expires = DateTimeOffset.Now.AddHours(24),
                Path = "/",
                SameSite = SameSiteMode.None,
                HttpOnly = false,
                Secure = true,
            });
        }
        public static void CreateUserCookie(HttpResponse response, User user)
        {
            response.Cookies.Append("user-info", JsonSerializer.Serialize(user), new CookieOptions()
            {
                Expires = DateTimeOffset.Now.AddHours(24),
                Path = "/",
                SameSite = SameSiteMode.None,
                HttpOnly = false,
                Secure = true,
            });
        }
    }
}
