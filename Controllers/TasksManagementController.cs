using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using SHERIA.Helpers;
using SHERIA.Models;
using static SHERIA.Helpers.CryptoHelper;
using System.Collections;
using System.Data;
using static SHERIA.Controllers.ClientManagementController;
//using static SHERIA.Controllers.MattersController;

namespace SHERIA.Controllers
{
    public class TasksManagementController : Controller
    {
        private IWebHostEnvironment ihostingenvironment;
        private ILoggerManager iloggermanager;
        private DBHandler dbhandler;

        public TasksManagementController(ILoggerManager logger, IWebHostEnvironment environment, DBHandler mydbhandler)
        {
            iloggermanager = logger;
            ihostingenvironment = environment;
            dbhandler = mydbhandler;
        }

        public class tasks_record
        {
            public Int64 id { set; get; }
            public string? task_name { set; get; }
            public Int64 matter_id { set; get; }
            public DateTime start_date { set; get; }
            public DateTime due_date { set; get; }
            public string? task_status { set; get; }
            public Int64 assigned_to { set; get; }
            public string? priority { set; get; }
            public string? description { set; get; }
        }

        public IActionResult TaskRecord()
        {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("AdminLogin", "AppAuth");
            else
            {
                ViewBag.MenuLayout = HttpContext.Session.GetString("menulayout");
                MenuHandler menuhandler = new MenuHandler(dbhandler);
                IEnumerable<MenuModel> menu = menuhandler.GetMenu(Convert.ToInt16(HttpContext.Session.GetString("profileid")), HttpContext.Request.Path);
                return View(menu);
            }
        }
        
        public IActionResult TaskCalender()
        {
            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("AdminLogin", "AppAuth");
            else
            {
                ViewBag.MenuLayout = HttpContext.Session.GetString("menulayout");
                MenuHandler menuhandler = new MenuHandler(dbhandler);
                IEnumerable<MenuModel> menu = menuhandler.GetMenu(Convert.ToInt16(HttpContext.Session.GetString("profileid")), HttpContext.Request.Path);
                return View(menu);
            }
        }


        [HttpPost]
        public ActionResult CreateTasks(tasks_record record)
        {
            processingresponse response = new processingresponse
            {
                system_ref = DateTime.Now.ToString("yyyyMMddHHmmssfff")
            };

            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("AdminLogin", "AppAuth");
            else
            {
                if (record.task_name == null)
                    return Content("Invalid value");

                if (record.task_status == null)
                    return Content("Invalid value");

                try
                {
                    TasksModel existingrecord = dbhandler.GetTasksRecord().Find(mymodel => mymodel.id == record.id)!;
                    if (existingrecord != null)
                    {
                        TasksModel mymodel = new TasksModel
                        {
                            id = existingrecord.id,
                            task_name = record.task_name,
                            matter_id = record.matter_id,
                            start_date = record.start_date,
                            due_date = record.due_date,
                            task_status = record.task_status,
                            assigned_to = record.assigned_to,
                            priority = record.priority,
                            description = record.description,
                        };

                        if (dbhandler.UpdateTasksRecord(mymodel))
                        {
                            CaptureAuditTrail("Updated task", "Updated task: " + mymodel.task_name);
                            ModelState.Clear();
                            response.error_code = "00";
                            response.error_desc = "Updated task successfully ";
                        }
                        else
                        {
                            response.error_code = "01";
                            response.error_desc = "Could not Updated task, kindly contact system admin ";
                        }
                    }
                    else
                    {
                        TasksModel mymodel = new TasksModel
                        {
                            task_name = record.task_name,
                            matter_id = record.matter_id,
                            start_date = record.start_date,
                            due_date = record.due_date,
                            task_status = record.task_status,
                            assigned_to = record.assigned_to,
                            priority = record.priority,
                            description = record.description,
                            created_by = Convert.ToInt16(HttpContext.Session.GetString("userid"))
                        };

                        if (dbhandler.AddTasksRecord(mymodel))
                        {
                            CaptureAuditTrail("Created task", "Created task: " + mymodel.task_name);
                            ModelState.Clear();
                            response.error_code = "00";
                            response.error_desc = "task successfully created";
                        }
                        else
                        {
                            response.error_code = "01";
                            response.error_desc = "Could not Create task, kindly contact system admin";
                        }

                    }
                }
                catch
                {
                    response.error_code = "01";
                    response.error_desc = "Could not Create task, kindly contact system admin";
                }
            }
            return Content(JsonConvert.SerializeObject(response, Formatting.Indented), "application/json");
        }




        [HttpGet]
        public ContentResult GetRecords(string module, string param = "normal")
        {
            FinpayiSecurity.CryptoFactory CryptographyFactory = new FinpayiSecurity.CryptoFactory();
            FinpayiSecurity.ICrypto Cryptographer = CryptographyFactory.MakeCryptographer("rijndael");
            ArrayList details = new ArrayList();
            DataTable datatable = new DataTable();
            DataTable datatableI = new DataTable();
            //System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            JObject jobject = new JObject();
            JArray jarray = new JArray();
            JArray option_array = new JArray();

            switch (module)
            {

                default:
                    datatable = dbhandler.GetRecords(module);
                    break;
            }

            if (datatable.Rows.Count > 0)
            {
                foreach (DataRow dr in datatable.Rows)
                {
                    row = new Dictionary<string, object>();
                    foreach (DataColumn col in datatable.Columns)
                    {
                        row.Add(col.ColumnName, dr[col]);
                    }
                    rows.Add(row);
                }
            }
            return Content(JsonConvert.SerializeObject(rows, Formatting.Indented) /*serializer.Serialize(rows)*/, "application/json");
        }

        public bool CaptureAuditTrail(string action_type, string action_description)
        {
            AuditTrailModel audittrailmodel = new AuditTrailModel
            {
                user_name = HttpContext.Session.GetString("email")!.ToString(),
                action_type = action_type,
                action_description = action_description,
                page_accessed = String.Format("{0}://{1}{2}{3}", HttpContext.Request.Scheme, HttpContext.Request.Host, HttpContext.Request.Path, HttpContext.Request.QueryString), /*Request.Url.ToString(),*/
                //client_ip_address = GetIPAddress(HttpContext), //Request.HttpContext.Connection.RemoteIpAddress.ToString(), /*Request.UserHostAddress,*/
                session_id = HttpContext.Session.Id //HttpContext.Session.GetString("userid") /*Session.SessionID*/
            };
            return dbhandler.AddAuditTrail(audittrailmodel);
        }

        [RBAC]
        public ActionResult Delete(/*[FromBody] JObject jobject*/ int id, string module)
        {

            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("AdminLogin", "AppAuth");
            else
            {
                switch (module)
                {
                    case "task_record":
                        TasksModel taskmodel = dbhandler.GetTasksRecord().Find(mymodel => mymodel.id == id)!;
                        if (taskmodel != null)
                        {
                            dbhandler.DeleteRecord(id, Convert.ToInt16(HttpContext.Session.GetString("userid")), module);
                            CaptureAuditTrail("Deleted task recod", "Deleted task record: " + taskmodel.task_name);
                        }
                        break;
                    

                    default:
                        break;
                }

                return GetRecords(module);
            }
        }

    }
}
