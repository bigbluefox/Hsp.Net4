using Hsp.Net4.Web.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hsp.Net4.Web.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController()
        {

        }

        // GET: Home/Hello
        public ActionResult Hello()
        {
            var req = Request;
            var id = Request.RequestContext.HttpContext.Session.SessionID;

            if (Session["Demo"] == null)
                Session["Demo"] = "Hello World! SessionID: " + id + "，操作时间：" + DateTime.Now.ToString("F");

            Console.WriteLine("MVC SessionID:", id);

            var abc = Session["Demo"];
            var def = abc;

            ViewBag.SessionValue = Session["Demo"] == null ? "Session值为空" : Session["Demo"].ToString();

            return View();
        }
    }
}