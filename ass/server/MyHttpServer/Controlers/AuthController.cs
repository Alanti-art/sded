using CustomHttpServer.Framework;
using CustomHttpServer.Framework.Attributes;
using CustomHttpServer.Framework.HttpResponses;
using MyHttpServer.frameworks;
using MyHttpServer.frameworks.HttpResponses;
using MyHttpServer.frameworks.Аttributes;
using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;

namespace CustomHttpServer.Controllers
{
    [HttpController("auth")]
    public class AuthController : ControllerBase
    {
        [GET("login")]
        public IHttpResponseTypeResult login()
        {
            string html = File.ReadAllText(
                Path.Combine(AppContext.BaseDirectory, "static", "login.html"));
            return Html(html);
        }

        [GET("status")]
        public IHttpResponseTypeResult status()
        {
            string? sessionId = Context.Request.Cookies["sessionId"]?.Value;
            bool isAuthenticated = SessionService.GetUserId(sessionId).HasValue;
            return Json(new { IsAuthenticated = isAuthenticated, isAuthenticated = isAuthenticated });
        }

        [GET("me")]
        public IHttpResponseTypeResult me()
        {
            string? sessionId = Context.Request.Cookies["sessionId"]?.Value;
            bool isAuthenticated = SessionService.GetUserId(sessionId).HasValue;
            return Json(new { IsAuthenticated = isAuthenticated, isAuthenticated = isAuthenticated });
        }

        // Тестовые эндпоинты для совместимости с UnitTests
        [GET("json")]
        public void json() { }

        [GET("html")]
        public IHttpResponseTypeResult htmlPage()
        {
            return Html("");
        }

        [POST("login")]
        public IHttpResponseTypeResult login([FormData] string login, [FormData] string password)
        {
            Console.WriteLine($"Получен login: {login}; пароль {password}.");

            const string testLogin = "student";
            const string testPassword = "test123";

            if (login != testLogin || password != testPassword)
            {
                return new JsonResult(new { Success = false, Message = "Неверный логин или пароль" }, 401);
            }

            int userId = 1;

            string sessionId = SessionService.CreateSession(userId);

            var cookie = new Cookie("sessionId", sessionId, "/")
            {
                HttpOnly = true,
                Secure = Context.Request.IsSecureConnection,
                Expires = DateTime.UtcNow.AddMinutes(30)
            };

            Context.Response.SetCookie(cookie);

            return Json(new
            {
                Success = true,
                Message = "Вход выполнен"
            });
        }

        
        //[POST("login")]
        //public IHttpResponseTypeResult Login(string login, string password)
        //{
        //    const string testLogin = "student";
        //    const string testPassword = "test123";

        //    if (login != testLogin || password != testPassword)
        //    {
        //        return new JsonResult( new{Success = false, Message = "Неверный логин или пароль"}, 401);
        //    }

        //    int userId = 1;

        //    string sessionId = SessionService.CreateSession(userId);

        //    var cookie = new Cookie( "sessionId",sessionId,"/")
        //    {
        //        HttpOnly = true,
        //        Secure = Context.Request.IsSecureConnection,
        //        Expires = DateTime.UtcNow.AddMinutes(30)
        //    };

        //    Context.Response.SetCookie(cookie);

        //    return Json(new
        //    {
        //        Success = true,
        //        Message = "Вход выполнен"
        //    });
        //}
        [POST("logout")]
        public IHttpResponseTypeResult Logout()
        {
            string? sessionId = Context.Request.Cookies["sessionId"]?.Value;

            SessionService.RemoveSession(sessionId);

            var cookie = new Cookie("sessionId", string.Empty, "/")
            {
                HttpOnly = true,
                Secure = Context.Request.IsSecureConnection,
                Expires = DateTime.UtcNow.AddDays(-1)
            };

            Context.Response.SetCookie(cookie);

            return Json(new
            {
                Success = true,
                Message = "Выход выполнен"
            });
        }

        [GET("logout")]
        public IHttpResponseTypeResult logoutGet()
        {
            string? sessionId = Context.Request.Cookies["sessionId"]?.Value;

            SessionService.RemoveSession(sessionId);

            var cookie = new Cookie("sessionId", string.Empty, "/")
            {
                HttpOnly = true,
                Secure = Context.Request.IsSecureConnection,
                Expires = DateTime.UtcNow.AddDays(-1)
            };

            Context.Response.SetCookie(cookie);

            // Для прямых переходов по ссылке делаем редирект на главную
            string accept = Context.Request.Headers["Accept"] ?? "";
            if (accept.Contains("text/html"))
            {
                Context.Response.Redirect("/");
                return Html("<html><head><meta http-equiv='refresh' content='0; url=/'></head><body>Выход выполнен. <a href='/'>На главную</a></body></html>");
            }

            return Json(new
            {
                Success = true,
                Message = "Выход выполнен"
            });
        }


    }
}