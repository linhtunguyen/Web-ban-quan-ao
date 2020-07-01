﻿using CNW_WebBanQuanAo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;
using PagedList.Mvc;
using CNW_WebBanQuanAo.ViewModel;
using System.Web.Mvc;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using System.Net;
namespace CNW_WebBanQuanAo.Controllers
{
    public class CartController : Controller
    {
        public static MyContext context = new MyContext();
        private List<string> CheckoutProds = new List<string>();
        public static int ID;
        // GET: Cart
        public ActionResult Gio()
        {
            var cart = (Cart)Session["CartSession"];

            if (cart == null)
            {
                cart = new Cart();
            }
            if (Session["dnhap"] != null)
            {
                var dn = (TAIKHOAN)Session["dnhap"];
                var model1 = (from m in context.TAIKHOAN
                              join n in context.GIOHANG on m.Username equals n.MaKH
                              join k in context.SANPHAM on n.MaQA equals k.MaQA
                              join h in context.MATHANG on k.MaMH equals h.MaMH
                              join a in context.SIZE on k.MaSize equals a.MaSize
                              join b in context.MAU on k.MaMau equals b.MaMau
                              join c in context.ANH on h.MaMH equals c.MaMH
                              where m.Username == dn.Username && c.MaMau == b.MaMau
                              select new dschitietsanpham()
                              {
                                  maqa = n.MaQA,
                                  so = n.SoLuong,
                                  gia = h.GiaBan.Value,
                                  size = a.MaSize,
                                  tenmau = b.TenMau,
                                  url = c.UrlAnh,
                                  tenh = h.TenMH

                              }
                       ).ToList();

                var giodn = context.GIOHANG.Where(m => m.MaKH == dn.Username).FirstOrDefault();

                var pro = model1.FirstOrDefault();

                // var cart = (Cart)Session["CartSession"];
                cart = new Cart();
                if (giodn != null)
                {

                    var product = context.SANPHAM.Find(pro.maqa);
                    var sl = context.GIOHANG.Find(dn.Username, pro.maqa);
                    cart.AddItem(product, sl.SoLuong);
                    Session["CartSession"] = cart;
                }
            }

            return View(cart);
        }

        public static string MoneyType(int? money)
        {
            if (!money.HasValue) return "";

            var m = money.ToString();
            int c = 1;
            for (int i = m.Length - 1; i >= 0; i--)
            {
                if (c % 3 == 0)
                    m = m.Insert(i, " ");
                c++;
            }

            return m;
        }

        public ActionResult AddItem(int id, int quant)
        {
            var product = context.SANPHAM.Find(id);

            var cart = (Cart)Session["CartSession"];
            if (cart == null)
            {
                //tạo mới đối tượng cart item
                cart = new Cart();
                cart.AddItem(product, quant);
                //Gán vào session
                Session["CartSession"] = cart;
            }
            else if (cart != null)
            {
                cart.AddItem(product, quant);
                //Gán vào session
                Session["CartSession"] = cart;
            }


            //return RedirectToAction("Gio");
            return RedirectToAction("Payment");
        }

        //public JsonResult GioSessionVaoCSDL()
        //{
        //    var cart = (Cart)Session["CartSession"];
        //    var dn = (TAIKHOAN)Session["dnhap"];

        //    foreach (var item in cart.Lines)
        //    {
        //        GIOHANG gio = new GIOHANG();
        //        gio.MaKH = dn.Username;
        //        gio.MaQA = item.Sanpham.MaQA;
        //        gio.SoLuong = item.Quantity;
        //        context.GIOHANG.Add(gio);

        //    }
        //    int k = context.SaveChanges();
        //    return Json(k);
        //}

        //public JsonResult GopVoiGioSession()
        //{
        //    var dn = (TAIKHOAN)Session["dnhap"];
        //    var giodn = context.GIOHANG.Where(m => m.MaKH == dn.Username).FirstOrDefault();

        //    var cart = (Cart)Session["CartSession"];
        //    cart = new Cart();
        //    if (giodn != null)
        //    {

