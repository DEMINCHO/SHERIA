using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using SHERIA.Helpers;
using SHERIA.Models;
using System.Collections;

namespace SHERIA.Controllers
{
    public class ClientManagementController : Controller
    {
        private IWebHostEnvironment ihostingenvironment;
        private ILoggerManager iloggermanager;
        private DBHandler dbhandler;

        public ClientManagementController(ILoggerManager logger, IWebHostEnvironment environment, DBHandler mydbhandler)
        {
            iloggermanager = logger;
            ihostingenvironment = environment;
            dbhandler = mydbhandler;
        }

        public class onboardrecord
        {
            public onboardclientrecord[]? applicant_details { get; set; }
            public string? client_files { get; set; }
        }

        public class onboardclientrecord
        {
            public Int64 id { get; set; }
            public string? client_type { get; set; }
            public string? first_name { get; set; }
            public string? middle_name { get; set; }
            public string? last_name { get; set; }
            public string? company_name { get; set; }
            public string? id_number { get; set; }
            public string? kra_pin { get; set; }
            public string? cert_of_incoporation { get; set; }
            public string? nationality { get; set; }
            public string? country { get; set; }
            public string? phone_number { get; set; }
            public string? sec_phone_number { get; set; }
            public string? email { get; set; }
            public string? physical_address { get; set; }
            public string? postal_address { get; set; }
            public string? remarks { get; set; }
        }
        public class productpurchaserecord
        {
            public Int64 id { get; set; }
            public string? client_name { get; set; }
            public string? mobile { get; set; }
            public string? email { get; set; }
            public string? id_no { get; set; }
            public Int64 product { get; set; }
            public Int64 product_total { get; set; }
            public string? price { get; set; }
            public string? start_date { get; set; }
            public string? return_date { get; set; }
            public Int64 return_quantity { get; set; }
            public DateTime actual_return_date { get; set; }
            public string? return_condition { get; set; }
            public string? return_notes { get; set; }


        }
        public class processingresponse
        {
            public string? error_code { get; set; }
            public string? error_desc { get; set; }
            public string? system_ref { get; set; }
            public string? account_number { get; set; }
        }


        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Onboard()
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
        public ActionResult ProductPurchase(productpurchaserecord record)
        {
            ArrayList details = new ArrayList();
            processingresponse response = new processingresponse
            {
                system_ref = DateTime.Now.ToString("yyyyMMddHHmmssfff")
            };

            try
            {
                if (record.email == null)
                {
                    record = null;
                    response.error_code = "01";
                    response.error_desc = "Client Email Missing!";
                }
                else
                {
                    //var user = HttpContext.Session.GetString("userid");
                    ProductpurchaseModel clientModel = new ()
                    {
                        client_name = record.client_name,
                        mobile = record.mobile,
                        email = record.email,
                        id_no = record.id_no,
                        product = record.product,
                        product_total = record.product_total,
                        price = record.price,
                        start_date = record.start_date,
                        return_date = record.return_date,
                        created_by = Convert.ToInt64(HttpContext.Session.GetString("userid"))
                    };

                    Int64 client_id = dbhandler.AddProductPurchase(clientModel);


                    if (client_id > 0)
                    {
                        ModelState.Clear();
                        response.error_code = "00";
                        response.error_desc = "Product Purchase was success, you can proceed";

                    }
                }
            }
            catch (Exception ex)
            {

                iloggermanager.LogError(ex.Message);
                response.error_code = "01";
                response.error_desc = "Could not create client, kindly contact system admin";
            }

            return Content(JsonConvert.SerializeObject(response, Formatting.Indented), "application/json");

        } 
        
        [HttpPost]
        public ActionResult ReturnProduct(productpurchaserecord record)
        {
            ArrayList details = new ArrayList();
            processingresponse response = new processingresponse
            {
                system_ref = DateTime.Now.ToString("yyyyMMddHHmmssfff")
            };

            try
            {
                //var user = HttpContext.Session.GetString("userid");
                ProductpurchaseModel clientModel = new()
                {
                    id = record.id,
                    return_quantity = record.return_quantity,
                    actual_return_date = record.actual_return_date,
                    return_condition = record.return_condition,
                    return_notes = record.return_notes,
                };

                if (dbhandler.UpdateProductPurchase(clientModel))
                {
                    ModelState.Clear();
                    response.error_code = "00";
                    response.error_desc = "Successfully Returned";
                    return Content(JsonConvert.SerializeObject(response, Formatting.Indented), "application/json");
                }
                else
                {
                    ModelState.Clear();
                    response.error_code = "01";
                    response.error_desc = "Failed to Update Return";
                    return Content(JsonConvert.SerializeObject(response, Formatting.Indented), "application/json");
                }
            }
            catch (Exception ex)
            {

                iloggermanager.LogError(ex.Message);
                response.error_code = "01";
                response.error_desc = "Could not create client, kindly contact system admin";
                return Content(JsonConvert.SerializeObject(response, Formatting.Indented), "application/json");
            }

            return Content(JsonConvert.SerializeObject(response, Formatting.Indented), "application/json");

        }

        [HttpPost]
        public IActionResult Upload(List<IFormFile> postedFiles)
        {
            JArray jarray = new JArray();
            string wwwPath = ihostingenvironment.WebRootPath;
            string contentPath = ihostingenvironment.ContentRootPath;

            string path = Path.Combine(ihostingenvironment.WebRootPath, "Uploads");
            //string path = dbhandler.GetRecords("parameters", "UPLOAD_FILE_PATH").Rows[0]["item_value"].ToString()!;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            foreach (IFormFile postedFile in postedFiles)
            {
                string fileName = DateTime.Now.ToFileTimeUtc().ToString() + Path.GetExtension(postedFile.FileName);
                using FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create);
                postedFile.CopyTo(stream);

                jarray.Add(new JObject {
                    { "original_file_name",  postedFile.FileName },
                    { "new_file_name",  fileName },
                    { "message",  "success" }
                });
            }

            //return Content("Success");
            return Content(JsonConvert.SerializeObject(jarray, Formatting.Indented), "application/json");
        }

       
    }

    
}
