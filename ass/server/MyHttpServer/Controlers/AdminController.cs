using ConsoleApp;
using CustomHttpServer.Framework.Attributes;
using MyHttpServer.frameworks.Аttributes;
using System.IO;

namespace MyHttpServer.Controllers
{
    // Очень простой админ-контроллер
    // Нужен только чтобы защитить странички /admin/cards
    [HttpController("admin")]
    public class AdminController : ControllerBase
    {
        // Список карточек - только для залогиненых
        [GET("cards")]
        [Authorize]
        public IHttpResponseTypeResult Cards()
        {
            string path = Path.Combine(AppContext.BaseDirectory, "static", "cards.html");
            string html = File.ReadAllText(path);

            // Делаем вид что есть 2 карточки, просто для примера
            var cards = new[]
            {
                new { Id = 1, Name = "Герцог", Image = "https://via.placeholder.com/62x88", Description = "Берёт 3 монеты" },
                new { Id = 2, Name = "Ассасин", Image = "https://via.placeholder.com/62x88", Description = "Убивает за 3 монеты" },
            };

            var render = new HtmlTemplateRenderer();
            string result = render.RenderFromString(html, new { Cards = cards, CardsCount = cards.Length });
            return Html(result);
        }

        // Форма создания - тоже только с кукой
        [GET("cards/new")]
        [Authorize]
        public IHttpResponseTypeResult NewCard()
        {
            string path = Path.Combine(AppContext.BaseDirectory, "static", "add-card.html");
            string html = File.ReadAllText(path);
            return Html(html);
        }

        // Форма редактирования
        [GET("cards/edit")]
        [Authorize]
        public IHttpResponseTypeResult EditCard(int id = 1)
        {
            string path = Path.Combine(AppContext.BaseDirectory, "static", "edit-card.html");
            string template = File.ReadAllText(path);
            var render = new HtmlTemplateRenderer();
            var data = new
            {
                Id = id,
                Name = "Герцог",
                Image = "https://via.placeholder.com/300x290",
                Description = "Описание",
                Action = "Действие",
                Cost = "Нет",
                Blocks = "Ничего",
                BlockedBy = "Никем",
                Note = ""
            };
            string html = render.RenderFromString(template, data);
            return Html(html);
        }

        // Заглушки для кнопок Создать/Обновить/Удалить
        [POST("cards/create")]
        [Authorize]
        public IHttpResponseTypeResult Create([FormData] string Name)
        {
            Context.Response.Redirect("/admin/cards");
            return Html("Создано");
        }

        [POST("cards/update")]
        [Authorize]
        public IHttpResponseTypeResult Update([FormData] int Id)
        {
            Context.Response.Redirect("/admin/cards");
            return Html("Обновлено");
        }

        [POST("cards/delete")]
        [Authorize]
        public IHttpResponseTypeResult Delete([FormData] int Id)
        {
            Context.Response.Redirect("/admin/cards");
            return Html("Удалено");
        }

        // Логин через /admin тоже должен работать
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
            // копируем логику из AuthController, чтобы /admin/login тоже работал
            if (login != "student" || password != "test123")
            {
                return Json(new { Success = false, Message = "Неверный логин или пароль" });
            }
            string sessionId = SessionService.CreateSession(1);
            var cookie = new System.Net.Cookie("sessionId", sessionId, "/");
            cookie.Expires = System.DateTime.UtcNow.AddMinutes(30);
            Context.Response.SetCookie(cookie);
            return Json(new { Success = true, Message = "Вход выполнен" });
        }

        [GET("logout")]
        public IHttpResponseTypeResult LogoutGet()
        {
            string v = Context.Request.Cookies["sessionId"]?.Value;
            SessionService.RemoveSession(v);
            var c = new System.Net.Cookie("sessionId", "", "/");
            c.Expires = System.DateTime.UtcNow.AddDays(-1);
            Context.Response.SetCookie(c);
            Context.Response.Redirect("/");
            return Html("Выход");
        }

        [POST("logout")]
        public IHttpResponseTypeResult LogoutPost()
        {
            string v = Context.Request.Cookies["sessionId"]?.Value;
            SessionService.RemoveSession(v);
            var c = new System.Net.Cookie("sessionId", "", "/");
            c.Expires = System.DateTime.UtcNow.AddDays(-1);
            Context.Response.SetCookie(c);
            return Json(new { Success = true });
        }
    }
}
