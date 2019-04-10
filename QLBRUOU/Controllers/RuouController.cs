using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QLBRUOU.Models;

using PagedList;
using PagedList.Mvc;

namespace QLBANRUOU.Controllers
{
    public class RuouController : Controller
    {
        dbQLBANRUOUDataContext data = new dbQLBANRUOUDataContext();

        private List<RUOU> Layruou()
        {
            return data.RUOUs.OrderByDescending(a => a.NgayCapNhat).ToList();
        }
        public ActionResult Index(int? page)
        {
            int pageSize = 8;
            int pageNum = (page ?? 1);

            var ruoumoi = Layruou();
            return View(ruoumoi.ToPagedList(pageNum, pageSize));
        }
        public ActionResult Loairuou()
        {
            var loairuou = from lr in data.PHANLOAIs select lr;
            return PartialView(loairuou);
        }

        public ActionResult xuatxu()
        {
            var xuatxu = from xx in data.XUATXUs select xx;
            return PartialView(xuatxu);
        }
        public ActionResult nhacungcap()
        {
            var nhacungcap = from ncc in data.NHACUNGCAPs select ncc;
            return PartialView(nhacungcap);
        }

        public ActionResult SPTheoNCC(int id)
        {
            var ncc = from r in data.RUOUs where r.MaNCC == id select r;
            return View(ncc);
        }

        public ActionResult SPTheoXX(int id)
        {
            var nxx = from x in data.RUOUs where x.MaXXu == id select x;
            return View(nxx);
        }

        public ActionResult SPTheoLoai(int id)
        {
            var loai = from l in data.RUOUs where l.MaLoaiRuou == id select l;
            return View(loai);
        }

        public ActionResult ChiTietSP(int id)
        {
            RUOU ct = data.RUOUs.SingleOrDefault(t => t.MaRuou == id);
            return View(ct);
        }
    }
}