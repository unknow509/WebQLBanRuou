using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QLBRUOU.Models;
using PagedList;
using PagedList.Mvc;
using System.IO;

namespace QLBRUOU.Controllers
{
    public class LienheController : Controller
    {
        dbQLBANRUOUDataContext data = new dbQLBANRUOUDataContext();
        // GET: Lienhe
        public ActionResult Index()
        {
            return View();
        }
       
    }
}