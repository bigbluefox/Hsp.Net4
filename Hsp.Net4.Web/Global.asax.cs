using Hsp.Net4.Web.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace Hsp.Net4.Web
{
    public class Global : HttpApplication
    {
        public override void Init()
        {
            base.Init();

            // 初始化RedisHelper
            RedisHelper.Initialization(CSRedisInstance.GetRedis());

            foreach (string moduleName in this.Modules)
            {
                string appName = "APPNAME";
                IHttpModule module = this.Modules[moduleName];
                SessionStateModule ssm = module as SessionStateModule;
                if (ssm != null)
                {
                    FieldInfo storeInfo = typeof(SessionStateModule).GetField("_store", BindingFlags.Instance | BindingFlags.NonPublic);
                    FieldInfo configMode = typeof(SessionStateModule).GetField("s_configMode", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);

                    if (configMode != null)
                    {
                        SessionStateMode mode = (SessionStateMode)configMode.GetValue(ssm);
                        if (mode == SessionStateMode.StateServer)
                        {
                            if (storeInfo != null)
                            {
                                SessionStateStoreProviderBase store = (SessionStateStoreProviderBase)storeInfo.GetValue(ssm);
                                if (store == null)//In IIS7 Integrated mode, module.Init() is called later
                                {
                                    FieldInfo runtimeInfo = typeof(HttpRuntime).GetField("_theRuntime", BindingFlags.Static | BindingFlags.NonPublic);
                                    if (runtimeInfo != null)
                                    {
                                        HttpRuntime theRuntime = (HttpRuntime)runtimeInfo.GetValue(null);
                                        FieldInfo appNameInfo = typeof(HttpRuntime).GetField("_appDomainAppId", BindingFlags.Instance | BindingFlags.NonPublic);
                                        if (appNameInfo != null) appNameInfo.SetValue(theRuntime, appName);
                                    }
                                }
                                else
                                {
                                    Type storeType = store.GetType();
                                    if (storeType.Name.Equals("OutOfProcSessionStateStore"))
                                    {
                                        FieldInfo uribaseInfo = storeType.GetField("s_uribase", BindingFlags.Static | BindingFlags.NonPublic);
                                        if (uribaseInfo != null)
                                        {
                                            uribaseInfo.SetValue(storeType, appName);
                                            object obj = null;
                                            uribaseInfo.GetValue(obj);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;
                }
            }

        }

        void Application_Start(object sender, EventArgs e)
        {
            // 在应用程序启动时运行的代码
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}