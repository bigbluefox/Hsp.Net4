using Hsp.Net4.Web.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Hsp.Net4.Web
{
    public partial class _Default : PageBase
    {
        public string SessionValue { get; set; }


        protected void Page_Load(object sender, EventArgs e)
        {
            var req = Request;
            var id = Request.RequestContext.HttpContext.Session.SessionID;

            if(Session["Demo"] == null)
                Session["Demo"] = "Hello World! SessionID: " + id + "，操作时间：" + DateTime.Now.ToString("F");

            //Console.WriteLine("Web Form SessionID:", id);

            var abc = Session["Demo"];
            var def = abc;

            SessionValue = Session["Demo"] == null? "Session值为空" : Session["Demo"].ToString();
        }

        // https://www.cnblogs.com/rdst/p/3162143.html
        //aspnet_regsql -S 数据库实例名 -ssadd -sstype p -U 连接用户名
        //aspnet_regsql -S DESKTOP-5U133HA -ssadd -sstype p -U sa



    }
}