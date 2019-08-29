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

        // экшн который возвращает список сотрудников с телефонами
        public IActionResult GetUsers()
        {
            string data = "";
            var cs = configuration.GetConnectionString("CS-Asterisk");
            using (var con = new SqlConnection(cs))
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "EXECUTE[xp_Users_Json]";
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

        // экшн который возвращает список отделов
        public IActionResult GetOtdels()
        {
            string data = "";
            var cs = configuration.GetConnectionString("CS-Asterisk");
            using (var con = new SqlConnection(cs))
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "EXECUTE[xp_Otdels_Json]";
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

        // экшн который возвращает данные по телефонам сотрудников
        public IActionResult GetPhones(string par1)
        {
            string phones_data = "";
            var cs = configuration.GetConnectionString("CS-Asterisk");
            using (var con = new SqlConnection(cs))
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "EXECUTE[xp_Device_State-Json]";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            phones_data += reader[0].ToString();
                        }
                    }
                }
            }
            return Ok(phones_data);
        }
    }
}