        //        var product = context.SANPHAM.Find(giodn.MaQA);
        //        var sl = context.GIOHANG.Find(dn.Username, giodn.MaQA);
        //        cart.AddItem(product, sl.SoLuong.Value);
        //        Session["CartSession"] = cart;
        //    }

        //    return Json(cart, JsonRequestBehavior.AllowGet);
        //}

        [HttpPost]
        public ActionResult AddItemCSDL(GIOHANG model)
        {
            context.GIOHANG.Add(model);
            context.SaveChanges();

            return Redirect("/Home/Index");
        }
        public ActionResult RemoveLine(int id)
        {
            var product = context.SANPHAM.Find(id);

            var cart = (Cart)Session["CartSession"];

            if (cart != null)
            {
                cart.RemoveLine(product);
                //Gán vào session
                Session["CartSession"] = cart;
            }
            return RedirectToAction("Gio");
        }
        public int UpdateCart(int masp, int qty)
        {
            var cart = (Cart)Session["CartSession"];

            if (cart != null)
            {
                var product = context.SANPHAM.Find(masp);
                if (product.SoLuong > qty)
                {
                    cart.UpdateItem(product, qty);
                    Session["CartSession"] = cart;

                    var t = qty * product.MATHANG.GiaBan;
                    if (t.HasValue)
                        return (int)t;
                }
                else
                {
                    ModelState.AddModelError("", "Sản phẩm chưa đủ số lượng !!");
                }
            }

            return 0;
        }

        [HttpGet]
        public ActionResult Payment()
        {
            //if (Session["dnhap"] != null)
            //{
            //    var cart = (Cart)Session["CartSession"];
            //    if (cart == null)
            //    {
            //        cart = new Cart();
            //    }
            //    return View(cart);
            //}
            //return Redirect("/Account/DangNhap");

            var cart = (Cart)Session["CartSession"];

            if (cart == null)
            {
                cart = new Cart();
            }
            return View(cart);
        }

        [HttpPost]
        public ActionResult Payment(string MaKH, string TenKhach, string DiaChiKhach, string TrangThai)
        {
            HOADON model = new HOADON();
            model.MaKH = MaKH;
            model.TenKhach = TenKhach;
            model.DiaChiKhach = DiaChiKhach;
            model.TrangThai = TrangThai;
            model.NgayDat = DateTime.Now;

            try
            {
                //Tao hoa don moi
                var x = context.HOADON.OrderByDescending(s => s.MaHD).Take(1).Single();
                model.MaHD = x.MaHD + 1;
                context.HOADON.Add(model);
                context.SaveChanges();


                //var cart = (Cart)Session["CartSession"];
                //foreach (var item in cart.Lines)
                //{
                //    GIAODICH obj = new GIAODICH();
                //    obj.MaHD = model.MaHD;
                //    obj.MaQA = item.Sanpham.MaQA;
                //    obj.SoLuong = item.Quantity;

                //    context.GIAODICH.Add(obj);
                //    context.SaveChanges();
                //}
                //cart.Clear();
                //Session["CartSession"] = cart;

                for (int i = 0; i < CheckoutProds.Count; i += 2)
                {
                    GIAODICH gd = new GIAODICH();
                    gd.MaHD = model.MaHD;
                    gd.MaQA = Convert.ToInt32(CheckoutProds[i]);
                    gd.SoLuong = Convert.ToInt32(CheckoutProds[i + 1]);

                    context.GIAODICH.Add(gd);
                    context.SaveChanges();
                }

                return Redirect("/Home/Index");
            }

            catch (Exception ex)
            {
                //ghi log
                return RedirectToAction("/Loi");
            }
        }

