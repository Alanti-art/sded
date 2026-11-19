//using CustomHttpServer.Framework.Attributes;
//using MyHttpServer.frameworks;
//using MyHttpServer.frameworks.HttpResponses;
//using MyHttpServer.frameworks.Аttributes;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Cryptography.X509Certificates;
//using System.Text;
//using System.Threading.Tasks;

//namespace MyHttpServer.Controllers
//{
//    [HttpController("characters")]
//    public class CardgameController : ControllerBase
//    {
//        [GET("list")]
//        public IHttpResponseTypeResult List()
//        {
//            string html = File.ReadAllText(
//             Path.Combine(AppContext.BaseDirectory, "static", "characters.html"));
//            return Html(html);
            
//        }
//        [POST("edit")]
//        public IHttpResponseTypeResult Edit()
//        {
//            return 
//        }
//        [POST("detele")]
//        public IHttpResponseTypeResult Delete()
//        {
//            return
//        }
//    }
//}
