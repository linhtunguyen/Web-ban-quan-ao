using CNW_WebBanQuanAo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CNW_WebBanQuanAo.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        //Context
        public static string url;
        MyContext context = new MyContext();
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                if (CheckUserName(model.Username))
                {
                    ModelState.AddModelError("", "Tên này đã tồn tại, vui lòng nhập tên khác");
                }
                else if (CheckEmail(model.Email))
                {
                    ModelState.AddModelError("", "Email này đã được sử dụng");
                }
                else
                {
                    if ((model.Password != model.ConfirmPassword))
                    {
                        ModelState.AddModelError("", "Xác thực mật khẩu không đúng");
                    }
                    else
                    {
                        var user = new TAIKHOAN();
                        user.Username = model.Username;
                        user.SDT = model.SDT;
                        user.isAdmin = 0;
                        user.HoTen = model.HoTen;
                        user.Password = model.Password;
                        user.DiaChi = model.DiaChi;
                        user.Email = model.Email;
                        var result = context.TAIKHOAN.Add(user);
                        if (result != null)
                        {
                            ViewBag.Success = " Đăng kí thành công";
                            model = new RegisterModel();
                        }
                        else
                        {
                            ModelState.AddModelError("", " Đăng kí thất bại");
                        }
                        context.SaveChanges();
                    }
                }
            }

            return View("DangNhap");
        }

        public bool CheckUserName(string Username)
        {
            return context.TAIKHOAN.Count(x => x.Username == Username) > 0;
        }

        public bool CheckEmail(string Email)
        {
            return context.TAIKHOAN.Count(x => x.Email == Email) > 0;
        }

        [HttpGet]
        public ActionResult DangNhap()
        {
            ViewBag.returnUrl = Request.UrlReferrer;
            return View();
        }
        [HttpPost]
        public ActionResult DangNhap(TAIKHOAN acc, string returnUrl)
        {
            url = returnUrl;
            if (url is null) 
                url = ViewBag.returnUrl;

            var result = context.TAIKHOAN
                .Where(a => a.Username.Equals(acc.Username) && a.Password.Equals(acc.Password))
                .FirstOrDefault();

            if (ModelState.IsValid)
            {
                if (result != null)
                {
                    CartController cc = new CartController();
                    Session["dnhap"] = result;
                    var cart = (Cart) Session["CartSession"];
                    if (cart is null)
                    {
                        cart = new Cart();
                        Session["CartSession"] = cart;
                    }

                    cc.GioCSDLVaoSession(result.Username, cart);

                    if (result.isAdmin == 1)
                    {
                        Session["AdminLogin"] = acc;
                        return Redirect("/Admin/Admin/Index");
                    }
                    else
                    {
                        return Redirect(url);
                    }
                }
                else
                {
                    Response.Write("Dang nhap sai");
                    ViewBag.returnUrl = url;
                }
            }
            return View();
        }
        public int CheckUser(string Username, string Password)
        {
            int kq = context.TAIKHOAN.Count(x => x.Username == Username && x.Password != Password);
            int kq2 = context.TAIKHOAN.Count(x => x.Username != Username && x.Password == Password);
            if (kq > 0)
                return 1;
            else if (kq2 > 0)
                return 2;
            else
                return 3;
        }
        public ActionResult Logout()
        {
            var user = (TAIKHOAN)Session["dnhap"];
            var cart = (Cart)Session["CartSession"];

            if (user != null && cart != null)
            {
                CartController.GioSessionVaoCSDL(user.Username, cart);
            }

            Session["dnhap"] = null;
            Session["CartSession"] = null;

            return Redirect("/Home/Index");
        }
    }
}