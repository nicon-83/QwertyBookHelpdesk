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
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Asterisk.Controllers
{
    public partial class DataController : Controller
    {

        //Не используется!!!
        // экшн возвращает данные для выбора периода  [{"NameDate":"2019 Январь","BeginDate":"2019-01-01T00:00:00","EndDate":"2019-01-31T23:59:59.990"},{"NameDate":"2018 Декабрь","
        public IActionResult GetDateForHistory(string par1)
        {
            string data = "";
            var cs = configuration.GetConnectionString("CS-Asterisk");
            using (var con = new SqlConnection(cs))
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM [dbo].[GetDateForHistory] () FOR JSON AUTO";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            data += reader[0].ToString();
                        }
                    }
                }
            }
            return Ok(data);
        }

        // экшн возвращает историю звонков
        public IActionResult GetHistory(string par1)
        {

            var q = HttpContext.Request.Query;

            StringValues xpName;

            StringValues dateBegin;
            StringValues dateEnd;
            StringValues disposition;
            StringValues dispositionCalc;
            StringValues otdel;
            StringValues otdelCalc;
            StringValues user;
            StringValues userCalc;
            StringValues client;
            string clientCalc;
            StringValues filter;
            string filterCalc;
            StringValues row_num_begin;
            StringValues row_num_end;



            if ((!q.TryGetValue("xpName", out xpName)) || (! q.TryGetValue("DateBegin", out dateBegin))  || (! q.TryGetValue("DateEnd", out dateEnd)))
            {
                return Ok("");
            }

            q.TryGetValue("Disposition", out disposition);
            dispositionCalc = disposition.ToString();
            if (dispositionCalc == "ALL" || userCalc.ToString().Contains("object"))
            {
                dispositionCalc = "";
            }
            dispositionCalc = "%" + dispositionCalc + "%";

            q.TryGetValue("Filter", out filter);
            filterCalc = filter.ToString();
            if (filterCalc.ToString().Contains("object"))
            {
                filterCalc = "";
            }

            q.TryGetValue("Otdel", out otdel);
            otdelCalc = otdel.ToString();
            if (otdelCalc == "Все отделы" || userCalc.ToString().Contains("object"))
            {
                otdelCalc = "";
            }
            otdelCalc = "%" + otdelCalc + "%";

            q.TryGetValue("User", out user);
            userCalc = user.ToString();
            if (userCalc == "Все сотрудники" || userCalc == "" || userCalc.ToString().Contains("object"))
            {
                userCalc = "";
            } else
            {
                userCalc = userCalc.ToString().Substring(5); //del "101 "
            }
            userCalc = "%" + userCalc + "%";

            q.TryGetValue("Client", out client);
            clientCalc = client.ToString();
            clientCalc = "%" + clientCalc.Trim() + "%";

            q.TryGetValue("row_num_begin", out row_num_begin);
            q.TryGetValue("row_num_end", out row_num_end);

            string data = "";
            var cs = configuration.GetConnectionString("CS-Asterisk");
            using (var con = new SqlConnection(cs))
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    if (xpName == "xp_History_Json") {
                        cmd.CommandText = String.Format("EXEC [dbo].[xp_History_Json] @begin = N'{0}', @end = N'{1}', @disposition = '{2}', @otdel = '{3}', @user = '{4}', @filter = '{5}', @client = '{6}', @row_num_begin = {7}, @row_num_end = {8}", dateBegin.ToString(), dateEnd.ToString(), dispositionCalc, otdelCalc, userCalc, filterCalc, clientCalc, row_num_begin.ToString(), row_num_end.ToString());
                    }
                    if (xpName == "xp_History_EmptyNpp_Json")
                    {
                        cmd.CommandText = String.Format("EXEC [dbo].[xp_History_EmptyNpp_Json] @begin = N'{0}', @end = N'{1}', @disposition = '{2}', @otdel = '{3}', @user = '{4}', @client = '{5}', @row_num_begin = {6}, @row_num_end = {7}", dateBegin.ToString(), dateEnd.ToString(), dispositionCalc, otdelCalc, userCalc, clientCalc, row_num_begin.ToString(), row_num_end.ToString());
                    }
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            data += reader[0].ToString();
                        }
                    }
                }
            }
            return Ok(data);
        }

        // экшн возвращает историю звонков
        public IActionResult GetHistoryUniqueid(string par1)
        {

            var q = HttpContext.Request.Query;

            StringValues @uniqueid1;
            StringValues @uniqueid2;
            StringValues @uniqueid3;
            StringValues @uniqueid4;
            StringValues @uniqueid5;

            q.TryGetValue("uniqueid1", out uniqueid1);
            q.TryGetValue("uniqueid2", out uniqueid2);
            q.TryGetValue("uniqueid3", out uniqueid3);
            q.TryGetValue("uniqueid4", out uniqueid4);
            q.TryGetValue("uniqueid5", out uniqueid5);

            string data = "";
            var cs = configuration.GetConnectionString("CS-Asterisk");
            using (var con = new SqlConnection(cs))
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = String.Format("EXEC [dbo].[xp_History_Uniqueid_Json] @uniqueid1 = N'{0}', @uniqueid2 = N'{1}', @uniqueid3 = N'{2}', @uniqueid4 = N'{3}', @uniqueid5 = N'{4}'", uniqueid1.ToString(), uniqueid2.ToString(), uniqueid3.ToString(), uniqueid4.ToString(), uniqueid5.ToString());
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            data += reader[0].ToString();
                        }
                    }
                }
            }
            return Ok(data);
        }

    }
}
