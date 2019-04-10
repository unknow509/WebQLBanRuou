using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QLBRUOU.Models;
using PagedList;
using PagedList.Mvc;
using System.IO;
using System.Text.RegularExpressions;

namespace QLBANRUOU.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        dbQLBANRUOUDataContext data = new dbQLBANRUOUDataContext();

        // GET: Admin
        public ActionResult Index()
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
                return RedirectToAction("Login");
            return View();
        }

        public ActionResult Ruou(int? page)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
                return RedirectToAction("Login");
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            return View(data.RUOUs.ToList().OrderBy(t => t.MaRuou).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Login(FormCollection collection)
        {
            dbQLBANRUOUDataContext data = new dbQLBANRUOUDataContext();

            var tendn = collection["form-username"];
            var matkhau = collection["form-password"];
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Phải nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Phải nhập mật khẩu";
            }
            else
            {
                ADMIN ad = data.ADMINs.SingleOrDefault(t => t.UserAdmin == tendn && t.PassAdmin == matkhau);
                if (ad != null)
                {
                    Session["Taikhoanadmin"] = ad;
                    Session["HoTenAdmin"] = ad.Hoten;
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";
                }
            }
            return View();
        }

        public ActionResult Logout()
        {
            Session.Remove("Taikhoanadmin");
            Session.Remove("HoTenAdmin");
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult themmoiruou()
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
                return RedirectToAction("Login");
            ViewBag.MaLoairuou = new SelectList(data.PHANLOAIs.ToList().OrderBy(t => t.Loairuou), "MaLoairuou", "Loairuou");
            ViewBag.MaXXu = new SelectList(data.XUATXUs.ToList().OrderBy(t => t.TenXXu), "MaXXu", "TenXXu");
            ViewBag.MaNCC = new SelectList(data.NHACUNGCAPs.ToList().OrderBy(t => t.TenNCC), "MaNCC", "TenNCC");
            return View();
        }

        [HttpPost]
        public ActionResult themmoiruou(RUOU ruou, HttpPostedFileBase fileUpload, FormCollection collection)
        {
            var filename = Path.GetFileName(fileUpload.FileName);
            var path = Path.Combine(Server.MapPath("~/Images"), filename);
            if (System.IO.File.Exists(path))
            {
                ViewBag.Thongbao = "Hình ảnh đã tồn tại";
            }
            else
            {
                fileUpload.SaveAs(path);
            }

            var tenruou = collection["TenRuou"];
            var giaban = collection["Giaban"];
            var mota = collection["Mota"];
            var ngaycapnhat = collection["sel_date"];
            var slton = collection["SoLuongTon"];
            var malr = collection["MaLoaiRuou"];
            var maxx = collection["MaXXu"];
            var mancc = collection["MaNCC"];

            ruou.TenRuou = tenruou;
            ruou.AnhRuou = filename;
            ruou.Giaban = Decimal.Parse(giaban);
            ruou.Mota = Regex.Replace(mota, "<.*?>", String.Empty);
            ruou.NgayCapNhat = Convert.ToDateTime(ngaycapnhat);
            ruou.SoLuongTon = Int32.Parse(slton);
            ruou.MaLoaiRuou = Int32.Parse(malr);
            ruou.MaNCC = Int32.Parse(mancc);
            ruou.MaXXu = Int32.Parse(maxx);
            data.RUOUs.InsertOnSubmit(ruou);
            data.SubmitChanges();

            ViewBag.MaLoairuou = new SelectList(data.PHANLOAIs.ToList().OrderBy(t => t.Loairuou), "MaLoairuou", "Loairuou");
            ViewBag.MaXXu = new SelectList(data.XUATXUs.ToList().OrderBy(t => t.TenXXu), "MaXXu", "TenXXu");
            ViewBag.MaNCC = new SelectList(data.NHACUNGCAPs.ToList().OrderBy(t => t.TenNCC), "MaNCC", "TenNCC");
            return RedirectToAction("Ruou");
        }
        public ActionResult Chitietruou(int id)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
                return RedirectToAction("Login");
            RUOU ruou = data.RUOUs.SingleOrDefault(n => n.MaRuou == id);
            ViewBag.MaRuou = ruou.MaRuou;
            if(ruou == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(ruou);
        }
        [HttpGet]
        public ActionResult xoaruou(int id)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
                return RedirectToAction("Login");
            RUOU ruou = data.RUOUs.SingleOrDefault(n => n.MaRuou == id);
            ViewBag.MaRuou = ruou.MaRuou;
            if (ruou == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(ruou);
        }

        [HttpPost,ActionName("Xoaruou")]
        public ActionResult Xacnhanxoa(int id)
        {
            RUOU ruou = data.RUOUs.SingleOrDefault(n => n.MaRuou == id);
            ViewBag.MaRuou = ruou.MaRuou;
            if (ruou == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            
            var filePath = Path.Combine(Server.MapPath("~/Images/"), ruou.AnhRuou);
            try
            {
                data.RUOUs.DeleteOnSubmit(ruou);
                data.SubmitChanges();
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            catch (Exception)
            {
                ViewBag.ErRemove = "Bạn phải xóa các màu của sản phẩm";
            }

            return RedirectToAction("Ruou");
        }

        [HttpGet]
        public ActionResult Suaruou(int id)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
                return RedirectToAction("Login");
            RUOU ruou = data.RUOUs.SingleOrDefault(n => n.MaRuou == id);
            ViewBag.MaRuou = ruou.MaRuou;
            ViewData["Image"] = ruou.AnhRuou;
            if (ruou == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.MaLoairuou = new SelectList(data.PHANLOAIs.ToList().OrderBy(t => t.Loairuou), "MaLoaiRuou", "Loairuou");
            ViewBag.MaXXu = new SelectList(data.XUATXUs.ToList().OrderBy(t => t.TenXXu), "MaXXu", "TenXXu");
            ViewBag.MaNCC = new SelectList(data.NHACUNGCAPs.ToList().OrderBy(t => t.TenNCC), "MaNCC", "TenNCC");
            return View(ruou);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Suaruou(RUOU ruou, HttpPostedFileBase NewImageP, FormCollection col)
        {
            ViewBag.MaLoairuou = new SelectList(data.PHANLOAIs.ToList().OrderBy(t => t.Loairuou), "MaLoaiRuou", "Loairuou");
            ViewBag.MaXXu = new SelectList(data.XUATXUs.ToList().OrderBy(t => t.TenXXu), "MaXXu", "TenXXu");
            ViewBag.MaNCC = new SelectList(data.NHACUNGCAPs.ToList().OrderBy(t => t.TenNCC), "MaNCC", "TenNCC");

            var image = col["ImageP"];
            RUOU r = data.RUOUs.First(n => n.MaRuou == ruou.MaRuou);

            if (ModelState.IsValid)
            {
                if (NewImageP != null)
                {
                    if (Path.GetExtension(NewImageP.FileName).ToLower() == ".jpg"
                        || Path.GetExtension(NewImageP.FileName).ToLower() == ".png"
                        || Path.GetExtension(NewImageP.FileName).ToLower() == ".gif"
                        || Path.GetExtension(NewImageP.FileName).ToLower() == ".jpeg"
                        )
                    {
                        var filename = Path.GetFileName(NewImageP.FileName);
                        var path = Path.Combine(Server.MapPath("~/Images/"), filename);
                        if (System.IO.File.Exists(path))
                        {
                            ViewBag.Thongbao = "Tên ảnh đã tồn tại";
                        }
                        else
                        {
                            var newImageP = NewImageP.FileName;
                            NewImageP.SaveAs(path);
                            r.AnhRuou = newImageP;
                            r.NgayCapNhat = DateTime.Now;
                            UpdateModel(r);
                            data.SubmitChanges();
                            return RedirectToAction("Ruou");
                        }
                    }
                    else
                    {
                        ViewBag.Thongbao = "Hãy chọn một ảnh";
                    }
                }
                else
                {
                    r.AnhRuou = image;
                    r.NgayCapNhat = DateTime.Now;
                    UpdateModel(r);
                    data.SubmitChanges();
                    return RedirectToAction("Ruou");
                }
            }
            return this.Suaruou(r.MaRuou);
        }
        //----------------------------------------------Kết thúc quản lý rượu----------------------------------------------------
        public ActionResult Nhacungcap(int? page)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
                return RedirectToAction("Login");
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            return View(data.NHACUNGCAPs.ToList().OrderBy(t => t.MaNCC).ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult themmoincc()
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
                return RedirectToAction("Login");
            ViewBag.MaNCC = new SelectList(data.NHACUNGCAPs.ToList().OrderBy(t => t.TenNCC), "MaNCC", "TenNCC");
            return View();
        }

        [HttpPost]
        public ActionResult themmoincc(NHACUNGCAP ncc, FormCollection collection)
        {
 
            var tenncc = collection["TenNCC"];
            var diachi = collection["DiaChi"];
            var dienthoai = collection["DienThoai"];

            ncc.TenNCC = tenncc;
            ncc.DiaChi = diachi;
            ncc.DienThoai = dienthoai;
            data.NHACUNGCAPs.InsertOnSubmit(ncc);
            data.SubmitChanges();

            return RedirectToAction("Nhacungcap", "Admin");
        }
        [HttpGet]
        public ActionResult xoancc(int id)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
                return RedirectToAction("Login");
            NHACUNGCAP ncc = data.NHACUNGCAPs.SingleOrDefault(n => n.MaNCC == id);
            ViewBag.MaNCC = ncc.MaNCC;
            if (ncc == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(ncc);
        }

        [HttpPost, ActionName("Xoancc")]
        public ActionResult Xacnhanxoancc(int id)
        {
            NHACUNGCAP ncc = data.NHACUNGCAPs.SingleOrDefault(n => n.MaNCC == id);
            ViewBag.MaNCC = ncc.MaNCC;
            if (ncc == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            data.NHACUNGCAPs.DeleteOnSubmit(ncc);
            data.SubmitChanges();
            return RedirectToAction("Nhacungcap","Admin");
        }

        [HttpGet]
        public ActionResult Suancc(int id)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
                return RedirectToAction("Login");
            NHACUNGCAP ncc = data.NHACUNGCAPs.SingleOrDefault(n => n.MaNCC == id);
            if (ncc == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            
            return View(ncc);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Suancc(NHACUNGCAP ncc, FormCollection col)
        {
            NHACUNGCAP nhacc = data.NHACUNGCAPs.SingleOrDefault(n => n.MaNCC == ncc.MaNCC);
            nhacc.TenNCC = col["TenNCC"];
            nhacc.DiaChi = col["DiaChi"];
            nhacc.DienThoai = col["DienThoai"];

            UpdateModel(ncc);
            data.SubmitChanges();
            return RedirectToAction("Nhacungcap");
        }
        //---------------------------------------Kết thúc quản lí nhà cung cấp-----------------------------------------------
        public ActionResult Nhaxuatxu(int? page)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
                return RedirectToAction("Login");
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            return View(data.XUATXUs.ToList().OrderBy(t => t.MaXXu).ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult themmoinxx()
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
                return RedirectToAction("Login");
            ViewBag.MaXXu = new SelectList(data.XUATXUs.ToList().OrderBy(t => t.TenXXu), "MaXXu", "TenXXu");
            return View();
        }

        [HttpPost]
        public ActionResult themmoinxx(XUATXU xx, FormCollection collection)
        {
            var tennxx = collection["TenXXu"];
            xx.TenXXu = tennxx;
            data.XUATXUs.InsertOnSubmit(xx);
            data.SubmitChanges();

            return RedirectToAction("Nhaxuatxu", "Admin");
        }
        [HttpGet]
        public ActionResult xoanxx(int id)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
                return RedirectToAction("Login");
            XUATXU nxx = data.XUATXUs.SingleOrDefault(n => n.MaXXu == id);
            ViewBag.MaXXu = nxx.MaXXu;
            if (nxx == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View();
        }

        [HttpPost, ActionName("Xoanxx")]
        public ActionResult Xacnhanxoanxx(int id)
        {
            XUATXU nhaxuatxu = data.XUATXUs.SingleOrDefault(n => n.MaXXu == id);
            ViewBag.MaXXu = nhaxuatxu.MaXXu;
            if (nhaxuatxu == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            data.XUATXUs.DeleteOnSubmit(nhaxuatxu);
            data.SubmitChanges();
            return RedirectToAction("Nhaxuatxu", "Admin");
        }
        [HttpGet]
        public ActionResult Suaxx(int id)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
                return RedirectToAction("Login");
            XUATXU xx = data.XUATXUs.SingleOrDefault(n => n.MaXXu == id);
            if (xx == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            return View(xx);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Suaxx(XUATXU xx, FormCollection col)
        {
            XUATXU nhaxx = data.XUATXUs.SingleOrDefault(n => n.MaXXu == xx.MaXXu);
            nhaxx.TenXXu = col["TenXX"];
            UpdateModel(nhaxx);
            data.SubmitChanges();
            return RedirectToAction("Nhaxuatxu");
        }
        //---------------------------------------Kết thúc quản lí nhà xuất xứ-----------------------------------------------
        public ActionResult Phanloai(int? page)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
                return RedirectToAction("Login");
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            return View(data.PHANLOAIs.ToList().OrderBy(t => t.MaLoaiRuou).ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult themmoipl()
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
                return RedirectToAction("Login");
            ViewBag.MaPL = new SelectList(data.PHANLOAIs.ToList().OrderBy(t => t.Loairuou), "MaPL", "Loairuou");
            return View();
        }

        [HttpPost]
        public ActionResult themmoipl(PHANLOAI pl, FormCollection collection)
        {
            var loairuou = collection["Loairuou"];
            pl.Loairuou = loairuou;
            data.PHANLOAIs.InsertOnSubmit(pl);
            data.SubmitChanges();

            return RedirectToAction("phanloai", "Admin");
        }
        [HttpGet]
        public ActionResult xoapl(int id)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
                return RedirectToAction("Login");
            PHANLOAI pl = data.PHANLOAIs.SingleOrDefault(n => n.MaLoaiRuou == id);
            ViewBag.MaPL = pl.MaLoaiRuou;
            if (pl == null)
            {
                Response.StatusCode = 404;
                return null;
            }
             return View();
        }

        [HttpPost, ActionName("Xoapl")]
        public ActionResult Xacnhanxoapl(int id)
        {
            PHANLOAI phanloai = data.PHANLOAIs.SingleOrDefault(n => n.MaLoaiRuou == id);
            ViewBag.MaPL = phanloai.MaLoaiRuou;
            if (phanloai == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            data.PHANLOAIs.DeleteOnSubmit(phanloai);
            data.SubmitChanges();
            return RedirectToAction("phanloai", "Admin");
        }
        [HttpGet]
        public ActionResult Suapl(int id)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
                return RedirectToAction("Login");
            PHANLOAI pl = data.PHANLOAIs.SingleOrDefault(n => n.MaLoaiRuou == id);
            if (pl == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            return View(pl);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Suapl(PHANLOAI pl, FormCollection col)
        {
            PHANLOAI phloai = data.PHANLOAIs.SingleOrDefault(n => n.MaLoaiRuou == pl.MaLoaiRuou);
            phloai.Loairuou = col["Loairuou"];
            UpdateModel(phloai);
            data.SubmitChanges();
            return RedirectToAction("Phanloai");
        }
        //---------------------------------------Kết thúc quản lí phân loại-----------------------------------------------
        public ActionResult QLNguoidung(int? page)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
                return RedirectToAction("Login");
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            return View(data.KHACHHANGs.ToList().OrderBy(t => t.MaKH).ToPagedList(pageNumber, pageSize));
        }
        //---------------------------------------Kết thúc quản lí khách hàng-----------------------------------------------
    }
}