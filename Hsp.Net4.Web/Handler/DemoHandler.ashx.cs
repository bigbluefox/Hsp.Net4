using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace Hsp.Net4.Web.Handler
{
    /// <summary>
    /// DemoHandler 的摘要说明
    /// </summary>
    public class DemoHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            var rst = context.Session["Demo"];


            context.Response.Write(rst);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}