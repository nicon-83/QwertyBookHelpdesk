using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using MimeKit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QB.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Asterisk.Controllers
{
    public partial class DataController : Controller
    {

#if TEST_DATABASE
        private readonly string serverUrl = "https://192.168.250.154:8443";
#else
        private readonly string serverUrl = "https://qb.qwerty.plus";
#endif

#if TEST_DATABASE
        private readonly string serverUrlClient = "https://192.168.250.154";
#else
        private readonly string serverUrlClient = "https://qb.qwerty.plus:4430";
#endif

        private string GetConnString()
        {
#if TEST_DATABASE
            return configuration.GetConnectionString("QB_TEST");
#else
            return configuration.GetConnectionString("QB");
#endif
        }

#if TEST_DATABASE
        private readonly string attachmentsPath = "d:/PHPStormProject/mail_parser_for_QB/Attachments";
#else
        private readonly string attachmentsPath = "d:/mail_parser_for_QB/Attachments";
#endif

        //списки email для рассылки информационных писем
        //private static class MailingAddresses
        //{
        //    public static string[] ResponseToClient = new string[] { "gop@qwerty.perm.ru", "dima@qwerty.perm.ru", "tanya@qwerty.perm.ru", "nataly@qwerty.perm.ru", "talankin.a@qwerty.perm.ru" };
        //    public static string[] TicketCreated = new string[] { "gop@qwerty.perm.ru", "dima@qwerty.perm.ru", "tanya@qwerty.perm.ru", "nataly@qwerty.perm.ru", "talankin.a@qwerty.perm.ru" };
        //    public static string[] TicketAssigned = new string[] { "gop@qwerty.perm.ru", "dima@qwerty.perm.ru", "tanya@qwerty.perm.ru", "nataly@qwerty.perm.ru", "talankin.a@qwerty.perm.ru" };
        //    public static string[] ResponseFromClient = new string[] { "gop@qwerty.perm.ru", "dima@qwerty.perm.ru", "tanya@qwerty.perm.ru", "nataly@qwerty.perm.ru", "talankin.a@qwerty.perm.ru" };
        //    public static string[] ExecutionReport = new string[] { "gop@qwerty.perm.ru", "dima@qwerty.perm.ru", "tanya@qwerty.perm.ru", "nataly@qwerty.perm.ru", "talankin.a@qwerty.perm.ru" };
        //    public static string[] ReturnedToWork = new string[] { "gop@qwerty.perm.ru", "dima@qwerty.perm.ru", "tanya@qwerty.perm.ru", "nataly@qwerty.perm.ru", "talankin.a@qwerty.perm.ru" };
        //}


#if TEST_DATABASE
        private static class MailingAddresses
        {
            public static string[] ResponseToClient = new string[] { "nicon-83@mail.ru" };
            public static string[] TicketCreated = new string[] { "nicon-83@mail.ru" };
            public static string[] TicketAssigned = new string[] { "nicon-83@mail.ru" };
            public static string[] ResponseFromClient = new string[] { "nicon-83@mail.ru" };
            public static string[] ExecutionReport = new string[] { "nicon-83@mail.ru" };
            public static string[] ReturnedToWork = new string[] { "nicon-83@mail.ru" };
        }
#else
        private static class MailingAddresses
        {
            public static string[] ResponseToClient = new string[] { "nicon-83@mail.ru", "gop@qwerty.plus", "a.talankin@qwerty.plus", "d.makerov@qwerty.plus", "t.botalova@qwerty.plus" };
            public static string[] TicketCreated = new string[] { "nicon-83@mail.ru", "gop@qwerty.plus", "a.talankin@qwerty.plus", "d.makerov@qwerty.plus", "t.botalova@qwerty.plus"};
            public static string[] TicketAssigned = new string[] { "nicon-83@mail.ru", "gop@qwerty.plus", "a.talankin@qwerty.plus", "d.makerov@qwerty.plus", "t.botalova@qwerty.plus" };
            public static string[] ResponseFromClient = new string[] { "nicon-83@mail.ru", "gop@qwerty.plus", "a.talankin@qwerty.plus", "d.makerov@qwerty.plus", "t.botalova@qwerty.plus" };
            public static string[] ExecutionReport = new string[] { "nicon-83@mail.ru", "gop@qwerty.plus", "a.talankin@qwerty.plus", "d.makerov@qwerty.plus", "t.botalova@qwerty.plus" };
            public static string[] ReturnedToWork = new string[] { "nicon-83@mail.ru", "gop@qwerty.plus", "a.talankin@qwerty.plus", "d.makerov@qwerty.plus", "t.botalova@qwerty.plus" };
        }
#endif

#if TEST_DATABASE
        private readonly string InfMails = "nicon-83@mail.ru";
#else
        private readonly string InfMails = "nicon-83@mail.ru,gop@qwerty.plus,a.talankin@qwerty.plus,d.makerov@qwerty.plus,t.botalova@qwerty.plus";
#endif

        private static class InformationMessageType
        {
            public static string newMessage = "newMessage";
            public static string newTicket = "newTicket";
            public static string returnedToWork = "returnedToWork";
        }

        //НЕ ИСПОЛЬЗУЕТСЯ!!! получаем список всех заявок для админской части
        [HttpGet]
        public async Task<IActionResult> GetTickets()
        {
            var list = new List<Ticket>();

            var cs = GetConnString();

            using (var con = new SqlConnection(cs))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"select t.NPP                            Npp,
                                            t.number                         number,
                                            t.title                          title,
                                            c.NAME                           customer,
                                            t.state_npp                      state,
                                            t.priority_npp                   priority,
                                            p.FIRST_NAME + ' ' + p.LAST_NAME agent,
                                            t.create_time                    create_time
                                    from Ticket t
                                            left join COMPANYS C on t.customer_npp = C.NPP
                                            left join PEOPLES P on t.agent_npp = P.NPP;";

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new Ticket
                            {
                                Npp = reader.GetInt64(0),
                                //Number = reader["number"] as string,
                                //Title = reader["title"] as string,
                                //Customer = reader["customer"] as string,
                                //State = reader["state"] as string,
                                //Priority = reader["priority"] as string,
                                //Agent = reader["agent"] as string,
                                //CreateTime = reader.GetDateTime(7),
                            };

                            list.Add(row);
                        }
                    }
                }
            }

            return Json(list);
        }

        //получаем список всех заявок клиента, используется для клиентской части в компоненте MainSupport mounted() для проверки наличия у клиента запрошенной заявки
        [HttpGet]
        public async Task<IActionResult> GetClientTickets()
        {
            var cs = GetConnString();
            var list = new List<Ticket>();
            var q = HttpContext.Request.Query;
            q.TryGetValue("apteka_npp", out StringValues apteka_npp);

            using (var con = new SqlConnection(cs))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"select NPP from Ticket where customer_npp = @apteka_npp;";

                    cmd.Parameters.AddWithValue("apteka_npp", apteka_npp.ToString());

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new Ticket
                            {
                                Npp = reader.GetInt64(0)
                            };

                            list.Add(row);
                        }
                    }
                }
            }

            return Json(list);
        }

        //получаем список всех заявок АПТЕКИ в которых есть непросмотренные изменения
        [HttpGet]
        public async Task<IActionResult> GetAllTicketsWithChanges()
        {
            var cs = GetConnString();
            var list = new List<NotSeenedTicket>();
            var q = HttpContext.Request.Query;
            q.TryGetValue("apteka_npp", out StringValues apteka_npp);
            q.TryGetValue("userId", out StringValues userId);

            using (var con = new SqlConnection(cs))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    //cmd.CommandText = @"select t.NPP, t.number
                    //                    from Ticket t
                    //                           left join HistoryMessages h on t.NPP = h.ticket_npp
                    //                    where h.client_npp = @apteka_npp
                    //                      and t.last_update_time is not null
                    //                      and t.last_update_time > h.seen_time;";

                    //cmd.CommandText = @"select t.NPP, t.number, (select count(m.NPP) from TicketMessages m where m.ticket_npp = t.NPP
                    //                                                              and (m.create_time > h.seen_time
                    //                                                                     or h.seen_time is null)) newMessagesCount
                    //                            from Ticket t
                    //                                   left join HistoryMessages h on t.NPP = h.ticket_npp
                    //                            where h.client_npp = @apteka_npp
                    //                              and t.last_update_time is not null
                    //                              and t.last_update_time > h.seen_time;";

                    cmd.CommandText = @"with res1 as (select t.NPP, t.number, h.agent_npp, h.client_npp, h.userId, t.last_update_time, h.seen_time, (select 1 from HistoryMessages where ticket_npp = t.npp
                                                                                                                                             and client_npp = @apteka_npp
                                                                                                                                             and userId = @userId) hasRecord
                                                          from Ticket t
                                                                 left join HistoryMessages h on t.NPP = h.ticket_npp where t.customer_npp = @apteka_npp),
                                            res2 as (select DISTINCT NPP,
                                                            number,
                                                            case
                                                              when hasRecord is null then (select count(m.NPP) from TicketMessages m where m.ticket_npp = res1.NPP
                                                                                                                                       and (m.create_time > CAST('1970-01-01' AS datetime))
                                                                                                                                       and (m.internal_note <> 1 or m.internal_note is null))
                                                              when hasRecord = 1 then (select count(m.NPP) from TicketMessages m where m.ticket_npp = res1.NPP
                                                                                                                                   and (m.create_time > res1.seen_time)
                                                                                                                                   and (m.internal_note <> 1 or m.internal_note is null))
                                                                end newMessagesCount
                                            from res1
                                            where (res1.hasRecord is null) --включаем заявки у которых нет записи в HistoryMessages, т.е. клиент ни разу не заходил в заявку
                                               or (res1.last_update_time > res1.seen_time and res1.client_npp = @apteka_npp and res1.userId = @userId))
                                            select * from res2 where newMessagesCount > 0;";

                    cmd.Parameters.AddWithValue("apteka_npp", apteka_npp.ToString());
                    cmd.Parameters.AddWithValue("userId", userId.ToString());

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new NotSeenedTicket
                            {
                                Npp = reader.GetInt64(0),
                                Number = reader.GetString(1),
                                NewMessagesCount = reader.GetInt32(2)
                            };

                            list.Add(row);
                        }
                    }
                }
            }

            return Json(list);
        }

        //получаем список заявок с непросмотренными изменениями в которых участвует пользователь, используется для вывода уведомлений "В заявке №ххх есть новое сообщение"
        [HttpGet]
        public async Task<IActionResult> GetUserTicketsWithChanges()
        {
            var cs = GetConnString();
            var list = new List<NotSeenedTicket>();
            var q = HttpContext.Request.Query;
            q.TryGetValue("apteka_npp", out StringValues apteka_npp);
            q.TryGetValue("userId", out StringValues userId);

            using (var con = new SqlConnection(cs))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    //cmd.CommandText = @"select distinct t.NPP, t.number
                    //                        from Ticket t
                    //                               left join TicketMessages m on t.NPP = m.ticket_npp
                    //                               left join HistoryMessages h on t.NPP = h.ticket_npp
                    //                        where t.customer_npp = @apteka_npp
                    //                          and m.userId = @userId
                    //                          and h.userId = @userId
                    //                          and t.last_update_time > h.seen_time
                    //                          and (m.internal_note <> 1 or m.internal_note is null)
                    //                        order by t.NPP;";

                    cmd.CommandText = @"with res1 as (select distinct t.NPP, t.number, (select count(npp)
                                                from TicketMessages mm
                                                where mm.ticket_npp = t.npp
                                                  and (mm.internal_note <> 1 or mm.internal_note is null)
                                                  and mm.create_time > h.seen_time) countMessages
                                                from Ticket t
                                                        left join TicketMessages m on t.NPP = m.ticket_npp
                                                        left join HistoryMessages h on t.NPP = h.ticket_npp
                                                where t.customer_npp = @apteka_npp
                                                and m.userId = @userId
                                                and h.userId = @userId
                                                and t.last_update_time > h.seen_time)
                                select distinct res1.NPP, res1.number, res1.countMessages
                                        from res1
                                        where res1.countMessages > 0
                                        order by res1.NPP;";

                    cmd.Parameters.AddWithValue("apteka_npp", apteka_npp.ToString());
                    cmd.Parameters.AddWithValue("userId", userId.ToString());

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new NotSeenedTicket
                            {
                                Npp = reader.GetInt64(0),
                                Number = reader.GetString(1),
                                NewMessagesCount = reader.GetInt32(2)
                            };

                            list.Add(row);
                        }
                    }
                }
            }

            return Json(list);
        }

        //НЕ ИСПОЛЬЗУЕТСЯ!!! получаем список заявок отфильтрованных с учетом параметров пагинации для админской части
        [HttpGet]
        public IActionResult GetTicketsPagination()
        {
            var q = HttpContext.Request.Query;


            q.TryGetValue("row_num_begin", out StringValues row_num_begin);
            q.TryGetValue("row_num_end", out StringValues row_num_end);
            q.TryGetValue("filterStates", out StringValues state);
            q.TryGetValue("filterClients", out StringValues client);

            var list = new List<TicketPagination>();

            var cs = GetConnString();

            using (var con = new SqlConnection(cs))
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"with tickets as (select ROW_NUMBER() OVER (ORDER BY t.NPP DESC) row_num,
                                                                t.NPP                              Npp,
                                                                t.number                           number,
                                                                t.title                            title,
                                                                c.NAME                             customer,
                                                                t.state_npp                        state,
                                                                ts.name                            stateName,
                                                                t.priority_npp                     priority,
                                                                tp.name                            priorityname,
                                                                tp.color                           priorityColor,
                                                                p.FIRST_NAME + ' ' + p.LAST_NAME   agent,
                                                                t.create_time                      create_time
                                                         from Ticket t
                                                                left join COMPANYS C on t.customer_npp = C.NPP
                                                                left join PEOPLES P on t.agent_npp = P.NPP
                                                                left join TicketState ts on ts.NPP = t.state_npp
                                                                left join TicketPriority tp on tp.NPP = t.priority_npp
                                                         where ts.name like @state
                                                            and C.NAME like @client)
                                        SELECT (SELECT max(row_num) from tickets) as max_row,
                                               d.row_num,
                                               d.NPP,
                                               d.create_time,
                                               d.number,
                                               d.title,
                                               d.customer,
                                               d.state,
                                               d.stateName,
                                               d.priority,
                                               d.priorityName,
                                               d.priorityColor,
                                               d.agent
                                        from tickets as d
                                        where row_num between @row_num_begin and @row_num_end
                                        order by row_num";
                    cmd.Parameters.AddWithValue("row_num_begin", row_num_begin.ToString());
                    cmd.Parameters.AddWithValue("row_num_end", row_num_end.ToString());
                    cmd.Parameters.AddWithValue("state", "%" + state.ToString() + "%");
                    cmd.Parameters.AddWithValue("client", "%" + client.ToString() + "%");

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = new TicketPagination
                            {
                                MaxRow = reader.GetInt64(0),
                                RowNum = reader.GetInt64(1),
                                Npp = reader.GetInt64(2),
                                CreateTime = string.Format("{0:G}", reader.GetDateTime(3)),
                                Number = reader["number"] as string,
                                Title = reader["title"] as string,
                                Customer = reader["customer"] as string,
                                State = reader["state"] as string,
                                StateName = reader["stateName"] as string,
                                Priority = reader["priority"] as string,
                                PriorityName = reader["priorityName"] as string,
                                PriorityColor = reader["priorityColor"] as string,
                                Agent = reader["agent"] as string
                            };

                            list.Add(row);
                        }
                    }
                }
            }

            return Json(list);

        }

        //получаем список заявок конкретной аптеки отфильтрованных с учетом параметров пагинации
        [HttpGet]
        public IActionResult GetClientTicketsPagination()
        {
            var q = HttpContext.Request.Query;

            q.TryGetValue("row_num_begin", out StringValues row_num_begin);
            q.TryGetValue("row_num_end", out StringValues row_num_end);
            q.TryGetValue("apteka_npp", out StringValues apteka_npp);
            q.TryGetValue("filterStates", out StringValues state);
            //q.TryGetValue("filterClients", out StringValues client);

            var list = new List<TicketPagination>();

            var cs = GetConnString();

            using (var con = new SqlConnection(cs))
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"with tickets as (select ROW_NUMBER() OVER (ORDER BY t.last_update_time DESC) row_num,
                                                                t.NPP                              Npp,
                                                                t.number                           number,
                                                                t.title                            title,
                                                                c.NAME                             customer,
                                                                cast(c.NPP as varchar(10))         customer_npp,
                                                                t.state_npp                        state,
                                                                ts.name                            stateName,
                                                                t.priority_npp                     priority,
                                                                tp.name                            priorityname,
                                                                tp.color                           priorityColor,
                                                                p.FIRST_NAME + ' ' + p.LAST_NAME   agent,
                                                                t.create_time                      create_time,
                                                                coalesce(t.last_update_time, CAST('1970-01-01' AS datetime))               last_update,
                                                                isnull(cast(u.id as varchar(10)), '')                           userId,
                                                                isnull(u.lastName + ' ' + u.firstName + ' ' + u.middleName, '') userFullName
                                                         from Ticket t
                                                                left join COMPANYS C on t.customer_npp = C.NPP
                                                                left join PEOPLES P on t.agent_npp = P.NPP
                                                                left join TicketState ts on ts.NPP = t.state_npp
                                                                left join TicketPriority tp on tp.NPP = t.priority_npp
                                                                left join qbLoginUsers u on t.userId = u.id
                                                         where C.NPP = @apteka_npp and ts.name like @state)
                                        SELECT (SELECT max(row_num) from tickets) as max_row,
                                               d.row_num,
                                               d.NPP,
                                               d.create_time,
                                               d.last_update,
                                               d.number,
                                               d.title,
                                               d.customer,
                                               d.customer_npp,
                                               d.state,
                                               d.stateName,
                                               d.priority,
                                               d.priorityName,
                                               d.priorityColor,
                                               d.agent,
                                               d.userId,
                                               d.userFullName
                                        from tickets as d
                                        where row_num between @row_num_begin and @row_num_end
                                        order by row_num";
                    cmd.Parameters.AddWithValue("row_num_begin", row_num_begin.ToString());
                    cmd.Parameters.AddWithValue("row_num_end", row_num_end.ToString());
                    cmd.Parameters.AddWithValue("apteka_npp", apteka_npp.ToString());
                    cmd.Parameters.AddWithValue("state", "%" + state.ToString() + "%");
                    //cmd.Parameters.AddWithValue("client", "%" + client.ToString() + "%");

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = new TicketPagination
                            {
                                MaxRow = reader.GetInt64(0),
                                RowNum = reader.GetInt64(1),
                                Npp = reader.GetInt64(2),
                                CreateTime = string.Format("{0:G}", reader.GetDateTime(3)),
                                LastUpdate = string.Format("{0:G}", reader.GetDateTime(4)),
                                Number = reader["number"] as string,
                                Title = reader["title"] as string,
                                Customer = reader["customer"] as string,
                                CustomerNpp = reader["customer_npp"] as string,
                                State = reader["state"] as string,
                                StateName = reader["stateName"] as string,
                                Priority = reader["priority"] as string,
                                PriorityName = reader["priorityName"] as string,
                                PriorityColor = reader["priorityColor"] as string,
                                Agent = reader["agent"] as string,
                                UserId = reader["userId"] as string,
                                UserFullName = reader["userFullName"] as string
                            };

                            list.Add(row);
                        }
                    }
                }
            }

            return Json(list);

        }

        //получаем список приоритетов заявок
        [HttpGet]
        public async Task<IActionResult> GetPriorities()
        {
            var list = new List<Priority>();

            var cs = GetConnString();

            using (var con = new SqlConnection(cs))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"select cast(p.id as varchar(10)) id, cast(NPP as varchar(10)) npp, p.name name, p.color color 
                                            from TicketPriority p order by p.id;";

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new Priority
                            {
                                Id = reader["id"] as string,
                                Npp = reader["npp"] as string,
                                Name = reader["name"] as string,
                                Color = reader["color"] as string
                            };

                            list.Add(row);
                        }
                    }
                }
            }

            return Json(list);

        }

        //получаем список статусов заявок
        [HttpGet]
        public async Task<IActionResult> GetStates()
        {
            var list = new List<State>();

            var cs = GetConnString();

            using (var con = new SqlConnection(cs))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"select npp, name from TicketState;";

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new State
                            {
                                Npp = reader["npp"] as string,
                                Name = reader["name"] as string,
                            };

                            list.Add(row);
                        }
                    }
                }
            }

            return Json(list);

        }

        //сохранение новой заявки в БД
        [HttpPost]
        public async Task<HttpResponseMessage> CreateTicket([FromBody] JObject data)
        {

            if (data == null)
                return new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, ReasonPhrase = "POST body is null" };

            var cs = GetConnString();
            SqlCommand cmd = null;
            decimal ticketNpp = 0;
            decimal messageNpp = 0;

            var agent = (data["agent"]).ToString();
            var customer = (data["customer"]).ToString();
            var userId = (data["userId"]).ToString();
            var message = (data["message"]).ToString();
            int number = await GetLTN1(customer) + 1;
            var priority = (data["priority"]).ToString();
            var state = (data["state"]).ToString();
            var title = (data["title"]).ToString();
            var idapt = (data["idapt"]).ToString();
            var fromEmail = string.Empty;
            if (data.ContainsKey("fromEmail"))
            {
                fromEmail = (data["fromEmail"]).ToString();
            }
            var attachmentsData = string.Empty;
            if (data.ContainsKey("attachmentsData"))
            {
                attachmentsData = (data["attachmentsData"]).ToString();
            }
            var clientEmails = string.Empty;
            if (data.ContainsKey("clientEmails"))
            {
                clientEmails = (data["clientEmails"]).ToString();
            }

            //формируем список email клиентов для отправки писем
            //используется парсером писем
            string To = fromEmail;
            string Cc = string.Empty;
            string Bcc = string.Empty;
            //if (clientEmails != string.Empty)
            //{
            //    try
            //    {
            //        JObject jsonObj = JObject.Parse(clientEmails);
            //        if (jsonObj.ContainsKey("to"))
            //        {
            //            JToken tokenTo = jsonObj.SelectToken("to");
            //            foreach (JToken item in tokenTo)
            //            {
            //                To += item.ToString().Trim() + ',';
            //            }
            //            To = To.TrimEnd(',');
            //        }
            //        if (jsonObj.ContainsKey("copy"))
            //        {
            //            JToken tokenCopy = jsonObj.SelectToken("copy");
            //            foreach (JToken item in tokenCopy)
            //            {
            //                Cc += item.ToString().Trim() + ',';
            //            }
            //            Cc = Cc.TrimEnd(',');
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        return new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, ReasonPhrase = e.Message.Replace(Environment.NewLine, "") };
            //    }
            //}

            using (var con = new SqlConnection(cs))
            {
                await con.OpenAsync();
                string queryString;

                queryString = $@"insert into Ticket (number, title, customer_npp, userId, state_npp, priority_npp, agent_npp, client_email)
                                    values (@number, @title, @customer, @userId, @state, @priority, @agent, @clientEmails);
                                    select SCOPE_IDENTITY() as ticketNpp;";

                cmd = new SqlCommand(queryString, con);
                cmd.Parameters.AddWithValue("agent", int.Parse(agent));
                cmd.Parameters.AddWithValue("customer", int.Parse(customer));
                cmd.Parameters.AddWithValue("userId", long.Parse(userId));
                cmd.Parameters.AddWithValue("number", number);
                cmd.Parameters.AddWithValue("priority", priority);
                cmd.Parameters.AddWithValue("state", state);
                cmd.Parameters.AddWithValue("title", title);
                cmd.Parameters.AddWithValue("clientEmails", clientEmails);

                try
                {
                    ticketNpp = (decimal)await cmd.ExecuteScalarAsync();
                }
                catch (Exception e)
                {
                    return new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, ReasonPhrase = e.Message.Replace(Environment.NewLine, "") };
                }

                //queryString = $@"select npp from Ticket where number = @number and customer_npp = @customer;";
                //cmd = new SqlCommand(queryString, con);
                //cmd.Parameters.AddWithValue("number", number);
                //cmd.Parameters.AddWithValue("customer", int.Parse(customer));

                //try
                //{
                //    npp = (long)await cmd.ExecuteScalarAsync();
                //}
                //catch (Exception e)
                //{
                //    return new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, ReasonPhrase = e.Message.Replace(Environment.NewLine, "") };
                //}


                //записываем первое сообщение
                queryString = $@"insert into TicketMessages (ticket_npp, message, client_npp, userId, attachments_data, emails) values (@ticketNpp, @message, @customer, @userId, @attachments_data, @clientEmails);
                                select SCOPE_IDENTITY() as messageNpp;";

                cmd = new SqlCommand(queryString, con);
                cmd.Parameters.AddWithValue("ticketNpp", ticketNpp);
                cmd.Parameters.AddWithValue("message", message);
                cmd.Parameters.AddWithValue("customer", int.Parse(customer));
                cmd.Parameters.AddWithValue("userId", long.Parse(userId));
                cmd.Parameters.AddWithValue("attachments_data", attachmentsData);
                cmd.Parameters.AddWithValue("clientEmails", clientEmails);
                try
                {
                    messageNpp = (decimal)await cmd.ExecuteScalarAsync();
                }
                catch (Exception e)
                {
                    return new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, ReasonPhrase = e.Message.Replace(Environment.NewLine, "") };
                }

                //добавляем к каждому элементу массива attachmentsData относительный путь расположения файла вложения
                //структура пути /npp/messageNpp
                if (attachmentsData != string.Empty)
                {
                    try
                    {
                        dynamic obj = JsonConvert.DeserializeObject(attachmentsData);
                        foreach (var attachment in obj)
                        {
                            var name = attachment["file_name"];
                            attachment["file_path"] = $"/{ticketNpp.ToString()}/{messageNpp.ToString()}/{name}";
                        }
                        var resultString = JsonConvert.SerializeObject(obj);

                        //записываем attachmentsData в таблицу TicketMessages
                        queryString = $@"update TicketMessages set attachments_data = @attachmentsData where NPP = @messageNpp;";
                        cmd = new SqlCommand(queryString, con);
                        cmd.Parameters.AddWithValue("attachmentsData", resultString);
                        cmd.Parameters.AddWithValue("messageNpp", messageNpp);
                        await cmd.ExecuteNonQueryAsync();
                    }
                    catch (Exception e)
                    {
                        return new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, ReasonPhrase = e.Message.Replace(Environment.NewLine, "") };
                    }
                }

                //записываем время прочтения этого сообщения в таблицу historyMessages
                queryString = $@"INSERT INTO HistoryMessages (max_message_npp, ticket_npp, client_npp, userId) 
                                                    values ((select max(NPP) from TicketMessages where ticket_npp = @ticketNpp), @ticketNpp, @customer, @userId);";

                cmd = new SqlCommand(queryString, con);
                cmd.Parameters.AddWithValue("ticketNpp", ticketNpp);
                cmd.Parameters.AddWithValue("customer", int.Parse(customer));
                cmd.Parameters.AddWithValue("userId", long.Parse(userId));
                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception e)
                {
                    return new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, ReasonPhrase = e.Message.Replace(Environment.NewLine, "") };
                }
            }


            //отправляем информационные письма

            string emailMessage = string.Empty;
            try
            {
                //string infSubject = $"[Заявка №{idapt}-{number}] создана - {title}";
                string infSubject = CreateMessageTitle(new { ticketTitle = title, idapt, number, messageType = InformationMessageType.newTicket });
                //emailMessage = CreateMessageText(new { ticketNpp, title, message, idapt, number, server = serverUrl, messageType = InformationMessageType.newTicket });
                //await SendInformationMail(MailingAddresses.TicketCreated, infSubject, emailMessage);

                if (clientEmails != string.Empty)
                {
                    emailMessage = CreateMessageText(new { ticketNpp, title, message, idapt, number, server = serverUrlClient, messageType = InformationMessageType.newTicket });
                    //await SendInformationMail(new string[] { fromEmail }, infSubject, emailMessage, "outer");
                    await SendMail(new { To, Cc, Bcc, subject = infSubject, message = emailMessage });
                }
            }
            catch (Exception)
            {
                //выполнение метода не прекращаем, т.к. ошибка не критична для работы приложения, в дальнейшем нужно настроить логирование ошибки в файл.
                return new HttpResponseMessage { StatusCode = HttpStatusCode.OK, ReasonPhrase = $"Ticket Saved;{ticketNpp};{messageNpp};{number};Возникла ошибка при отправке информационного письма" };
            }

            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK, ReasonPhrase = $"Ticket Saved;{ticketNpp};{messageNpp};{number};Информационное письмо отправлено" };

        }

        //НЕ ИСПОЛЬЗУЕТСЯ!!! вычисляем последний номер заявки клиента для отправки в Vue
        [HttpPost]
        public async Task<int> GetLTN([FromBody] JObject data)
        {
            var customer = (data["customer"]).ToString();
            int num = 0;

            var cs = GetConnString();
            using (var con = new SqlConnection(cs))
            {
                await con.OpenAsync();

                string queryString = $@"select max(cast(number as int)) from Ticket where customer_npp = @customer";
                var cmd = new SqlCommand(queryString, con);
                cmd.Parameters.AddWithValue("customer", customer);
                num = (int)await cmd.ExecuteScalarAsync();

            }

            return num;
        }

        //вычисляем последний номер заявки клиента
        private async Task<int> GetLTN1(string customer)
        {
            int num = 0;

            var cs = GetConnString();
            using (var con = new SqlConnection(cs))
            {
                await con.OpenAsync();

                string queryString = $@"select isnull(max(cast(number as int)), 0) from Ticket where customer_npp = @customer";
                var cmd = new SqlCommand(queryString, con);
                cmd.Parameters.AddWithValue("customer", customer);
                num = (int)await cmd.ExecuteScalarAsync();

            }

            return num;
        }

        //получаем сообщения заявки
        [HttpPost]
        public async Task<IActionResult> GetMessages([FromBody] JObject data)
        {
            var ticketNumber = (data["ticketNumber"]).ToString();
            var list = new List<Message>();

            var cs = GetConnString();
            using (var con = new SqlConnection(cs))
            {
                await con.OpenAsync();

                string queryString = $@"select t.npp, 
                                            t.number, 
                                            t.state_npp state,
                                            p.name priority,
                                            cast(Pl.NPP as varchar(10))                            agent_npp,
                                            pl.LAST_NAME + ' ' + pl.FIRST_NAME + ' ' + pl.MID_NAME agent, 
                                            m.message, 
                                            m.create_time,
                                            m.NPP                                                                                                     message_npp,
                                            coalesce((select name from COMPANYS where NPP = m.client_npp), '')                                        messageClient,
                                            coalesce((select LAST_NAME + ' ' + FIRST_NAME + ' ' + MID_NAME from PEOPLES where NPP = m.agent_npp), '') messageAgent,
                                            (select title from Ticket where npp = m.ticket_npp)                                                       ticketTitle,
                                            coalesce(cast(l.id as varchar(10)), '')                                                                   userId,
                                            coalesce(l.lastName + ' ' + l.firstName + ' ' + l.middleName, '')                                         userFullName,
                                            coalesce(m.attachments_data, '')                                                                          attachments_data,
                                            coalesce(m.emails, '')                                                                                    emails
                                    from Ticket t
                                        left join TicketMessages m on t.NPP = m.ticket_npp
                                        left join TicketPriority p on t.priority_npp = p.NPP
                                        left join PEOPLES Pl on t.agent_npp = Pl.NPP
                                        left join qbLoginUsers l on m.userId = l.id
                                        where t.npp = @ticketNumber 
                                        and (internal_note <> 1 or internal_note is null)
                                        order by m.create_time asc";
                var cmd = new SqlCommand(queryString, con);
                cmd.Parameters.AddWithValue("ticketNumber", ticketNumber);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        if (reader.HasRows)
                        {
                            var row = new Message
                            {
                                TicketNpp = reader.GetInt64(0),
                                TicketNumber = reader["number"] as string,
                                Priority = reader["priority"] as string,
                                AgentNpp = reader["agent_npp"] as string,
                                Agent = reader["agent"] as string,
                                State = reader["state"] as string,
                                Text = reader["message"] as string,
                                CreateTime = string.Format("{0:G}", reader.GetDateTime(7)),
                                MessageNpp = reader.GetInt64(8),
                                MessageAgent = reader["messageAgent"] as string,
                                TicketTitle = reader["ticketTitle"] as string,
                                MessageClient = reader["messageClient"] as string,
                                UserId = reader["userId"] as string,
                                UserFullName = reader["userFullName"] as string,
                                AttachmentsData = reader["attachments_data"] as string,
                                Emails = reader["emails"] as string
                            };
                            list.Add(row);
                        }
                    }
                }
            }

            return Json(list);
        }

        //сохраняем новое сообщение
        [HttpPost]
        public async Task<HttpResponseMessage> SaveMessage([FromBody] JObject data)
        {
            string ticketNpp;
            decimal messageNpp = 0;

            if (data == null)
                return new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, ReasonPhrase = "POST body is null" };

            var cs = GetConnString();

            var message = (data["newMessage"]).ToString();
            ticketNpp = (data["ticketNumber"]).ToString();
            var clientNpp = (data["clientNpp"]).ToString();
            var userId = (data["userId"]).ToString();
            var idapt = (data["idapt"]).ToString();
            var number = (data["number"]).ToString();
            var state = (data["state"]).ToString();
            var title = (data["title"]).ToString();
            var fromEmail = string.Empty;
            if (data.ContainsKey("fromEmail"))
            {
                fromEmail = (data["fromEmail"]).ToString();
            }
            var attachmentsData = string.Empty;
            if (data.ContainsKey("attachmentsData"))
            {
                attachmentsData = (data["attachmentsData"]).ToString();
            }
            var clientEmails = string.Empty;
            if (data.ContainsKey("clientEmails"))
            {
                clientEmails = (data["clientEmails"]).ToString();
            }

            //формируем список email клиентов для отправки писем
            //используется парсером писем
            //string To = fromEmail;
            //string Cc = string.Empty;
            //string Bcc = string.Empty;
            //if (clientEmails != string.Empty)
            //{
            //    try
            //    {
            //        JObject jsonObj = JObject.Parse(clientEmails);
            //        if (jsonObj.ContainsKey("to"))
            //        {
            //            JToken tokenTo = jsonObj.SelectToken("to");
            //            foreach (JToken item in tokenTo)
            //            {
            //                To += item.ToString().Trim() + ',';
            //            }
            //            To = To.TrimEnd(',');
            //        }
            //        if (jsonObj.ContainsKey("copy"))
            //        {
            //            JToken tokenCopy = jsonObj.SelectToken("copy");
            //            foreach (JToken item in tokenCopy)
            //            {
            //                Cc += item.ToString().Trim() + ',';
            //            }
            //            Cc = Cc.TrimEnd(',');
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        return new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, ReasonPhrase = e.Message.Replace(Environment.NewLine, "") };
            //    }
            //}

            using (var con = new SqlConnection(cs))
            {
                await con.OpenAsync();
                string queryString;

                if (state == "close")
                {
                    queryString = $@"update Ticket set state_npp = 'open' where NPP = @ticketNpp;
                                     insert into TicketMessages (ticket_npp, message, client_npp, userId, attachments_data, emails) 
                                        values(@ticketNpp, @message, @clientNpp, @userId, @attachments_data, @clientEmails);
                                        select SCOPE_IDENTITY() as messageNpp;";

                } else
                {
                    queryString = $@"insert into TicketMessages (ticket_npp, message, client_npp, userId, attachments_data, emails) values(@ticketNpp, @message, @clientNpp, @userId, @attachments_data, @clientEmails);
                                select SCOPE_IDENTITY() as messageNpp;";
                }

                var cmd = new SqlCommand(queryString, con);
                cmd.Parameters.AddWithValue("ticketNpp", ticketNpp);
                cmd.Parameters.AddWithValue("message", message);
                cmd.Parameters.AddWithValue("clientNpp", clientNpp);
                cmd.Parameters.AddWithValue("userId", userId);
                cmd.Parameters.AddWithValue("attachments_data", attachmentsData);
                cmd.Parameters.AddWithValue("clientEmails", clientEmails);

                try
                {
                    messageNpp = (decimal)await cmd.ExecuteScalarAsync();
                }
                catch (Exception e)
                {
                    return new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, ReasonPhrase = e.Message.Replace(Environment.NewLine, "") };
                }

                //добавляем к каждому элементу массива attachmentsData относительный путь расположения файла вложения
                //структура пути /npp/messageNpp
                if (attachmentsData != string.Empty)
                {
                    try
                    {
                        dynamic obj = JsonConvert.DeserializeObject(attachmentsData);
                        foreach (var attachment in obj)
                        {
                            var name = attachment["file_name"];
                            attachment["file_path"] = $"/{ticketNpp.ToString()}/{messageNpp.ToString()}/{name}";
                        }
                        var resultString = JsonConvert.SerializeObject(obj);

                        //записываем attachmentsData в таблицу TicketMessages
                        queryString = $@"update TicketMessages set attachments_data = @attachmentsData where NPP = @messageNpp;";
                        cmd = new SqlCommand(queryString, con);
                        cmd.Parameters.AddWithValue("attachmentsData", resultString);
                        cmd.Parameters.AddWithValue("messageNpp", messageNpp);
                        await cmd.ExecuteNonQueryAsync();
                    }
                    catch (Exception e)
                    {
                        return new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, ReasonPhrase = e.Message.Replace(Environment.NewLine, "") };
                    }
                }
            }

            //отправляем информационные письма
            string emailMessage = string.Empty;

            if (state == "close")
            {
                try
                {
                    string infSubject = CreateMessageTitle(new { ticketTitle = title, idapt, number, messageType = InformationMessageType.returnedToWork });
                    emailMessage = CreateMessageText(new { ticketNpp, title, message, idapt, number, server = serverUrl, messageType = InformationMessageType.returnedToWork });
                    await SendInformationMail(MailingAddresses.ReturnedToWork, infSubject, emailMessage);

                    //if (clientEmails != string.Empty)
                    //{
                    //    emailMessage = CreateMessageText(new { ticketNpp, title, message, idapt, number, server = serverUrlClient, messageType = InformationMessageType.returnedToWork });
                    //    await SendInformationMail(new string[] { fromEmail }, infSubject, emailMessage, "outer");
                    //    await SendMail(new { To, Cc, Bcc, subject = infSubject, message = emailMessage });
                    //}
                }
                catch (Exception)
                {
                    //выполнение метода не прекращаем, т.к. ошибка не критична для работы приложения, в дальнейшем нужно настроить логирование ошибки в файл.
                    return new HttpResponseMessage { StatusCode = HttpStatusCode.OK, ReasonPhrase = $"Message Saved;{messageNpp};Возникла ошибка при отправке информационного письма" };
                }
            }
            else
            {
                try
                {
                    string infSubject = CreateMessageTitle(new { ticketTitle = title, idapt, number, messageType = InformationMessageType.newMessage });
                    emailMessage = CreateMessageText(new { ticketNpp, title, message, idapt, number, server = serverUrl, messageType = InformationMessageType.newMessage });
                    await SendInformationMail(MailingAddresses.TicketCreated, infSubject, emailMessage);
                    //if (clientEmails != string.Empty)
                    //{
                    //    emailMessage = CreateMessageText(new { ticketNpp, title, message, idapt, number, server = serverUrlClient, messageType = InformationMessageType.newMessage });
                    //    await SendInformationMail(new string[] { fromEmail }, infSubject, emailMessage, "outer");
                    //    await SendMail(new { To, Cc, Bcc, subject = infSubject, message = emailMessage });
                    //}
                }
                catch (Exception)
                {
                    //выполнение метода не прекращаем, т.к. ошибка не критична для работы приложения, в дальнейшем нужно настроить логирование ошибки в файл.
                    return new HttpResponseMessage { StatusCode = HttpStatusCode.OK, ReasonPhrase = $"Message Saved;{messageNpp};Возникла ошибка при отправке информационного письма" };
                }
            }

            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK, ReasonPhrase = $"Message Saved;{messageNpp};Информационное письмо отправлено" };

        }

        //получаем список агентов (сотрудников Кверти)
        [HttpGet]
        public async Task<IActionResult> GetAgents()
        {
            var list = new List<Agent>();

            var cs = GetConnString();

            using (var con = new SqlConnection(cs))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"select cast(p.NPP as varchar(10)) npp, 
                                                isnull(p.LAST_NAME, '') + ' ' + isnull(p.FIRST_NAME, '') + ' ' + isnull(p.MID_NAME, '') name
                                        from PEOPLES p
                                        order by p.LAST_NAME;";

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new Agent
                            {
                                Id = reader["npp"] as string,
                                Name = reader["name"] as string
                            };

                            list.Add(row);
                        }
                    }
                }
            }

            return Json(list);
        }

        //получаем информацию о клиенте (аптеке)
        [HttpGet]
        public async Task<IActionResult> GetClient()
        {
            var cs = GetConnString();
            var list = new List<Client>();

            var q = HttpContext.Request.Query;
            q.TryGetValue("apteka_id", out StringValues apteka_id);


            using (var con = new SqlConnection(cs))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"select npp,
                                                idapt,
                                                coalesce(name, '') name,
                                                coalesce(ADDR, '') addr,
                                                coalesce(Director, '') director,
                                                coalesce(FIO_ZAV, '') fio_zav,
                                                coalesce(E_mail, '') email
                                            from COMPANYS where IDAPT = @apteka_id;";
                    cmd.Parameters.AddWithValue("apteka_id", apteka_id.ToString());

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new Client
                            {
                                Npp = (int)reader.GetSqlDecimal(0),
                                IDAPT = reader.GetInt32(1),
                                Name = reader["name"] as string,
                                ADDR = reader["addr"] as string,
                                Director = reader["director"] as string,
                                FioZav = reader["fio_zav"] as string,
                                Email = reader["email"] as string
                            };

                            list.Add(row);
                        }
                    }
                }
            }

            return Json(list);
        }

        // НЕ ИСПОЛЬЗУЕТСЯ! получаем список клиентов (аптек) планировалось использовать для выпадающего списка в фильтре выбора аптеки
        [HttpGet]
        public async Task<IActionResult> GetClients()
        {
            //var list = new List<Client>();
            var list = new List<string>();

            var cs = GetConnString();

            using (var con = new SqlConnection(cs))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    //cmd.CommandText = @"select TOP 50 NPP npp, coalesce(NAME, '') name, coalesce(ADDR, '') addr, coalesce(IDAPT, '') adapt from COMPANYS where npp > 1000;";
                    cmd.CommandText = @"select coalesce(NAME, 'нет наименования') name from COMPANYS where NPP > 4000 order by name;";

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        //while (await reader.ReadAsync())
                        //{
                        //    var row = new Client
                        //    {
                        //        Npp = (int)reader.GetSqlDecimal(0),
                        //        Name = reader["name"] as string,
                        //        ADDR = reader["addr"] as string,
                        //        IDAPT = reader.GetInt32(3)
                        //    };

                        //    list.Add(row);
                        //}
                        while (await reader.ReadAsync())
                        {
                            list.Add(reader.GetString(0));
                        }

                    }
                }
            }

            return Json(list);
        }

        //установка приоритета заявки
        [HttpPost]
        public async Task<HttpResponseMessage> SetPriority()
        {
            string cs = GetConnString();
            string npp = Request.Form["npp"];
            string priority = Request.Form["priority"];

            using (var con = new SqlConnection(cs))
            {
                await con.OpenAsync();
                string queryString = @"UPDATE Ticket SET priority_npp = @priority where npp = @npp;";
                SqlCommand cmd = new SqlCommand(queryString, con);
                cmd.Parameters.AddWithValue("npp", npp);
                cmd.Parameters.AddWithValue("priority", priority);

                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception e)
                {
                    return new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, ReasonPhrase = e.Message.Replace(Environment.NewLine, "") };
                }
            }

            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK, ReasonPhrase = "Priority changed successfully" };
        }

        //установка статуса заявки
        [HttpPost]
        public async Task<HttpResponseMessage> SetState()
        {
            string cs = GetConnString();
            string npp = Request.Form["npp"];
            string state = Request.Form["state"];
            string agentNpp = Request.Form["agentNpp"];

            using (var con = new SqlConnection(cs))
            {
                await con.OpenAsync();
                string queryString = @"UPDATE Ticket SET state_npp = @state, agent_npp = @agentNpp where npp = @npp;";
                SqlCommand cmd = new SqlCommand(queryString, con);
                cmd.Parameters.AddWithValue("npp", npp);
                cmd.Parameters.AddWithValue("state", state);
                cmd.Parameters.AddWithValue("agentNpp", agentNpp);

                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception e)
                {
                    return new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, ReasonPhrase = e.Message.Replace(Environment.NewLine, "") };
                }
            }

            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK, ReasonPhrase = "State changed successfully" };
        }

        //установка исполнителя заявки
        [HttpPost]
        public async Task<HttpResponseMessage> SetAgent()
        {
            string cs = GetConnString();
            string npp = Request.Form["npp"];
            string agentNpp = Request.Form["agentNpp"];

            using (var con = new SqlConnection(cs))
            {
                await con.OpenAsync();
                string queryString = @"UPDATE Ticket SET agent_npp = @agentNpp where npp = @npp;";
                SqlCommand cmd = new SqlCommand(queryString, con);
                cmd.Parameters.AddWithValue("npp", npp);
                cmd.Parameters.AddWithValue("agentNpp", agentNpp);

                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception e)
                {
                    return new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, ReasonPhrase = e.Message.Replace(Environment.NewLine, "") };
                }
            }

            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK, ReasonPhrase = "Agent changed successfully" };
        }

        //изменение Темы заявки
        [HttpPost]
        public async Task<HttpResponseMessage> SetNewTicketTitle()
        {
            string cs = GetConnString();
            string ticketNpp = Request.Form["ticketNpp"];
            string newTitle = Request.Form["newTitle"];

            using (var con = new SqlConnection(cs))
            {
                await con.OpenAsync();
                string queryString = @"UPDATE Ticket SET title = @newTitle WHERE NPP = @ticketNpp;";
                SqlCommand cmd = new SqlCommand(queryString, con);
                cmd.Parameters.AddWithValue("ticketNpp", ticketNpp);
                cmd.Parameters.AddWithValue("newTitle", newTitle);

                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception e)
                {
                    return new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, ReasonPhrase = e.Message.Replace(Environment.NewLine, "") };
                }
            }

            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK, ReasonPhrase = "Title changed successfully" };
        }

        //получаем с сервера время последних изменений в заявке
        [HttpGet]
        public IActionResult GetLastUpdateTimeFromServer()
        {
            var cs = GetConnString();
            var q = HttpContext.Request.Query;
            q.TryGetValue("clientNpp", out StringValues clientNpp);
            q.TryGetValue("userId", out StringValues userId);
            DateTime time;

            using (var con = new SqlConnection(cs))
            {
                con.Open();

                string queryString = @"select last_update_time from HistoryClientTickets where client_npp = @clientNpp and userId = @userId;";

                var cmd = new SqlCommand(queryString, con);
                cmd.Parameters.AddWithValue("clientNpp", clientNpp.ToString());
                cmd.Parameters.AddWithValue("userId", userId.ToString());
                var result = cmd.ExecuteScalar();
                if (result != null)
                {
                    time = (DateTime)result;
                    return Ok(time.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
                }
                return Ok(string.Empty);
            }
        }

        //НЕ ИСПОЛЬЗУЕТСЯ!!! получаем с сервера из таблицы HistoryMessages время последнего просмотра заявки клиентом
        [HttpGet]
        public IActionResult GetTicketLastUpdateTime()
        {
            var cs = GetConnString();
            var q = HttpContext.Request.Query;
            q.TryGetValue("ticketNpp", out StringValues ticketNpp);
            q.TryGetValue("clientNpp", out StringValues clientNpp);
            DateTime time;

            using (var con = new SqlConnection(cs))
            {
                con.Open();

                string queryString = @"select seen_time from HistoryMessages where ticket_npp = @ticketNpp and client_npp = @clientNpp;";
                var cmd = new SqlCommand(queryString, con);
                cmd.Parameters.AddWithValue("ticketNpp", ticketNpp.ToString());
                cmd.Parameters.AddWithValue("clientNpp", clientNpp.ToString());
                var result = cmd.ExecuteScalar();
                if (result != null)
                {
                    time = (DateTime)result;
                    return Ok(time.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
                }
                return Ok(string.Empty);
            }
        }

        //получаем с сервера время последних загруженных данных в браузере клиента
        [HttpGet]
        public IActionResult GetLastUpdateTimeOnClient()
        {
            var cs = GetConnString();
            var q = HttpContext.Request.Query;
            q.TryGetValue("clientNpp", out StringValues clientNpp);
            q.TryGetValue("userId", out StringValues userId);
            DateTime time;

            using (var con = new SqlConnection(cs))
            {
                con.Open();

                string queryString = @"select last_update_time_on_client from HistoryClientTickets where client_npp = @clientNpp and userId = @userId;";

                var cmd = new SqlCommand(queryString, con);
                cmd.Parameters.AddWithValue("clientNpp", clientNpp.ToString());
                cmd.Parameters.AddWithValue("userId", userId.ToString());
                var result = cmd.ExecuteScalar();
                if (result != null)
                {
                    time = (DateTime)result;
                    return Ok(time.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
                }
                return Ok(string.Empty);
            }
        }

        //устанавливаем время последних загруженных данных в браузере клиента
        [HttpPut]
        public HttpResponseMessage SetLastUpdateTimeOnClient()
        {
            string cs = GetConnString();
            string clientNpp = Request.Form["clientNpp"];
            string userId = Request.Form["userId"];

            using (var con = new SqlConnection(cs))
            {
                con.Open();

                string queryString = @"update HistoryClientTickets set last_update_time_on_client = current_timestamp where client_npp = @clientNpp and userId = @userId;";

                SqlCommand cmd = new SqlCommand(queryString, con);
                cmd.Parameters.AddWithValue("clientNpp", clientNpp);
                cmd.Parameters.AddWithValue("userId", userId);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    return new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, ReasonPhrase = e.Message.Replace(Environment.NewLine, "") };
                }
            }

            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK, ReasonPhrase = "Last_update_time_on_client update successfully" };
        }

        //НЕ ИСПОЛЬЗУЕТСЯ!!! устанавливаем для сообщения флаг прочитано
        [HttpPut]
        public HttpResponseMessage SetIsSeen()
        {
            string cs = GetConnString();
            string messageNpp = Request.Form["messageNpp"];

            using (var con = new SqlConnection(cs))
            {
                con.Open();
                string queryString = @"UPDATE TicketMessages SET [isSeen] = 1 WHERE [NPP] = @messageNpp;";
                SqlCommand cmd = new SqlCommand(queryString, con);
                cmd.Parameters.AddWithValue("messageNpp", messageNpp);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    return new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, ReasonPhrase = e.Message.Replace(Environment.NewLine, "") };
                }
            }

            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK, ReasonPhrase = "isSeen set successfully" };
        }

        //устанавливаем время последнего просмотра заявки клиентом (клик по теме или по вкладке с открытой заявкой) 
        [HttpPut]
        public HttpResponseMessage SetTicketLastUpdateTime()
        {
            string cs = GetConnString();
            string ticketNpp = Request.Form["ticketNpp"];
            string clientNpp = Request.Form["clientNpp"];
            string userId = Request.Form["userId"];

            using (var con = new SqlConnection(cs))
            {
                con.Open();
                string queryString = @"begin
                                          if (select seen_time from HistoryMessages where ticket_npp = @ticketNpp
                                                                                      and client_npp = @clientNpp and userId = @userId) is null
                                            begin
                                              insert into HistoryMessages (max_message_npp, ticket_npp, client_npp, userId) values ((select max(NPP) from TicketMessages where ticket_npp = @ticketNpp), @ticketNpp,  @clientNpp, @userId)
                                            end
                                          else
                                            begin
                                              update HistoryMessages
                                              set max_message_npp = (select max(NPP) from TicketMessages where ticket_npp = @ticketNpp),
                                                  seen_time       = current_timestamp
                                              where ticket_npp = @ticketNpp
                                                and client_npp = @clientNpp
                                                and userId = @userId
                                            end
                                        end";

                SqlCommand cmd = new SqlCommand(queryString, con);
                cmd.Parameters.AddWithValue("ticketNpp", ticketNpp);
                cmd.Parameters.AddWithValue("clientNpp", clientNpp);
                cmd.Parameters.AddWithValue("userId", userId);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    return new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, ReasonPhrase = e.Message.Replace(Environment.NewLine, "") };
                }
            }

            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK, ReasonPhrase = "Seen time update successfully" };
        }

        //сохраняем время когда клиент последний раз запрашивал список заявок, используется для проставления признака наличия изменений в заявках
        [HttpPut]
        public HttpResponseMessage SetLastUpdateTimeFromServer()
        {
            string cs = GetConnString();
            string clientNpp = Request.Form["clientNpp"];
            string userId = Request.Form["userId"];

            using (var con = new SqlConnection(cs))
            {
                con.Open();
                string queryString = @"  IF (select last_update_time
                                              from HistoryClientTickets
                                              where client_npp = @clientNpp and userId = @userId) is null
                                            begin
                                              INSERT INTO HistoryClientTickets (client_npp, last_update_time, userId) VALUES (cast(@clientNpp as numeric(9)), current_timestamp, @userId)
                                            end
                                          ELSE
                                            begin
                                              update HistoryClientTickets SET last_update_time = current_timestamp where client_npp = @clientNpp and userId = @userId
                                            end;";
                SqlCommand cmd = new SqlCommand(queryString, con);
                cmd.Parameters.AddWithValue("clientNpp", clientNpp);
                cmd.Parameters.AddWithValue("userId", userId);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    return new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, ReasonPhrase = e.Message.Replace(Environment.NewLine, "") };
                }
            }

            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK, ReasonPhrase = "Last time set successfully" };
        }

        //НЕ ИСПОЛЬЗУЕТСЯ!!! получаем список всех заявок клиента из таблицы HistoryClientMessages, используется для проверки наличия изменений
        [HttpGet]
        public IActionResult GetClientsTicketsFromHistoryClientMessages()
        {
            var cs = GetConnString();
            var list = new List<TicketFromHistory>();
            var q = HttpContext.Request.Query;
            q.TryGetValue("client_npp", out StringValues client_npp);


            using (var con = new SqlConnection(cs))
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"select t.ticket_npp, t.last_update_time from HistoryClientMessages t where t.client_npp = @client_npp;";

                    cmd.Parameters.AddWithValue("client_npp", client_npp.ToString());

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = new TicketFromHistory
                            {
                                TicketNpp = reader.GetInt64(0),
                                LastUpdateTime = reader.GetDateTime(1)
                            };

                            list.Add(row);
                        }
                    }
                }
            }

            return Json(list);
        }

        //получаем список новых сообщений заявки
        [HttpGet]
        public async Task<IActionResult> GetNewMessagesOfTickets()
        {
            var cs = GetConnString();
            var list = new List<NewMessage>();
            var q = HttpContext.Request.Query;
            q.TryGetValue("client_npp", out StringValues client_npp);
            q.TryGetValue("ticket_npp", out StringValues ticket_npp);
            q.TryGetValue("userId", out StringValues userId);

            using (var con = new SqlConnection(cs))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"begin
                                          declare @seenTime datetime;
                                          set @seenTime = (select h.seen_time from HistoryMessages h where ticket_npp = @ticket_npp
                                                                                                       and client_npp = @client_npp and userId = @userId);
                                          select m.NPP from TicketMessages m where m.ticket_npp = @ticket_npp
                                                                           and (m.create_time > @seenTime or @seenTime is null) 
                                                                           and (internal_note <> 1 or internal_note is null);
                                        END;";

                    cmd.Parameters.AddWithValue("client_npp", client_npp.ToString());
                    cmd.Parameters.AddWithValue("ticket_npp", ticket_npp.ToString());
                    cmd.Parameters.AddWithValue("userId", userId.ToString());

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new NewMessage
                            {
                                Npp = reader.GetInt64(0)
                            };

                            list.Add(row);
                        }
                    }
                }
            }

            return Json(list);
        }

        //получаем список сообщений заявки просмотренных агентами
        [HttpGet]
        public async Task<IActionResult> GetSeenedByAgentsMessagesOfTickets()
        {
            var cs = GetConnString();
            var list = new List<NewMessage>();
            var q = HttpContext.Request.Query;
            q.TryGetValue("ticket_npp", out StringValues ticket_npp);

            using (var con = new SqlConnection(cs))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"begin
                                          declare @seenTime datetime;
                                          set @seenTime = (select max(h.seen_time)
                                                           from HistoryMessages h
                                                           where ticket_npp = @ticket_npp
                                                             and agent_npp is not null);
                                          select m.NPP
                                          from TicketMessages m
                                          where m.ticket_npp = @ticket_npp
                                            and m.create_time < @seenTime;
                                        end";

                    cmd.Parameters.AddWithValue("ticket_npp", ticket_npp.ToString());

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new NewMessage
                            {
                                Npp = reader.GetInt64(0)
                            };

                            list.Add(row);
                        }
                    }
                }
            }

            return Json(list);
        }

        //получаем заявки в которых участвует пользователь (т.е. заявки в которых есть сообщение от пользователя)
        [HttpGet]
        public IActionResult GeTicketsWithUserMessages()
        {
            var list = new List<Ticket>();

            var cs = GetConnString();
            var q = HttpContext.Request.Query;
            q.TryGetValue("apteka_npp", out StringValues apteka_npp);
            q.TryGetValue("userId", out StringValues userId);

            using (var con = new SqlConnection(cs))
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    //cmd.CommandText = @"with res as (select m.ticket_npp, (select top 1 ms.client_npp from TicketMessages ms where ticket_npp = m.ticket_npp order by create_time desc) sender
                    //                                    from TicketMessages m
                    //                                    where m.client_npp = @apteka_npp and m.userId = @userId)
                    //                    select distinct res.ticket_npp from res
                    //                    where res.sender is null;";

                    cmd.CommandText = @"with res as (select distinct m.ticket_npp
                                                         from TicketMessages m
                                                         where m.client_npp = @apteka_npp
                                                           and m.userId = @userId)
                                            select res.ticket_npp
                                            from res
                                                   left join HistoryMessages h on res.ticket_npp = h.ticket_npp
                                            where h.client_npp = @apteka_npp
                                              and h.userId = @userId
                                              and (select max(create_time) from TicketMessages tm where tm.ticket_npp = res.ticket_npp and (tm.internal_note <> 1 or tm.internal_note is null)) > h.seen_time;";

                    cmd.Parameters.AddWithValue("apteka_npp", apteka_npp.ToString());
                    cmd.Parameters.AddWithValue("userId", userId.ToString());

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = new Ticket
                            {
                                Npp = reader.GetInt64(0),
                            };

                            list.Add(row);
                        }
                    }
                }
            }

            return Json(list);
        }

        //Сохраняем нового пользователя аптеки
        [HttpPut]
        public HttpResponseMessage SaveUser()
        {
            string cs = GetConnString();
            string aptekaId = Request.Form["aptekaId"];
            string userId = Request.Form["userId"];
            string firstName = Request.Form["firstName"];
            string lastName = Request.Form["lastName"];
            string middleName = Request.Form["middleName"];
            string email = Request.Form["email"];
            string password = Request.Form["password"];
            string isSendByEmail = Request.Form["isSendByEmail"];
            string isSendBySystemOrder = Request.Form["isSendBySystemOrder"];

            using (var con = new SqlConnection(cs))
            {
                con.Open();
                string queryString;
                if (userId == "0")
                {
                    queryString = @"begin
                                          declare @idaptInTable int;
                                          declare @emailInTable varchar(50);

                                          -- делаем запись для аптеки
                                          set @idaptInTable = (select idapt from qbLoginUsers where idapt = @apteka_id
                                                                                                and firstName is null);
                                          if @idaptInTable is null
                                            begin
                                              insert into qbLoginUsers (idapt, password) values (@apteka_id, reverse(@apteka_id));
                                            end

                                          -- делаем запись пользователя этой аптеки
                                          set @emailInTable = (select email from qbLoginUsers where idapt = @apteka_id
                                                                                                and email = @email);
                                          if @emailInTable is null
                                            begin
                                              insert into qbLoginUsers (idapt, firstName, middleName, lastName, email, password, sendByEmail, sendBySystemOrder)
                                              values (@apteka_id, @firstName, @middleName, @lastName, @email, @password, @sendByEmail, @sendBySystemOrder);
                                            end
                                          else
                                              begin
                                                RAISERROR('Указанный email уже существует', 16,16)
                                              end
                                        end;";
                }
                else
                {
                    queryString = @"update qbLoginUsers
                                        set firstName         = @firstName,
                                            middleName        = @middleName,
                                            lastName          = @lastName,
                                            password          = @password,
                                            sendByEmail       = @sendByEmail,
                                            sendBySystemOrder = @sendBySystemOrder
                                        where id = @userId;";
                }

                SqlCommand cmd = new SqlCommand(queryString, con);
                cmd.Parameters.AddWithValue("apteka_id", aptekaId);
                cmd.Parameters.AddWithValue("userId", userId);
                cmd.Parameters.AddWithValue("firstName", firstName);
                cmd.Parameters.AddWithValue("lastName", lastName);
                cmd.Parameters.AddWithValue("middleName", middleName);
                cmd.Parameters.AddWithValue("email", email);
                cmd.Parameters.AddWithValue("password", password);
                cmd.Parameters.AddWithValue("sendByEmail", isSendByEmail);
                cmd.Parameters.AddWithValue("sendBySystemOrder", isSendBySystemOrder);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    return new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, ReasonPhrase = e.Message.Replace(Environment.NewLine, "") };
                }
            }

            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK, ReasonPhrase = "Information set successfully" };
        }

        //получаем список пользователей сайта аптеки
        [HttpGet]
        public async Task<IActionResult> GetSiteUsers()
        {
            var cs = GetConnString();
            var list = new List<SiteUser>();
            var q = HttpContext.Request.Query;
            q.TryGetValue("aptekaId", out StringValues aptekaId);

            using (var con = new SqlConnection(cs))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"select lastName + ' ' + firstName + ' ' + middleName fullname, email, SendByEmail, SendBySystemOrder
                                            from qbLoginUsers
                                            where idapt = @aptekaId
                                              and firstName is not null;";

                    cmd.Parameters.AddWithValue("aptekaId", aptekaId.ToString());

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new SiteUser
                            {
                                FullName = reader["fullname"] as string,
                                Email = reader["email"] as string,
                                SendByMail = reader.GetBoolean(2),
                                SendBySystemOrder = reader.GetBoolean(3)
                            };

                            list.Add(row);
                        };

                    }
                }
            }
            return Json(list);
        }

        //получаем информацию о пользователе сайта из таблицы qbLoginUsers, используется при аутентификации по email пользователя
        [HttpGet]
        public async Task<IActionResult> GetSiteUser()
        {
            var cs = GetConnString();
            var list = new List<SiteUser>();
            var q = HttpContext.Request.Query;
            //q.TryGetValue("aptekaId", out StringValues aptekaId);
            q.TryGetValue("email", out StringValues email);
            q.TryGetValue("ps", out StringValues password);

            using (var con = new SqlConnection(cs))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"select lastName + ' ' + firstName + ' ' + middleName fullname, email, SendByEmail, SendBySystemOrder, firstName, lastName, middleName, idapt, id
                                            from qbLoginUsers
                                            where email = @email
                                              and password = @password;";

                    //cmd.Parameters.AddWithValue("aptekaId", aptekaId.ToString());
                    cmd.Parameters.AddWithValue("email", email.ToString());
                    cmd.Parameters.AddWithValue("password", password.ToString());

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new SiteUser
                            {
                                FullName = reader["fullname"] as string,
                                Email = reader["email"] as string,
                                SendByMail = reader.GetBoolean(2),
                                SendBySystemOrder = reader.GetBoolean(3),
                                FirstName = reader["firstName"] as string,
                                LastName = reader["lastName"] as string,
                                MiddleName = reader["middleName"] as string,
                                AptekaId = reader.GetInt32(7),
                                UserId = reader.GetInt64(8)
                            };

                            list.Add(row);
                        };

                    }
                }
            }
            return Json(list);
        }

        //получаем информацию об аптеке из таблицы qbLoginUsers, используется при аутентификации по id аптеки
        [HttpGet]
        public async Task<IActionResult> GetApteka()
        {

            var cs = GetConnString();
            int result;
            var q = HttpContext.Request.Query;
            q.TryGetValue("aptekaId", out StringValues aptekaId);
            q.TryGetValue("ps", out StringValues password);

            using (var con = new SqlConnection(cs))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"begin
                                          declare @result int;
                                          set @result = (select id from qbLoginUsers where idapt = @aptekaId
                                                                                       and firstName is null
                                                                                       and password = @password);
                                          if @result is null
                                            select 0;
                                          else 
                                            select cast(@result as int);
                                        end;";

                    cmd.Parameters.AddWithValue("aptekaId", aptekaId.ToString());
                    cmd.Parameters.AddWithValue("password", password.ToString());

                    try
                    {
                        result = (int)await cmd.ExecuteScalarAsync();
                    }
                    catch (Exception e)
                    {
                        return BadRequest(e.Message);
                    }
                }
            }

            return Ok(result);
        }

        //загружаем файл вложения в браузер
        [HttpGet]
        public IActionResult GetAttachmentFile()
        {
            var q = HttpContext.Request.Query;
            q.TryGetValue("file_path", out StringValues filePath);

            //string file_path = attachmentsPath + Iconv(filePath.ToString());
            string file_path = attachmentsPath + filePath.ToString();
            string file_type = "application/octet-stream";

            return PhysicalFile(file_path, file_type);
        }

        //переводим имя файла на кирилице из кодировки utf-8 в кодировку windows-1251, нужно только если вложения будут храниться на диске ОС семейства Windows
        private string Iconv(string str)
        {

            Encoding utf8 = Encoding.GetEncoding("UTF-8");
            Encoding win1251 = Encoding.GetEncoding("Windows-1251");

            byte[] utf8Bytes = utf8.GetBytes(str);
            //byte[] win1251Bytes = Encoding.Convert(utf8, win1251, utf8Bytes);

            var res = win1251.GetString(utf8Bytes);

            return res;
        }

        [HttpPost, DisableRequestSizeLimit]
        public async Task<IActionResult> UploadAttachmentsFiles(IList<IFormFile> files, string ticketNpp, string messageNpp)
        {
            string fileName = string.Empty;
            string filesNames = string.Empty;
            if (files.Count < 1)
            {
                return Ok("В заявке нет вложений");
            }
            try
            {
                //string filePath = Path.Combine(attachmentsPath, ticketNpp, messageNpp);
                string filePath = $"{attachmentsPath}/{ticketNpp}/{messageNpp}";
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                foreach (IFormFile file in files)
                {
                    fileName = file.FileName;
                    filesNames += fileName + ", ";
                    using (FileStream output = System.IO.File.Create(Path.Combine(filePath, fileName)))
                        await file.CopyToAsync(output);
                }

                return Ok($"Files {filesNames} successfully upload.");

            }
            catch (Exception e)
            {
                return BadRequest($"Files {filesNames} upload failed: " + e.Message.Replace(Environment.NewLine, ""));
            }
        }

        //отправка информационного сообщения
        private async Task SendInformationMail(string[] emails, string subject, string message, string type = "inner")
        {
            var emailMessage = new MimeMessage();
            var listBcc = new InternetAddressList();
            var listTo = new InternetAddressList();

#if TEST_DATABASE
            emailMessage.From.Add(new MailboxAddress("Кверти+ Помощь", "helptest@qwerty.plus"));
#else
            emailMessage.From.Add(new MailboxAddress("Кверти+ Помощь", "help@qwerty.plus"));
#endif

            foreach (var email in emails)
            {
                if (type == "outer")
                {
                    listTo.Add(new MailboxAddress(email));
                }
                else
                {
                    listBcc.Add(new MailboxAddress(email));
                }
            }

            if (type == "outer")
            {
                emailMessage.To.AddRange(listTo);
            }
            else
            {
                emailMessage.Bcc.AddRange(listBcc);
            }

            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("Plain")
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("mail.qwerty.plus", 25, SecureSocketOptions.None);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }

        private async Task<IActionResult> SendMail(dynamic data)
        {
            string To = data.To;
            string Cc = data.Cc;
            string Bcc = data.Bcc;
            //добавляем в скрытую копию адреса ящиков Кверти
            if (Bcc == string.Empty)
            {
                Bcc = InfMails;
            }
            else
            {
                Bcc = Bcc + "," + InfMails;
            }
            string subject = data.subject;
            string message = data.message;

            //если не передано ни одного email, то прекращаем работу метода
            if ($"{To}{Cc}{Bcc}".Trim() == string.Empty)
            {
                return Ok("Отправка писем клиентам не требуется, т.к. не указано ни одного email");
            }

            dynamic emailData = new { To, Cc, Bcc, subject, message };

            try
            {
                await SendEmailAsync(emailData);
            }
            catch (Exception e)
            {
                return BadRequest($"Произошла ошибка при отправке писем клиентам: " + e.Message.Replace(Environment.NewLine, ""));
            }

            return Ok("Письма отправлены");
        }

        private async Task SendEmailAsync(dynamic emailData)
        {
            var emailMessage = new MimeMessage();
#if TEST_DATABASE
            emailMessage.From.Add(new MailboxAddress("Кверти+ Помощь", "helptest@qwerty.plus"));
#else
            emailMessage.From.Add(new MailboxAddress("Кверти+ Помощь", "help@qwerty.plus"));
#endif


            if (emailData.To != string.Empty)
            {
                var listTo = new InternetAddressList();
                var emailsInTo = (emailData.To).Split(",");
                if (emailsInTo.Length > 0)
                {
                    foreach (string email in emailsInTo)
                    {
                        listTo.Add(new MailboxAddress(email.Trim()));
                    }
                    emailMessage.To.AddRange(listTo);
                }
            }

            if (emailData.Cc != string.Empty)
            {
                var listCc = new InternetAddressList();
                var emailsInCc = (emailData.Cc).Split(",");
                if (emailsInCc.Length > 0)
                {
                    foreach (string email in emailsInCc)
                    {
                        listCc.Add(new MailboxAddress(email.Trim()));
                    }
                    emailMessage.Cc.AddRange(listCc);
                }

            }

            if (emailData.Bcc != string.Empty)
            {
                var listBcc = new InternetAddressList();
                var emailsInBcc = (emailData.Bcc).Split(',');
                if (emailsInBcc.Length > 0)
                {
                    foreach (string email in emailsInBcc)
                    {
                        listBcc.Add(new MailboxAddress(email.Trim()));
                    }
                    emailMessage.Bcc.AddRange(listBcc);
                }

            }

            emailMessage.Subject = emailData.subject;

            var builder = new BodyBuilder();
            builder.TextBody = emailData.message;
            emailMessage.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("mail.qwerty.plus", 25, SecureSocketOptions.None);
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }

        //определяем какая база используется (тестовая или рабочая) для отображения версии на клиенте
        [HttpGet]
        public string GetDataBaseVersion()
        {
#if TEST_DATABASE
            return "test";
#else

            return "production";
#endif
        }

        //формируем текст информационного сообщения
        private string CreateMessageText(dynamic data)
        {
            var emailMessage = string.Empty;

            switch (data.messageType)
            {
                case "newMessage":

                    emailMessage = "Здравствуйте," + Environment.NewLine;
                    emailMessage += $"В заявке №{data.idapt}-{data.number}" + Environment.NewLine;
                    emailMessage += $"Тема: {data.title}" + Environment.NewLine;
                    emailMessage += $"Добавлено сообщение:" + Environment.NewLine;
                    emailMessage += data.message + Environment.NewLine + Environment.NewLine;
                    //emailMessage += $"{data.server}/support/messages?tn={data.ticketNpp}{Environment.NewLine}{Environment.NewLine}";
                    //emailMessage += new string('-', 50) + Environment.NewLine;
                    //emailMessage += "Кверти+ Уведомление";

                    return emailMessage;

                case "newTicket":

                    emailMessage = "Здравствуйте," + Environment.NewLine;
                    emailMessage += $"Создана заявка №{data.idapt}-{data.number}" + Environment.NewLine;
                    emailMessage += $"Тема: {data.title}" + Environment.NewLine;
                    emailMessage += $"Текст сообщения:" + Environment.NewLine;
                    emailMessage += data.message + Environment.NewLine + Environment.NewLine;
                    //emailMessage += $"{data.server}/support/messages?tn={data.ticketNpp}{Environment.NewLine}{Environment.NewLine}";
                    //emailMessage += new string('-', 50) + Environment.NewLine;
                    //emailMessage += "Кверти+ Уведомление";

                    return emailMessage;

                case "returnedToWork":

                    emailMessage = "Здравствуйте," + Environment.NewLine;
                    emailMessage += $"Заявка №{data.idapt}-{data.number} возвращена в работу" + Environment.NewLine;
                    emailMessage += $"Тема: {data.title}" + Environment.NewLine;
                    emailMessage += $"Текст сообщения:" + Environment.NewLine;
                    emailMessage += data.message + Environment.NewLine + Environment.NewLine;

                    return emailMessage;

                default:
                    return emailMessage;
            }
        }

        //формируем текст темы сообщения
        private string CreateMessageTitle(dynamic data)
        {
            string title = string.Empty;

            switch (data.messageType)
            {
                case "newMessage":
                    title = $"[Заявка №{data.idapt}-{data.number}] ответ клиента - {data.ticketTitle}";
                    return title;

                case "newTicket":
                    title = $"[Заявка №{data.idapt}-{data.number}] создана - {data.ticketTitle}";
                    return title;

                case "returnedToWork":
                    title = $"[Заявка №{data.idapt}-{data.number}] возвращена в работу - {data.ticketTitle}";
                    return title;

                default:
                    return title;
            }
        }
    }
}
