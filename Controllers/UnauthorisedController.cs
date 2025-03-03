using Microsoft.AspNetCore.Mvc;
using SHERIA.Models;

namespace SHERIA.Controllers
{
    public class UnauthorisedController : Controller
    {
        private DBHandler dbhandler;

        public UnauthorisedController(DBHandler mydbhandler)
        {
            dbhandler = mydbhandler;
        }

        /* GET: Unauthorised */
        public ActionResult Index()
        {
            ViewBag.MenuLayout = HttpContext.Session.GetString("menulayout");
            MenuHandler menuhandler = new MenuHandler(dbhandler);
            IEnumerable<MenuModel> menu = menuhandler.GetMenu(Convert.ToInt16(HttpContext.Session.GetString("profileid")), HttpContext.Request.Path);
            //HttpContext.Session.Clear();
            return View(menu);
        }
    }
}