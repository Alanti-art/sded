using CustomHttpServer.Framework.Handlers;
using System.IO;
using System.Net;

namespace MyHttpServer.frameworks.Новая_папка
{
    // Отдает статику (html, css, картинки)
    internal class StaticFileHandler : Handler
    {
        // Сервис для проверки куки
        private readonly MyHttpServer.frameworks.Authentication.IUserSessionService _sessionService;

        public StaticFileHandler(MyHttpServer.frameworks.Authentication.IUserSessionService sessionService = null)
        {
            _sessionService = sessionService;
        }

        public override async Task HandleRequest(HttpListenerContext context)
        {
            var request = context.Request;
            string path = request.Url.LocalPath;
            var response = context.Response;

            // Главная страница
            if (string.IsNullOrEmpty(path) || path == "/" || path == "/main/")
            {
                path = "/main.html";
            }

            // --- простая защита ---
            // Эти странички можно смотреть только если есть кука
            if (_sessionService != null)
            {
                string low = path.ToLower();
                bool secret = (low == "/cards.html" || low == "/add-card.html" || low == "/edit-card.html");
                if (secret)
                {
                    string sess = context.Request.Cookies["sessionId"]?.Value;
                    bool ok = _sessionService.GetUserId(sess).HasValue;
                    if (!ok)
                    {
                        // нет куки - отправляем на логин
                        response.Redirect("/auth/login");
                        response.Close();
                        return;
                    }
                }
            }

            // Если в пути есть точка - считаем что это файл
            bool isFile = path.Contains(".");

            if (isFile)
            {
                string filePath = Directory.GetCurrentDirectory() + $"/static/{path}";
                FileInfo fileInfo = new FileInfo(filePath);

                if (!fileInfo.Exists)
                {
                    response.StatusCode = 404;
                    filePath = Directory.GetCurrentDirectory() + $"/static/404.html";
                    if (!File.Exists(filePath))
                    {
                        response.Close();
                        return;
                    }
                    fileInfo = new FileInfo(filePath);
                }

                // выбираем правильный Content-Type
                switch (fileInfo.Extension.ToLower())
                {
                    case ".html":
                    case ".htm":
                        response.ContentType = "text/html; charset=utf-8";
                        break;
                    case ".css":
                        response.ContentType = "text/css; charset=utf-8";
                        break;
                    case ".js":
                    case ".mjs":
                        response.ContentType = "text/javascript; charset=utf-8";
                        break;
                    case ".json":
                    case ".map":
                        response.ContentType = "application/json; charset=utf-8";
                        break;
                    case ".png":
                        response.ContentType = "image/png";
                        break;
                    case ".jpg":
                    case ".jpeg":
                        response.ContentType = "image/jpeg";
                        break;
                    case ".gif":
                        response.ContentType = "image/gif";
                        break;
                    case ".svg":
                        response.ContentType = "image/svg+xml";
                        break;
                    default:
                        response.ContentType = "application/octet-stream";
                        break;
                }

                byte[] buffer = File.ReadAllBytes(filePath);
                response.ContentLength64 = buffer.Length;
                using Stream output = response.OutputStream;
                await output.WriteAsync(buffer);
                await output.FlushAsync();
            }
            else if (Successor != null)
            {
                // это не файл, пусть разбирается контроллер
                await Successor.HandleRequest(context);
            }
        }
    }
}