        //public ActionResult Payment(DateTime Ngaygiao, string MaKH, string TenKhach, string DiaChiKhach, string TrangThai)
        //{
        //    if (Session["dnhap"] != null)
        //    {
        //        HOADON model = new HOADON();
        //        model.MaKH = MaKH;
        //        model.TenKhach = TenKhach;
        //        model.DiaChiKhach = DiaChiKhach;
        //        model.TrangThai = TrangThai;
        //        model.NgayDat = DateTime.Now;
        //        model.NgayGiao = Ngaygiao;
        //        // var cart = (Cart)Session["CartSession"];
        //        try
        //        {
        //            var x = context.HOADON.Count();
        //            model.MaHD = x + 1;
        //            context.HOADON.Add(model);
        //            context.SaveChanges();
        //            var cart = (Cart)Session["CartSession"];
        //            foreach (var item in cart.Lines)
        //            {
        //                GIAODICH obj = new GIAODICH();
        //                obj.MaHD = model.MaHD;
        //                obj.MaQA = item.Sanpham.MaQA;

        //                obj.SoLuong = item.Quantity;

        //                context.GIAODICH.Add(obj);
        //                context.SaveChanges();

        //                //var giodn = context.GIOHANG.Where(m => m.MaKH == MaKH).FirstOrDefault();
        //                //if (giodn != null)
        //                //{
        //                //    context.GIOHANG.Remove(giodn);
        //                //    context.SaveChanges();
        //                //}

        //                //GIOHANG y = new GIOHANG();
        //                //y.MaKH = MaKH;
        //                //y.MaQA = item.Sanpham.MaQA;
        //                //y.SoLuong = item.Quantity;
        //                //context.GIOHANG.Add(y);
        //                //context.SaveChanges();

        //                SANPHAM sp = new SANPHAM();
        //                sp = context.SANPHAM.Find(obj.MaQA);
        //                if (sp.SoLuong > obj.SoLuong)
        //                {
        //                    sp.SoLuong = sp.SoLuong - obj.SoLuong;
        //                    context.SaveChanges();
        //                }
        //                else
        //                {
        //                    //context.GIOHANG.Remove(gi);
        //                    context.HOADON.Remove(model);
        //                    context.GIAODICH.Remove(obj);
        //                    context.SaveChanges();
        //                }
        //            }

        //            cart.Clear();
        //            Session["CartSession"] = cart;
        //            return Redirect("/Home/Index");
        //        }

        //        catch (Exception ex)
        //        {
        //            //ghi log
        //            return RedirectToAction("/Loi");
        //        }
        //    }
        //    else
        //    {
        //        return Redirect("/Account/DangNhap");
        //    }
        //}
        public ActionResult GioTam()
        {
            var dn = (TAIKHOAN)Session["dnhap"];
            var model1 = (from m in context.TAIKHOAN
                          join n in context.GIOHANG on m.Username equals n.MaKH
                          join k in context.SANPHAM on n.MaQA equals k.MaQA
                          join h in context.MATHANG on k.MaMH equals h.MaMH
                          join a in context.SIZE on k.MaSize equals a.MaSize
                          join b in context.MAU on k.MaMau equals b.MaMau
                          join c in context.ANH on h.MaMH equals c.MaMH
                          where m.Username == dn.Username && c.MaMau == b.MaMau
                          select new dschitietsanpham()
                          {
                              maqa = n.MaQA,
                              so = n.SoLuong,
                              gia = h.GiaBan.Value,
                              size = a.MaSize,
                              tenmau = b.TenMau,
                              url = c.UrlAnh,
                              tenh = h.TenMH

                          }
                         ).ToList();

            var giodn = context.GIOHANG.Where(m => m.MaKH == dn.Username).FirstOrDefault();

            var pro = model1.FirstOrDefault();

            var cart = (Cart)Session["CartSession"];
            cart = new Cart();
            if (giodn != null)
            {

                var product = context.SANPHAM.Find(pro.maqa);
                var sl = context.GIOHANG.Find(dn.Username, pro.maqa);
                cart.AddItem(product, sl.SoLuong);
                Session["CartSession"] = cart;
            }

            //  return View(model1);
            return View(cart);
        }

