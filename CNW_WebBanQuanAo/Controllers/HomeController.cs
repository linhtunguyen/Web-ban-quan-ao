using CNW_WebBanQuanAo.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;

namespace CNW_WebBanQuanAo.Controllers
{
    public class HomeController : Controller
    {
        MyContext context = new MyContext();
        public ActionResult Index()
        {
            var model = context.MATHANG.Where(x => x.MaMH != null).ToList();
            System.Diagnostics.Debug.WriteLine("Vao on index roi nha");

            return View(model);
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [ChildActionOnly]
        public ActionResult LeftMenu()
        {
            var model = context.NHASANXUAT.ToList();
            return PartialView("~/Views/Shared/_LeftMenu.cshtml", model);
        }
        public ActionResult Detail()
        {
            ViewBag.Message = "Details of a specified product";

            return View();
        }
        [HttpGet]
        public ActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Contact(string subject, string message)
        {
            if (ModelState.IsValid)
            {
                if (subject.Equals("") || message.Equals(""))
                {
                    ModelState.AddModelError("", "Điền thiếu thông tin");
                }
                else
                {
                    PHANHOI model = new PHANHOI();
                    var dn = (TAIKHOAN)Session["dnhap"];

                    var tk = context.TAIKHOAN.Find(dn.Username);
                    var makh = dn.Username;
                    model.NgayGui = DateTime.Now;
                    model.MaKH = makh.ToString();

                    model.TieuDe = subject;
                    model.NoiDung = message;
                    var result = context.PHANHOI.Add(model);
                    if (result != null)
                    {
                        ViewBag.Success = " Gửi phản hồi thành công";
                    }   
                    else
                    {
                        ModelState.AddModelError("", " Không hợp lệ");

                    }
                    context.SaveChanges();
                }

            }
            return View();

        }
        public ActionResult TestWebService()
        {
            return View();
        }
    }
}