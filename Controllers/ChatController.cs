using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Asterisk.Controllers
{
    public partial class DataController : Controller
    {
        public IActionResult SetMessages()
        {
            string messages_data = "Ответ от data/SetMessages";
            return Ok(messages_data);
        }
    }
}