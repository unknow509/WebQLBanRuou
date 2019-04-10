using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QLBRUOU.Models;

namespace QLBANRUOU.Controllers
{
    public class GiohangController : Controller
    {
        // GET: Giohang
        public ActionResult Index()
        {
            return View();
        }
        dbQLBANRUOUDataContext data = new dbQLBANRUOUDataContext();

        // GET: Giohang
        public List<Giohang> Laygiohang() //get Giohang and assign new if it's empty
        {
            List<Giohang> lstGiohang = Session["Giohang"] as List<Giohang>;
            if (lstGiohang == null)
            {
                lstGiohang = new List<Giohang>();
                Session["Giohang"] = lstGiohang;
            }
            return lstGiohang;
        }

        public ActionResult ThemGiohang(int id)
        {
            List<Giohang> lstGiohang = Laygiohang();
            Giohang sanpham = lstGiohang.Find(t => t.iMaruou == id);
            if (sanpham == null)
            {
                sanpham = new Giohang(id);
                lstGiohang.Add(sanpham);
                return Redirect(Url.Action("Index", "Ruou"));
            }
            else
            {
                sanpham.iSoluong++;
                return Redirect(Url.Action("Index", "Ruou"));
            }
        }

        private int Tongsoluong()
        {
            int iTongsoluong = 0;
            List<Giohang> lstGiohang = Session["Giohang"] as List<Giohang>;
            if (lstGiohang != null)
            {
                iTongsoluong = lstGiohang.Sum(t => t.iSoluong);
            }
            return iTongsoluong;
        }

        private double Tongtien()
        {
            double iTongtien = 0;
            List<Giohang> lstGiohang = Session["Giohang"] as List<Giohang>;
            if (lstGiohang != null)
            {
                iTongtien = lstGiohang.Sum(t => t.dThanhtien);
            }
            return iTongtien;
        }

        public ActionResult Giohang()
        {
            List<Giohang> lstGiohang = Laygiohang();
            if (lstGiohang.Count == 0)
            {
                return RedirectToAction("Index", "Ruou");
            }
            ViewBag.Tongsoluong = Tongsoluong();
            ViewBag.Tongtien = Tongtien();
            return View(lstGiohang);
        }

        public ActionResult GiohangPartial()
        {
            ViewBag.Tongsoluong = Tongsoluong();
            ViewBag.Tongtien = Tongtien();
            return PartialView();
        }

        public ActionResult XoaGiohang(int iMaSP)
        {
            List<Giohang> lstGiohang = Laygiohang();
            Giohang sanpham = lstGiohang.SingleOrDefault(t => t.iMaruou == iMaSP);
            if (sanpham != null)
            {
                lstGiohang.RemoveAll(t => t.iMaruou == iMaSP);
                return RedirectToAction("Giohang");
            }
            if (lstGiohang.Count == 0)
            {
                return RedirectToAction("Index", "Ruou");
            }
            return RedirectToAction("Giohang");
        }

        public ActionResult XoaTatcaGiohang()
        {
            List<Giohang> lstGiohang = Laygiohang();
            lstGiohang.Clear();
            return RedirectToAction("Index", "Ruou");
        }
        [HttpGet]
        public ActionResult Dathang()
        {
            if (Session["Taikhoan"] == null || Session["Taikhoan"].ToString() == "")
            {
                return RedirectToAction("Dangnhap", "Nguoidung");
            }
            if (Session["Giohang"] == null)
            {
                return RedirectToAction("Index", "Ruou");
            }
            List<Giohang> lstGiohang = Laygiohang();
            ViewBag.Tongsoluong = Tongsoluong();
            ViewBag.Tongtien = Tongtien();
            return View(lstGiohang);
        }
        [HttpPost]
        public ActionResult Dathang(FormCollection collection)
        {
            DONDATHANG ddh = new DONDATHANG();
            KHACHHANG kh = (KHACHHANG)Session["Taikhoan"];
            ddh.MaKH = kh.MaKH;
            ddh.Ngaydat = DateTime.Now;
            ddh.Ngaygiao = DateTime.Now.AddHours(72);
            ddh.Tinhtranggiaohang = false;
            ddh.Dathanhtoan = false;
            data.DONDATHANGs.InsertOnSubmit(ddh);
            data.SubmitChanges();

            List<Giohang> lstGiohang = Laygiohang();
            foreach (var item in lstGiohang)
            {
                CHITIETDONTHANG ctdh = new CHITIETDONTHANG();
                ctdh.MaDH = ddh.MaDH;
                ctdh.Maruou = item.iMaruou;
                ctdh.Soluong = item.iSoluong;
                ctdh.Dongia = (decimal)item.dDongia;
                data.CHITIETDONTHANGs.InsertOnSubmit(ctdh);
            }
            data.SubmitChanges();
            Session["Giohang"] = null;
            return RedirectToAction("Xacnhandonhang", "Giohang");
        }

        public ActionResult Xacnhandonhang()
        {
            return View();
        }

        public ActionResult CapnhatGiohang(int iMaSp, FormCollection collection)
        {
            List<Giohang> lstGiohang = Laygiohang();
            foreach (var item in lstGiohang)
            {
                if (item.iMaruou == iMaSp)
                {
                    item.iSoluong = Convert.ToInt32(collection["txtSoluong"]);
                    break;
                }
            }
            return RedirectToAction("Giohang", "Giohang");
        }
    }
}