using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SHERIA.Helpers;
using SHERIA.Models;
using SHERIA.Utility;
using System.Security.Cryptography;
using System.Text;

namespace SHERIA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // api/MpesaIntergration
    public class MpesaIntergrationController : ControllerBase
    {

        private readonly IHttpClientFactory _httpClientFactory;
        private ILoggerManager iloggermanager;
        private IWebHostEnvironment ihostingenvironment;
        private DBHandler dbhandler;

        public MpesaIntergrationController(ILoggerManager logger,IHttpClientFactory httpClientFactory, IWebHostEnvironment environment, DBHandler mydbhandler)
        {
            _httpClientFactory = httpClientFactory;
            iloggermanager = logger;
            ihostingenvironment = environment;
            dbhandler = mydbhandler;
        }

        public class B2CRequestModel
        {
            public string? PhoneNumber { get; set; }
            public decimal Amount { get; set; }
            public string? Remarks { get; set; }
        }

        [HttpGet]

        public async Task<string?> GetToken()
        {
            var client = _httpClientFactory.CreateClient("");
            var authString = "CeiebLBspGGneIkgr8WHHUyD6XNsnpKBMj7EiL2tAeEIOWEU:FnpNDE26VecDHJveWuinyxZYoZp4dxB25DZiHxL2CGbHAoKdVjWttJphKTnsel39";
            //var encodedString = Convert.ToBase64String(Encoding.UTF8.GetBytes(authString));
            var encodedString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authString));
            var _url = SD.mpesa + "/oauth/v1/generate?grant_type=client_credentials";
            var request = new HttpRequestMessage(HttpMethod.Get, _url);
            request.Headers.Add("Authorization", $"Basic {encodedString}");
            //request.Headers.Add("Authorization", "Basic NnZwclFZVXN6MUdwcUNHM2FoWkxoQVozMkJZZ0tCSE86N3RKWG16MVhJWFJ2eEJvMA==");
            var response = await client.SendAsync(request);
            var mpesaResponse = await response.Content.ReadAsStringAsync();
            TokenModel tokenObject = JsonConvert.DeserializeObject<TokenModel>(mpesaResponse)!;

            return tokenObject.access_token;

        }


        [HttpPost]
        [Route("SubmitSTKRequest")]
        // api/SubmitSTKRequest
        public async Task<string> SendStkPush([FromBody] B2CRequestModel model)
        {
            var token = await GetToken();
            iloggermanager.LogInfo("Mpesa token: " + Convert.ToString(token));

            var client = _httpClientFactory.CreateClient("");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var url = SD.mpesa + "/mpesa/stkpush/v1/processrequest";
            iloggermanager.LogInfo("Mpesa STK End point: " + Convert.ToString(url));

            //string securityCredential = "fR8akwmLOS2Lk4jC3py85Sr9ggvGewSPPF66anJWOJmeVYO3rcJatpxr6wIdpo/UWfLt75IT++abtHfePcbTs+e+Cixmh0pmT4sQhDPeJUUkQePvR3tRlDGu1xsOyVz0r6QbG8ohN7IW4SnvQ2J/i88FqeHkiT0j0daJndtdqH7ccP6sdr6osMLITyzfArgLBc3ayhoagoV/gHpyLvLaUwEcpj2BO8SHBYE2peHwc82Zy6MUcRuM0d82YRuikTXsaRFei8M/O7H6KMk/xtL5sTvdCCBQH4d8aFfwEbuvILYUfi4npMQaFdc16s1WeyU46PXxB7qnDTU5G6tMVbQnYw==";
            string external_ref_num = DateTime.Now.ToString("yyyyMMddHHmmssfff");

            var jsonBody = JsonConvert.SerializeObject(new
            {
                BusinessShortCode = "174379",
                Password = "MTc0Mzc5YmZiMjc5ZjlhYTliZGJjZjE1OGU5N2RkNzFhNDY3Y2QyZTBjODkzMDU5YjEwZjc4ZTZiNzJhZGExZWQyYzkxOTIwMjUwMzA5MjExODMy",
                Timestamp = "20250309211718",
                TransactionType = "CustomerPayBillOnline",
                Amount = "1",
                PartyA = "254713249357",
                PartyB = 174379,
                PhoneNumber = "254713249357",
                CallBackURL = "https://mydomain.com/pat",
                AccountReference = "Test",
                TransactionDesc = "Test"
            });

            iloggermanager.LogInfo("STK payload request: " + Convert.ToString(jsonBody));

            var jsonReadyBody = new StringContent(
                jsonBody.ToString(),
                Encoding.UTF8,
                "application/json"

                );
            //iloggermanager.LogInfo("B2C payload request: " + Convert.ToString(jsonReadyBody));
            var response = await client.PostAsync(url, jsonReadyBody);
            var result = await response.Content.ReadAsStringAsync();

            JObject json = JObject.Parse(result);

            //bool hasConversationID = json.ContainsKey("ConversationID");
           // bool hasRequestId = json.ContainsKey("requestId");


            //if (hasConversationID)
            //{
            //    // dbhandler.AddmpesaB2CSuccessResponse(json["ConversationID"]!.ToString(), json["OriginatorConversationID"]!.ToString(), json["ResponseCode"]!.ToString(), json["ResponseDescription"]!.ToString());
            //}

            //if (hasRequestId)
            //{
            //    dbhandler.AddmpesaB2CErrorResponse(json["requestId"]!.ToString(), json["OriginatorConversationID"]!.ToString(), json["errorCode"]!.ToString(), json["errorMessage"]!.ToString());
            //}

            //iloggermanager.LogInfo("B2C request response result: " + Convert.ToString(result));
            return result;

        }

        //[HttpPost]
        //[Route("SubmitRequest")]
        //public async Task<string> SendB2CRequest([FromBody] B2CRequestModel model)
        //{
        //    var token = await GetToken();
        //    iloggermanager.LogInfo("Mpesa token: " + Convert.ToString(token));

        //    var client = _httpClientFactory.CreateClient("");
        //    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        //    var url = SD.mpesa + "/mpesa/b2c/v3/paymentrequest";
        //    iloggermanager.LogInfo("Mpesa B2c End point: " + Convert.ToString(url));

        //    string securityCredential = "fR8akwmLOS2Lk4jC3py85Sr9ggvGewSPPF66anJWOJmeVYO3rcJatpxr6wIdpo/UWfLt75IT++abtHfePcbTs+e+Cixmh0pmT4sQhDPeJUUkQePvR3tRlDGu1xsOyVz0r6QbG8ohN7IW4SnvQ2J/i88FqeHkiT0j0daJndtdqH7ccP6sdr6osMLITyzfArgLBc3ayhoagoV/gHpyLvLaUwEcpj2BO8SHBYE2peHwc82Zy6MUcRuM0d82YRuikTXsaRFei8M/O7H6KMk/xtL5sTvdCCBQH4d8aFfwEbuvILYUfi4npMQaFdc16s1WeyU46PXxB7qnDTU5G6tMVbQnYw==";
        //    string external_ref_num = DateTime.Now.ToString("yyyyMMddHHmmssfff");

        //    var jsonBody = JsonConvert.SerializeObject(new
        //    {
        //        OriginatorConversationID = external_ref_num,
        //        InitiatorName = "testapi", 
        //        SecurityCredential = securityCredential, 
        //        CommandID = "BusinessPayment", 
        //        Amount = model.Amount,
        //        PartyA = 600990,
        //        PartyB = model.PhoneNumber,
        //        Remarks = model.Remarks,
        //        QueueTimeOutURL = "https://riziki.app:9010/api/MpesaIntergration/timeoutRequest", 
        //        ResultURL = "https://riziki.app:9010/api/MpesaIntergration/resultRequest", 
        //        Occasion = "Bima_d_Line Payment" 
        //    }) ;

        //    iloggermanager.LogInfo("B2C payload request: " + Convert.ToString(jsonBody));

        //    var jsonReadyBody = new StringContent(
        //        jsonBody.ToString(),
        //        Encoding.UTF8,
        //        "application/json"

        //        );
        //    //iloggermanager.LogInfo("B2C payload request: " + Convert.ToString(jsonReadyBody));
        //    var response = await client.PostAsync(url, jsonReadyBody);
        //    var result = await response.Content.ReadAsStringAsync();

        //    JObject json = JObject.Parse(result);

        //    bool hasConversationID = json.ContainsKey("ConversationID");
        //    bool hasRequestId = json.ContainsKey("requestId");


        //    if (hasConversationID)
        //    {
        //       // dbhandler.AddmpesaB2CSuccessResponse(json["ConversationID"]!.ToString(), json["OriginatorConversationID"]!.ToString(), json["ResponseCode"]!.ToString(), json["ResponseDescription"]!.ToString());
        //    }

        //    if (hasRequestId)
        //    {
        //        dbhandler.AddmpesaB2CErrorResponse(json["requestId"]!.ToString(), json["OriginatorConversationID"]!.ToString(), json["errorCode"]!.ToString(), json["errorMessage"]!.ToString());
        //    }

        //    iloggermanager.LogInfo("B2C request response result: " + Convert.ToString(result));
        //    return result;

        //}

        //[HttpPost] 
        //[Route("timeoutRequest")]
        //public JToken TimeoutRequest([FromBody] JObject jobject)
        //{
        //    HttpHandler httphandler = new HttpHandler();
        //    RandomKeyGeneratorManagement randomkeymanager = new RandomKeyGeneratorManagement();
        //    JToken json_res;
        //    JObject json = new JObject();
        //    string ip = Request.HttpContext.Connection.RemoteIpAddress!.ToString();

        //    try
        //    {
        //        iloggermanager.LogInfo("Incoming traffice from IP address: " + ip);
        //        iloggermanager.LogInfo("STK Push Callback: " + jobject.ToString());

        //        //json = ProcessBimadlineCallback(jobject);
        //        iloggermanager.LogInfo("STK Push response feedback: " + json.ToString());


        //    }
        //    catch (Exception ex)
        //    {
        //        iloggermanager.LogInfo(ex.Message + " " + ex.StackTrace);
        //        json = new JObject
        //        {
        //            { "error_code", "01" },
        //            { "error_desc", ex.StackTrace }
        //        };

        //        iloggermanager.LogInfo("B2C timeout Callback: " + json.ToString());
        //    }

        //    json_res = json;

        //    return json_res;
        //}


        //[HttpPost] 
        //[Route("resultRequest")]
        //public JToken ResultRequest([FromBody] JObject jobject)
        //{
        //    HttpHandler httphandler = new HttpHandler();
        //    RandomKeyGeneratorManagement randomkeymanager = new RandomKeyGeneratorManagement();
        //    JToken json_res;
        //    JObject json = new JObject();

        //    try
        //    {
        //        iloggermanager.LogInfo("B2C Result Callback: " + jobject.ToString());
        //        dbhandler.UpdatempesaB2CResult(jobject.ToString(),json["ResultType"]!.ToString(), json["ResultCode"]!.ToString(), json["ResultDesc"]!.ToString(), json["OriginatorConversationID"]!.ToString(), json["TransactionID"]!.ToString());

        //        //json = ProcessBimadlineCallback(jobject);
        //        // iloggermanager.LogInfo("STK Push response feedback: " + json.ToString());


        //    }
        //    catch (Exception ex)
        //    {
        //        iloggermanager.LogInfo(ex.Message + " " + ex.StackTrace);
        //        json = new JObject
        //        {
        //            { "error_code", "01" },
        //            { "error_desc", ex.StackTrace }
        //        };
        //        iloggermanager.LogInfo("STK Push Callback: " + json.ToString());
        //    }

        //    json_res = json;

        //    return json_res;
        //}



        public static string GenerateSecurityCredential(string initiatorPassword, string publicKey)
        {
            var encryptedPassword = EncryptPassword(initiatorPassword, publicKey);
            return Convert.ToBase64String(encryptedPassword);
        }

        private static byte[] EncryptPassword(string password, string publicKey)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKey);
                return rsa.Encrypt(passwordBytes, false);
            }
        }



        [HttpGet]
        [Route("register-urls")]
        public async Task<string?> RegisterMpesaUrls()
        {
            var jsonBody = JsonConvert.SerializeObject(new
            {
                ValidationURL = "https://mydemo.url.com/confirmation",
                ConfirmationURL = "https://mydemo.url.com/confirmation",
                ResponseType = "Completed",
                ShortCode = 600991
            });

            var jsonReadyBody = new StringContent(
                jsonBody.ToString(),
                Encoding.UTF8,
                "application/json"

                );

            var token = await GetToken();

            var client = _httpClientFactory.CreateClient("");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var url = SD.mpesa + "/mpesa/c2b/v1/registerurl";

            var response = await client.PostAsync(url, jsonReadyBody);

            return await response.Content.ReadAsStringAsync();



        }




    }
}
