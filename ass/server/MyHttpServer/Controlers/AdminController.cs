using ConsoleApp;
using CustomHttpServer.Framework.Attributes;
using MyHttpServer.frameworks;
using MyHttpServer.frameworks.Аttributes;
using System;
using System.IO;

namespace MyHttpServer.Controllers
{
    [HttpController("admin")]
    public class AdminController : ControllerBase
    {
        [GET("cards")]
        [Authorize]
        public IHttpResponseTypeResult Cards()
        {
            string templatePath = Path.Combine(AppContext.BaseDirectory, "static", "cards.html");
            // If template missing fallback to static
            if (!File.Exists(templatePath))
                return Html("<h1>Управление карточками</h1>");
            string template = File.ReadAllText(templatePath);
            var renderer = new HtmlTemplateRenderer();
            // Dummy data for rendering
            var cards = new[]
            {
                new { Id = 1, Name = "Герцог", Image = "https://via.placeholder.com/62x88?text=H", Description = "Берёт 3 монеты" },
                new { Id = 2, Name = "Ассасин", Image = "https://via.placeholder.com/62x88?text=A", Description = "Убивает за 3 монеты" },
            };
            string html = renderer.RenderFromString(template, new { Cards = cards, CardsCount = cards.Length });
            return Html(html);
        }

        [GET("cards/new")]
        [Authorize]
        public IHttpResponseTypeResult NewCard()
        {
            string path = Path.Combine(AppContext.BaseDirectory, "static", "add-card.html");
            string html = File.ReadAllText(path);
            return Html(html);
        }

        [GET("cards/edit")]
        [Authorize]
        public IHttpResponseTypeResult EditCard(int id = 1)
        {
            string path = Path.Combine(AppContext.BaseDirectory, "static", "edit-card.html");
            string template = File.ReadAllText(path);
            var renderer = new HtmlTemplateRenderer();
            var model = new
            {
                Id = id,
                Name = "Герцог",
                Image = "https://via.placeholder.com/300x290?text=Hero",
                Description = "Описание",
                Action = "Действие",
                Cost = "Нет",
                Blocks = "Ничего",
                BlockedBy = "Никем",
                Note = ""
            };
            string html = renderer.RenderFromString(template, model);
            return Html(html);
        }

        [POST("cards/create")]
        [Authorize]
        public IHttpResponseTypeResult Create([FormData] string Name, [FormData] string Image, [FormData] string Description,
            [FormData] string Action, [FormData] string Cost, [FormData] string Blocks, [FormData] string BlockedBy, [FormData] string Note)
        {
            // В реальном проекте здесь сохранение в БД, сейчас просто редирект
            Context.Response.Redirect("/admin/cards");
            return Html("<html><head><meta http-equiv='refresh' content='0; url=/admin/cards'></head><body>Карточка создана. <a href='/admin/cards'>Назад</a></body></html>");
        }

        [POST("cards/update")]
        [Authorize]
        public IHttpResponseTypeResult Update([FormData] int Id, [FormData] string Name, [FormData] string Image, [FormData] string Description,
            [FormData] string Action, [FormData] string Cost, [FormData] string Blocks, [FormData] string BlockedBy, [FormData] string Note)
        {
            Context.Response.Redirect("/admin/cards");
            return Html("<html><head><meta http-equiv='refresh' content='0; url=/admin/cards'></head><body>Карточка обновлена. <a href='/admin/cards'>Назад</a></body></html>");
        }

        [POST("cards/delete")]
        [Authorize]
        public IHttpResponseTypeResult Delete([FormData] int Id)
        {
            Context.Response.Redirect("/admin/cards");
            return Html("<html><head><meta http-equiv='refresh' content='0; url=/admin/cards'></head><body>Карточка удалена. <a href='/admin/cards'>Назад</a></body></html>");
        }

        [GET("login")]
        public IHttpResponseTypeResult LoginPage()
        {
            string path = Path.Combine(AppContext.BaseDirectory, "static", "login.html");
            string html = File.ReadAllText(path);
            return Html(html);
        }

        [POST("login")]
        public IHttpResponseTypeResult LoginPost([FormData] string login, [FormData] string password)
        {
            Console.WriteLine($"Получен login: {login}; пароль {password}.");
            const string testLogin = "student";
            const string testPassword = "test123";
            if (login != testLogin || password != testPassword)
            {
                return new MyHttpServer.frameworks.HttpResponses.JsonResult(new { Success = false, Message = "Неверный логин или пароль" }, 401);
            }
            int userId = 1;
            string sessionId = SessionService.CreateSession(userId);
            var cookie = new System.Net.Cookie("sessionId", sessionId, "/")
            {
                HttpOnly = true,
                Secure = Context.Request.IsSecureConnection,
                Expires = DateTime.UtcNow.AddMinutes(30)
            };
            Context.Response.SetCookie(cookie);
            return Json(new { Success = true, Message = "Вход выполнен" });
        }

        [GET("logout")]
        public IHttpResponseTypeResult LogoutGet()
        {
            string sessionId = Context.Request.Cookies["sessionId"]?.Value;
            SessionService.RemoveSession(sessionId);
            var cookie = new System.Net.Cookie("sessionId", string.Empty, "/")
            {
                HttpOnly = true,
                Secure = Context.Request.IsSecureConnection,
                Expires = DateTime.UtcNow.AddDays(-1)
            };
            Context.Response.SetCookie(cookie);
            Context.Response.Redirect("/");
            return Html("<html><head><meta http-equiv='refresh' content='0; url=/'></head><body>Выход выполнен</body></html>");
        }

        [POST("logout")]
        public IHttpResponseTypeResult LogoutPost()
        {
            string sessionId = Context.Request.Cookies["sessionId"]?.Value;
            SessionService.RemoveSession(sessionId);
            var cookie = new System.Net.Cookie("sessionId", string.Empty, "/")
            {
                HttpOnly = true,
                Secure = Context.Request.IsSecureConnection,
                Expires = DateTime.UtcNow.AddDays(-1)
            };
            Context.Response.SetCookie(cookie);
            return Json(new { Success = true, Message = "Выход выполнен" });
        }
    }
}
