using ConsoleApp;
using CustomHttpServer.Framework.Attributes;
using MyHttpServer.frameworks;
using MyHttpServer.frameworks.Аttributes;
using System.IO;

namespace MyHttpServer.Controllers
{
    // Показываем карточки
    [HttpController("characters")]
    public class CardgameController : ControllerBase
    {
        [GET("list")]
        public IHttpResponseTypeResult Characters()
        {
            // Читаем шаблон
            string path = Path.Combine(AppContext.BaseDirectory, "static", "characters.html");
            string template = File.ReadAllText(path);

            // Проверяем куку - вошел ли человек
            string cookie = Context.Request.Cookies["sessionId"]?.Value;
            bool isAuth = SessionService.GetUserId(cookie).HasValue;

            // Простые тестовые карточки
            var characters = new[]
            {
                new { Link = "/card-template.html", Image = "https://via.placeholder.com/300", Name = "Герцог", Description = "Берёт 3 монеты" },
                new { Link = "/card-template.html", Image = "https://via.placeholder.com/300", Name = "Ассасин", Description = "Убивает за 3 монеты" },
                new { Link = "/card-template.html", Image = "https://via.placeholder.com/300", Name = "Капитан", Description = "Крадёт 2 монеты" },
                new { Link = "/card-template.html", Image = "https://via.placeholder.com/300", Name = "Посол", Description = "Меняет карты" },
            };

            // Рендерим шаблон с флагом
            var renderer = new HtmlTemplateRenderer();
            string html = renderer.RenderFromString(template, new { IsAuthenticated = isAuth, Characters = characters });

            return Html(html);
        }
    }
}
