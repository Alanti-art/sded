using ConsoleApp;
using CustomHttpServer.Framework.Attributes;
using MiniHttpServer.ORM;
using MyHttpServer.frameworks;
using MyHttpServer.frameworks.HttpResponses;
using MyHttpServer.frameworks.Аttributes;
using MyHttpServer.Models;
using System;
using System.IO;
using System.Text.Json;

namespace MyHttpServer.Controllers
{
    [HttpController("home")]
    public class HomeController : ControllerBase
    {
        private static readonly HtmlTemplateRenderer Renderer = new HtmlTemplateRenderer();
        private static string ConnectionString
        {
            get
            {
                var settings = JsonSerializer.Deserialize<Settings>(File.ReadAllText("settings.json"));
                return settings.ConnectionString;
            }
        }


        [GET("")]
        public IHttpResponseTypeResult Main()
        {
            var orm = new OrmContext(ConnectionString);
            var comments = orm.GetAllAsync<Comment>("Comments").Result;

            string template = File.ReadAllText(
                Path.Combine(AppContext.BaseDirectory, "static", "main.html"));

            string html = Renderer.RenderFromString(template, new
            {
                Comments = comments,
                HasComments = comments.Count > 0
            });

            return Html(html);
        }
    }
}

