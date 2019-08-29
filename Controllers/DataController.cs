using System;
using System.Web;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Asterisk.Models;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Asterisk.Controllers
{
    public partial class DataController : Controller
    {

        private readonly IConfiguration configuration;

        // параметр в конструкторе - инъекция зависимости от конфигурации
        public DataController (IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // Адрес для тестов будет /Data/Index или просто /Data (Action=Index в маршруте по умолчанию)
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Phones()
        {
            return View();
        }

        // экшн который возвращает json
        // параметры могут браться автоматически из пути (по маршруту) или строки запрос или из тела, если там json или форма
        public IActionResult GetInfo(string par1)
        {

            var list = new List<string>();

            list.Add(par1);

            // это из appsettings.json потому что проект на коре
            var cs = configuration.GetConnectionString("CS-Asterisk");
            // тут надо какой-то слой для данных
            // в портале я использовал Dapper, можно посмотреть в App_Code/Controllers/Dealer.Order.cs
            using (var con = new SqlConnection(cs))
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "select F1 = 'test' union all select 'kek'";
                    using (var reader = cmd.ExecuteReader()) {
                        while (reader.Read())
                        {
                            list.Add(reader[0] as string);
                        }
                    }
                }
            }

            return Json(list);
        }

        // экшн который возвращает разметку
        public IActionResult GetHtml ()
        {

            var list = new List<string>();

            var cs = configuration.GetConnectionString("CS-Asterisk");
            using (var con = new SqlConnection(cs))
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    /*cmd.CommandText = "select F1 = 'test' union all select 'kek'";*/

                    cmd.CommandText = "EXECUTE[xp_Device_State-Json]";

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(reader[0] as string);
                        }
                    }
                }
            }

            ViewBag.Data = list;
            return View("Html");
        }

        // как всё это в vue подключить лучше спросить у Коли
    }
}
