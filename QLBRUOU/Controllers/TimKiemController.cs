using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QLBRUOU.Models;
using PagedList.Mvc;
using PagedList;

namespace QLBRUOU.Controllers
{

    public class TimKiemController : Controller
    {
        dbQLBANRUOUDataContext db = new dbQLBANRUOUDataContext();
        // GET: TimKiem
        [HttpPost]
        public ActionResult Ketquatimkiem(FormCollection f, int? page)
        {
            string sTuKhoa = f["txtTimKiem"].ToString();
            ViewBag.TuKhoa = sTuKhoa;
            List<RUOU> lstKQTK = db.RUOUs.Where(n => n.TenRuou.Contains(sTuKhoa)).ToList();
                int pageNumber = (page ?? 1);
            int pageSize = 7;           
            if (lstKQTK.Count == null)
            {
                ViewBag.ThongBao = "không tìm thấy sản phẩm nào!!!";
                return View(db.RUOUs.ToList().OrderBy(t => t.MaRuou).ToPagedList(pageNumber, pageSize));
            }
            ViewBag.ThongBao = "Đã tìm thấy" + lstKQTK.Count + "kết quả";
            return View(lstKQTK.OrderBy(n => n.TenRuou).ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult Ketquatimkiem(int? page, string sTuKhoa)
        {
            ViewBag.TuKhoa = sTuKhoa;
            List<RUOU> lstKQTK = db.RUOUs.Where(n => n.TenRuou.Contains(sTuKhoa)).ToList();
            int pageNumber = (page ?? 1);
            int pageSize = 9;
            if (lstKQTK.Count == null)
            {
                ViewBag.ThongBao = "không tìm thấy sản phẩm nào!!!";
                return View(db.RUOUs.ToList().OrderBy(t => t.MaRuou).ToPagedList(pageNumber, pageSize));
            }
            ViewBag.ThongBao = "Đã tìm thấy " + lstKQTK.Count + " kết quả";
            return View(lstKQTK.OrderBy(n => n.TenRuou).ToPagedList(pageNumber, pageSize));
        }
    }
}