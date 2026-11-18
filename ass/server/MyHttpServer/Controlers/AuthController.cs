using CustomHttpServer.Framework.Attributes;
using MyHttpServer.frameworks;
using MyHttpServer.frameworks.Аttributes;
using System;
using System.IO;
using System.Net;

namespace CustomHttpServer.Controllers
{
    // Простой контроллер для входа и выхода
    [HttpController("auth")]
    public class AuthController : ControllerBase
    {
        // Показать страничку входа
        [GET("login")]
        public IHttpResponseTypeResult login()
        {
            string path = Path.Combine(AppContext.BaseDirectory, "static", "login.html");
            string html = File.ReadAllText(path);
            return Html(html);
        }

        // Проверить, вошел ли человек (для JS на фронте)
        [GET("status")]
        public IHttpResponseTypeResult status()
        {
            string cookie = Context.Request.Cookies["sessionId"]?.Value;
            bool ok = SessionService.GetUserId(cookie).HasValue;
            // отдаем оба варианта имени, чтобы фронт точно понял
            return Json(new { IsAuthenticated = ok, isAuthenticated = ok });
        }

        [GET("me")]
        public IHttpResponseTypeResult me()
        {
            string cookie = Context.Request.Cookies["sessionId"]?.Value;
            bool ok = SessionService.GetUserId(cookie).HasValue;
            return Json(new { IsAuthenticated = ok, isAuthenticated = ok });
        }

        // Для тестов - нужны пустые странички
        [GET("json")]
        public void json() { }

        [GET("html")]
        public IHttpResponseTypeResult htmlPage()
        {
            return Html("");
        }

        // Сам вход - проверяем логин и пароль
        [POST("login")]
        public IHttpResponseTypeResult login([FormData] string login, [FormData] string password)
        {
            Console.WriteLine($"Получен login: {login}; пароль {password}.");

            // Простой тестовый логин
            string goodLogin = "student";
            string goodPassword = "test123";

            if (login != goodLogin || password != goodPassword)
            {
                // не правильный пароль
                return Json(new { Success = false, Message = "Неверный логин или пароль" });
            }

            // пароль верный - делаем новую сессию
            int userId = 1;
            string sessionId = SessionService.CreateSession(userId);

            // кладем сессию в куку
            Cookie cookie = new Cookie("sessionId", sessionId, "/");
            cookie.HttpOnly = true;
            cookie.Expires = DateTime.UtcNow.AddMinutes(30);
            Context.Response.SetCookie(cookie);

            return Json(new { Success = true, Message = "Вход выполнен" });
        }

        // Выход - удаляем сессию и чистим куку
        [POST("logout")]
        public IHttpResponseTypeResult Logout()
        {
            string cookieValue = Context.Request.Cookies["sessionId"]?.Value;
            SessionService.RemoveSession(cookieValue);

            Cookie cookie = new Cookie("sessionId", "", "/");
            cookie.Expires = DateTime.UtcNow.AddDays(-1);
            Context.Response.SetCookie(cookie);

            return Json(new { Success = true, Message = "Выход выполнен" });
        }

        // Тоже выход, но через GET (когда жмут на ссылку)
        [GET("logout")]
        public IHttpResponseTypeResult logoutGet()
        {
            string cookieValue = Context.Request.Cookies["sessionId"]?.Value;
            SessionService.RemoveSession(cookieValue);

            Cookie cookie = new Cookie("sessionId", "", "/");
            cookie.Expires = DateTime.UtcNow.AddDays(-1);
            Context.Response.SetCookie(cookie);

            // просто отправляем на главную
            Context.Response.Redirect("/");
            return Html("<html><body>Выход выполнен. <a href='/'>На главную</a></body></html>");
        }
    }
}