        //[HttpGet]
        //public ActionResult ThanhToan()
        //{
        //    var dn = (TAIKHOAN)Session["dnhap"];
        //    var model1 = (from m in context.TAIKHOAN
        //                  join n in context.GIOHANG on m.Username equals n.MaKH
        //                  join k in context.SANPHAM on n.MaQA equals k.MaQA
        //                  join h in context.MATHANG on k.MaMH equals h.MaMH
        //                  join a in context.SIZE on k.MaSize equals a.MaSize
        //                  join b in context.MAU on k.MaMau equals b.MaMau
        //                  join c in context.ANH on h.MaMH equals c.MaMH
        //                  where m.Username == dn.Username && c.MaMau == b.MaMau 
        //                  select new dschitietsanpham()
        //                  {
        //                      maqa = n.MaQA,
        //                      so = n.SoLuong,
        //                      gia = h.GiaBan.Value,
        //                      size = a.MaSize,
        //                      tenmau = b.TenMau,
        //                      url = c.UrlAnh,
        //                      tenh = h.TenMH

        //                  }
        //                 ).ToList();
        //    return View(model1);

        //}
        //[HttpPost]
        //public ActionResult ThanhToan(DateTime Ngaygiao, string MaKH, string TenKhach, string DiaChiKhach, string TrangThai)
        //{
        //    var dn = (TAIKHOAN)Session["dnhap"];
        //    HOADON model = new HOADON();
        //    model.NgayDat = DateTime.Now;
        //    model.MaKH = MaKH;
        //    model.NgayGiao = Ngaygiao;
        //    model.TrangThai = TrangThai;
        //    model.TenKhach = TenKhach;
        //    model.DiaChiKhach = DiaChiKhach;

        //    var x = context.HOADON.Count();
        //    model.MaHD = x + 1;
        //    context.HOADON.Add(model);
        //    context.SaveChanges();

        //    var model1 = (from m in context.TAIKHOAN
        //                  join n in context.GIOHANG on m.Username equals n.MaKH
        //                  join k in context.SANPHAM on n.MaQA equals k.MaQA
        //                  join h in context.MATHANG on k.MaMH equals h.MaMH
        //                  join a in context.SIZE on k.MaSize equals a.MaSize
        //                  join b in context.MAU on k.MaMau equals b.MaMau
        //                  join c in context.ANH on h.MaMH equals c.MaMH
        //                  where m.Username == dn.Username && c.MaMau == b.MaMau
        //                  select new dschitietsanpham()
        //                  {
        //                      maqa = n.MaQA,
        //                      so = n.SoLuong,
        //                      gia = h.GiaBan.Value,
        //                      size = a.MaSize,
        //                      tenmau = b.TenMau,
        //                      url = c.UrlAnh,
        //                      tenh = h.TenMH
        //                  }
        //                  ).ToList();


        //    foreach (var item in model1)
        //    {
        //        GIAODICH obj = new GIAODICH();
        //        obj.MaHD = model.MaHD;
        //        obj.MaQA = item.maqa.Value;
        //        obj.SoLuong = item.so.Value;

        //        context.GIAODICH.Add(obj);
        //        context.SaveChanges();
        //    }

        //    var model2 = (from m in context.TAIKHOAN
        //                  join n in context.GIOHANG on m.Username equals n.MaKH
        //                  join k in context.SANPHAM on n.MaQA equals k.MaQA

        //                  where m.Username == dn.Username
        //                  select new dsgio()
        //                  {
        //                      MaQa = n.MaQA,
        //                      so = n.SoLuong,
        //                      name = m.Username

        //                  }
        //               ).ToList();

        //    //foreach (var item in model2)
        //    //{
        //    //    GIOHANG g = context.GIOHANG.Find(item.name, item.MaQa);
        //    //    context.GIOHANG.Remove(g);
        //    //    context.SaveChanges();
        //    //}

        //    return Redirect("/Home/Index");
        //}

