using CustomHttpServer.Framework.Attributes;
using MiniHttpServer.ORM;
using MyHttpServer.Models;
using MyHttpServer.frameworks;
using MyHttpServer.frameworks.HttpResponses;
using MyHttpServer.frameworks.Аttributes;
using System;
using System.IO;
using System.Text.Json;

namespace MyHttpServer.Controllers
{
    [HttpController("comment")]
    public class CommentsController : ControllerBase
    {
        private static string ConnectionString
        {
            get
            {
                var settings = JsonSerializer.Deserialize<Settings>(File.ReadAllText("settings.json"));
                return settings.ConnectionString;
            }
        }




        [GET("comments")]
        public IHttpResponseTypeResult List()
        {
            var orm = new OrmContext(ConnectionString);
            var comments = orm.GetAllAsync<Comment>("Comments").Result;
            return Json(comments);
        }

        [POST("add")]
        public IHttpResponseTypeResult Add([FormData] string name, [FormData] string text, [FormData] string rating)
        {
            var orm = new OrmContext(ConnectionString);
            orm.InsertAsync("Comments", new Comment
            {
                Name = name ?? "",
                Text = text ?? "",
                Rating = rating ?? "",
                CreatedAt = DateTime.Now
            }).Wait();

            Context.Response.StatusCode = 302;
            Context.Response.Headers.Add("Location", "/");
            return Html("");
        }
    }
}