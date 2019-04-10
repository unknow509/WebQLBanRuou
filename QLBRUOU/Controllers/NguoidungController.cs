using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QLBRUOU.Models;

namespace QLBANRUOU.Controllers
{
    public class NguoidungController : Controller
    {
        dbQLBANRUOUDataContext data = new dbQLBANRUOUDataContext();
        // GET: Nguoidung
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Dangky()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Dangky(FormCollection collection, KHACHHANG kh)
        {
            var hoten = collection["HotenKH"];
            var tendn = collection["TenDN"];
            var matkhau = collection["Matkhau"];
            var matkhaunhaplai = collection["MatkhauNhaplai"];
            var diachi = collection["Diachi"];
            var email = collection["Email"];
            var dienthoai = collection["Dienthoai"];
            var ngaysinh = string.Format("{0:MM/dd/yyyy}", collection["Ngaysinh"]);

            if (string.IsNullOrEmpty(hoten))
            {
                ViewData["Loi1"] = " Họ tên khách hàng không được để trống";
            }
            if (string.IsNullOrEmpty(tendn))
            {
                ViewData["loi2"] = " Tên đăng nhập không được để trống";
            }
            if (string.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi3"] = " Mật khẩu không được để trống";
            }
            if (string.IsNullOrEmpty(matkhaunhaplai))
            {
                ViewData["Loi4"] = " Mật khẩu nhập lại không được để trống";
            }
            if (string.IsNullOrEmpty(diachi))
            {
                ViewData["Loi5"] = " Địa chỉ không được để trống";
            }
            if (string.IsNullOrEmpty(email))
            {
                ViewData["Loi6"] = " Email không được để trống";
            }
            if (string.IsNullOrEmpty(dienthoai))
            {
                ViewData["Loi7"] = " Điện thoại không được để trống";
            }
            else
            {
                kh.HoTen = hoten;
                kh.TaiKhoan = tendn;
                kh.MatKhau = matkhau;
                kh.Email = email;
                kh.DiaChiKH = diachi;
                kh.DienThoaiKH = dienthoai;
                kh.NgaySinh = DateTime.Parse(ngaysinh);
                data.KHACHHANGs.InsertOnSubmit(kh);
                data.SubmitChanges();
                return RedirectToAction("Dangnhap"); 
            }
            return View();
        }

        [HttpGet]
        public ActionResult Dangnhap()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Dangnhap(FormCollection collection)
        {
            var tendn = collection["TenDN"];
            var matkhau = collection["Matkhau"];
            if (string.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Tên đăng nhập không được để trống";
            }
            else if (string.IsNullOrEmpty(matkhau))
            {
                ViewData["loi2"] = "Mật khẩu không được để trống";
            }
            else
            {
                KHACHHANG kh = data.KHACHHANGs.SingleOrDefault(t => t.TaiKhoan == tendn && t.MatKhau == matkhau);
                if (kh != null)
                {
                    ViewBag.Thongbao = "Đăng nhập thành công";
                    Session["Taikhoan"] = kh;
                    Session["HoTenUser"] = kh.HoTen;
                    return RedirectToAction("Index", "Ruou");
                }
                else
                {
                    ViewBag.Thongbao = "Sai tên đăng nhập hoặc mật khẩu";
                }
            }
            return View();
        }

        public ActionResult Dangxuat()
        {
            Session.Remove("Taikhoan");
            Session.Remove("HoTenUser");
            return RedirectToAction("Index", "Ruou");
        }
    }
}