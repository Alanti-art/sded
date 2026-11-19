using ConsoleApp;
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
        private static readonly IHtmlTemplateRenderer Renderer = new HtmlTemplateRenderer();

        [GET("login")]
        public IHttpResponseTypeResult login()
        {

            return RenderLoginPage();
        }

        [POST("login")]
        public IHttpResponseTypeResult login([FormData] string login, [FormData] string password)
        {
            Console.WriteLine($"Получен login: {login}; пароль {password}.");

            const string testLogin = "admin";
            const string testPassword = "admin123";

            if (login != testLogin || password != testPassword)
            {
                return RenderLoginPage(
                    hasError: true,
                    errorMessage: "Неверный логин или пароль",
                    login: login ?? string.Empty);
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


            string html = File.ReadAllText(
                Path.Combine(AppContext.BaseDirectory, "static", "cards.html"));

            return Html(html);
        }

        
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

        private IHttpResponseTypeResult RenderLoginPage(
           bool hasError = false,
           string errorMessage = "",
           string login = "")
        {
            string html = Renderer.RenderFromFile(
                Path.Combine(AppContext.BaseDirectory, "static", "login.html"),
                new 
                {
                    HasError = hasError,
                    ErrorMessage = WebUtility.HtmlEncode(errorMessage ?? string.Empty),
                    Login = WebUtility.HtmlEncode(login ?? string.Empty)
                });

            return Html(html);
        }
      


    }
}