        public ActionResult XemDon(int? page)
        {
            var dn = (TAIKHOAN)Session["dnhap"];

            var model = (from m in context.GIAODICH
                         join n in context.HOADON on m.MaHD equals n.MaHD
                         join k in context.TAIKHOAN on n.MaKH equals k.Username
                         join a in context.SANPHAM on m.MaQA equals a.MaQA
                         join c in context.MAU on a.MaMau equals c.MaMau
                         join b in context.MATHANG on a.MaMH equals b.MaMH
                         join d in context.ANH on b.MaMH equals d.MaMH


                         where k.Username == dn.Username
                         select new xemgiohang()
                         {
                             MaQa = m.MaQA,
                             so = m.SoLuong,
                             Gia = b.GiaBan.Value,
                             size = a.MaSize,
                             tenmau = c.TenMau,
                             trangthai = n.TrangThai,
                             tenmh = b.TenMH,
                             url = d.UrlAnh,
                             Mahd = n.MaHD
                         }
                        ).OrderByDescending(m => m.Mahd).ToPagedList(page ?? 1, 3);
            return View(model);

        }

        public ActionResult DonHang()
        {
            var hOADON = context.HOADON.Include(m => m.TAIKHOAN).Include(m => m.GIAODICH);

            return View(hOADON.ToList());
        }
        public ActionResult Details(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var test = context.HOADON.Include(d => d.GIAODICH.Select(g => g.SANPHAM))
                .Include(d => d.TAIKHOAN)
                .Single(h => h.MaHD == id);

            if (test == null)
            {
                return HttpNotFound();
            }
            return View(test);
        }
        public ActionResult HuyDon(int? id)
        {

            var gd = context.GIAODICH.Where(s => s.MaHD == id).ToList();
            foreach (var item in gd)
            {
                context.GIAODICH.Remove(item);
                context.SaveChanges();
            }
            var hd = context.HOADON.Where(s => s.MaHD == id).ToList();
            foreach (var item in hd)
            {
                context.HOADON.Remove(item);
                context.SaveChanges();
            }
            return Redirect("~/Cart/DonHang");

        }



        public JsonResult GetUserAccount()
        {
            var dn = (TAIKHOAN)Session["dnhap"];

            //var tk = context.TAIKHOAN.Find(dn.Username);
            var acc = from tk in context.TAIKHOAN
                      where tk.Username == dn.Username
                      select new
                      {
                          tk.HoTen,
                          tk.Username,
                          tk.DiaChi,
                          tk.Email,
                          tk.SDT
                      };


            return Json(acc, JsonRequestBehavior.AllowGet);
        }

        public void RemoveCheckedItem(List<string> prodIdList)
        {
            var cart = (Cart)Session["CartSession"];
            //CheckoutProds = prodIdList;
            foreach (var prod in prodIdList)
            {
                var item = context.SANPHAM.Find(Convert.ToInt32(prod));
                //Console.WriteLine(item.MaQA);
                CheckoutProds.Add(prod);
                CheckoutProds.Add(cart.GetProductQuantity(item).ToString());
                cart.RemoveLine(item);
            }
        }

        public static void GioSessionVaoCSDL(string MaKhach, Cart cart)
        {
            //xoa gio trong csdl truoc
            var preCart = context.GIOHANG.Where(s => s.MaKH == MaKhach).ToList();
            foreach (var item in preCart)
            {
                context.GIOHANG.Remove(item);
                context.SaveChanges();
            }

            foreach (var item in cart.Lines)
            {
                GIOHANG gio = new GIOHANG();
                gio.MaKH = MaKhach;
                gio.MaQA = item.Sanpham.MaQA;
                gio.SoLuong = item.Quantity;
                context.GIOHANG.Add(gio);
                context.SaveChanges();
            }
        }

        public void GioCSDLVaoSession(string MaKhach, Cart cart)
        {
            var giodn = context.GIOHANG.Where(m => m.MaKH == MaKhach).ToList();

            if (giodn == null) return;

            foreach (var sp in giodn)
            {
                var product = context.SANPHAM.Find(sp.MaQA);
                cart.AddItem(product, sp.SoLuong);
            }
        }
    }
}