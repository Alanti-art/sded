//using CustomHttpServer.Framework.Attributes;
//using Microsoft.Data.SqlClient;
//using MyHttpServer.frameworks;
//using MyHttpServer.frameworks.HttpResponses;
//using MyHttpServer.frameworks.Аttributes;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Cryptography.X509Certificates;
//using System.Text;
//using System.Text.Json;
//using System.Threading.Tasks;

//namespace MyHttpServer.Controllers
//{
//    [HttpController("characters")]
//    public class CardgameController : ControllerBase
//    {
//        string connectionString = Settings.Instance.ConnectionString;


//        SqlConnection connection = new SqlConnection(connectionString);

     

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
//        [GET("delete")]
//        public IHttpResponseTypeResult Delete(int id)
//        {

//            var orm = new OrmContext(connectionString);
//            int countOfDel = orm.DeleteAsync("Users", id).Result;


//            return Json(countOfDel);
//        }
//    }
//}
