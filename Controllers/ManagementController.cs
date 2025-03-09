using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using SHERIA.Helpers;
using SHERIA.Models;
using static SHERIA.Helpers.CryptoHelper;
using System.Collections;
using System.Data;
using static SHERIA.Controllers.ClientManagementController;
using System.Net.Http;
using System.Net.Mime;
using System.Text;

namespace SHERIA.Controllers
{
    public class ManagementController : Controller
    {
        private IWebHostEnvironment ihostingenvironment;
        private ILoggerManager iloggermanager;
        private DBHandler dbhandler;
        private HttpClient _httpClient;

        public ManagementController(ILoggerManager logger, IWebHostEnvironment environment, DBHandler mydbhandler, HttpClient httpClient)
        {
            iloggermanager = logger;
            ihostingenvironment = environment;
            dbhandler = mydbhandler;
            _httpClient = httpClient;
        }

        public class matters_record
        {
            public Int64 id { set; get; }
            public string? matter_name { set; get; }
            public string? matter_number { set; get; }
            public Int64 assigned_to { set; get; }
            public Int64 client_id { set; get; }
            public DateTime start_date { set; get; }
            public DateTime close_date { set; get; }
            public string? practice_area { set; get; }
            public string? matter_status { set; get; }
            public string? matter_billing { set; get; }
            public string? description { set; get; }
        }
        
        public class product_record
        {
            public Int64 id { set; get; }
            public string? name { set; get; }
            public Int64 type { set; get; }
            public Int64 total { set; get; }
            public string? price { set; get; }
        }

        public IActionResult Product()
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
        public IActionResult ProductPurchase()
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
        public ActionResult CreateProduct(product_record record)
        {
            processingresponse response = new processingresponse
            {
                system_ref = DateTime.Now.ToString("yyyyMMddHHmmssfff")
            };

            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("AdminLogin", "AppAuth");
            else
            {
                if (record.name == null)
                    return Content("Invalid Name");

                try
                {
                    ProductModel existingrecord = dbhandler.GetProductRecord().Find(mymodel => mymodel.id == record.id)!;
                    if (existingrecord != null)
                    {
                        ProductModel mymodel = new ProductModel
                        {
                            id = existingrecord.id,
                            name = record.name,
                            type = record.type,
                            total = record.total,
                            price = record.price
                        };

                        if (dbhandler.UpdateProductRecord(mymodel))
                        {
                            ModelState.Clear();
                            response.error_code = "00";
                            response.error_desc = "Updated matters successfully ";
                        }
                        else
                        {
                            response.error_code = "01";
                            response.error_desc = "Could not Updated matters, kindly contact system admin ";
                        }
                    }
                    else
                    {
                        ProductModel mymodel = new ProductModel
                        {
                            name = record.name,
                            type = record.type,
                            total = record.total,
                            price = record.price
                        };

                        if (dbhandler.AddProductRecord(mymodel))
                        {
                            ModelState.Clear();
                            response.error_code = "00";
                            response.error_desc = "Matter successfully created";
                        }
                        else
                        {
                            response.error_code = "01";
                            response.error_desc = "Could not Create matters, kindly contact system admin";
                        }
                            
                    }
                }
                catch
                {
                    response.error_code = "01";
                    response.error_desc = "Could not Create matters, kindly contact system admin";
                }
            }
            return Content(JsonConvert.SerializeObject(response, Formatting.Indented), "application/json");
        }

        [HttpPost]
        public ActionResult UpdateMatters(Int64 id, string module)
        {
            if (HttpContext.Session.GetString("name") == null )
                return RedirectToAction("AdminLogin", "AppAuth");
            else
            {
                switch (module)
                {
                    case "open_matter_status":
                        if (dbhandler.Update_Open_Matter_Status(id, module))
                        {
                            return Content("Success");

                        }
                        break;
                    case "close_matter_status":
                        if (dbhandler.Update_Open_Matter_Status(id, module))
                        {
                            return Content("Success");

                        }
                        break;
                    default:
                        break;
                }

                return Content("Fail");
            }
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

        [HttpPost]
        public ActionResult Delete(/*[FromBody] JObject jobject*/ int id, string module)
        {

            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("AdminLogin", "AppAuth");
            else
            {
                switch (module)
                {
                    case "delete_product":

                        dbhandler.DeleteRecord(id, Convert.ToInt16(HttpContext.Session.GetString("userid")), module);
                        break;
                    case "purchase_product":

                        dbhandler.DeleteRecord(id, Convert.ToInt16(HttpContext.Session.GetString("userid")), module);
                        break;


                    default:
                        break;
                }

                return GetRecords(module);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DownloadAction(string amount, string mobile)
        {
            
            // Process mobile number: remove spaces and keep only the last 9 digits
            string cleanedMobile = new string(mobile.Where(char.IsDigit).ToArray());
            string last9Digits = cleanedMobile.Length >= 9
                ? cleanedMobile.Substring(cleanedMobile.Length - 9)
                : cleanedMobile; // Fallback for numbers shorter than 9 digits
            string phone_no = "254" + last9Digits;

            // Process amount: remove spaces and decimals, ensure it's a whole number
            string cleanedAmount = new string(amount.Where(char.IsDigit).ToArray());

            string baseUrl = "https://localhost:44347/api/MpesaIntergration/SubmitSTKRequest";
            iloggermanager.LogInfo("baseUrl: " + baseUrl);

            JObject requestBody = new JObject
            {
                { "amount", cleanedAmount },
                { "phone_no", phone_no } 
            };
            iloggermanager.LogInfo("requestBody: " + requestBody.ToString());

            StringContent content = new StringContent(requestBody.ToString(), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(baseUrl, content);
            string result = await response.Content.ReadAsStringAsync();
            iloggermanager.LogInfo("RESULT: " + result);
            JObject result_json = JObject.Parse(result);
            iloggermanager.LogInfo("result_json: " + result_json);

            return Ok();

            
        }

    }
}
