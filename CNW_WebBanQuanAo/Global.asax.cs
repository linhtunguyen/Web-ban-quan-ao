using CNW_WebBanQuanAo.Controllers;
using CNW_WebBanQuanAo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace CNW_WebBanQuanAo
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        public void Session_OnStart()
        {
            //System.Diagnostics.Debug.WriteLine("Vao on start roi nha");
            //Session.Timeout = 1;
        }

        public void Session_OnEnd()
        {
            //System.Diagnostics.Debug.WriteLine("Vao on end roi nha");
            //CartController cc = new CartController();
            var user = (TAIKHOAN)Session["dnhap"];
            var cart = (Cart)Session["CartSession"];

            if (user != null && cart != null)
            {
                CartController.GioSessionVaoCSDL(user.Username, cart);
            }
        }
    }
}