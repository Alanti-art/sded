using ConsoleApp;
using CustomHttpServer.Framework.Attributes;
using MyHttpServer.frameworks;
using MyHttpServer.frameworks.HttpResponses;
using MyHttpServer.frameworks.Аttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MyHttpServer.Controllers
{
    [HttpController("characters")]
    public class CardgameController : ControllerBase
    {
        [GET("list")]
        public IHttpResponseTypeResult Characters()
        {
            string templatePath = Path.Combine(AppContext.BaseDirectory, "static", "characters.html");
            string template = File.ReadAllText(templatePath);

            string? sessionId = Context.Request.Cookies["sessionId"]?.Value;
            bool isAuthenticated = SessionService.GetUserId(sessionId).HasValue;

            var characters = new[]
            {
                new { Link = "/card-template.html?name=Герцог", Image = "https://via.placeholder.com/300x290?text=Герцог", Name = "Герцог", Description = "Берёт 3 монеты из казны." },
                new { Link = "/card-template.html?name=Ассасин", Image = "https://via.placeholder.com/300x290?text=Ассасин", Name = "Ассасин", Description = "Убивает персонажа за 3 монеты." },
                new { Link = "/card-template.html?name=Капитан", Image = "https://via.placeholder.com/300x290?text=Капитан", Name = "Капитан", Description = "Крадёт 2 монеты у соперника." },
                new { Link = "/card-template.html?name=Посол", Image = "https://via.placeholder.com/300x290?text=Посол", Name = "Посол", Description = "Меняет карты в руке." },
            };

            var renderer = new HtmlTemplateRenderer();
            string html = renderer.RenderFromString(template, new { IsAuthenticated = isAuthenticated, Characters = characters });
            return Html(html);
        }
    }
}
