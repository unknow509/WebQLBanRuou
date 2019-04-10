using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QLBRUOU.Models
{
    public class Giohang
    {
        dbQLBANRUOUDataContext data = new dbQLBANRUOUDataContext();

        public int iMaruou { set; get; }
        public string sTenruou { set; get; }
        public string sAnhruou { set; get; }
        public Double dDongia { set; get; }
        public int iSoluong { set; get; }

        public Double dThanhtien
        {
            get { return iSoluong * dDongia; }
        }
        public Giohang(int Maruou)
        {
            iMaruou = Maruou;
            RUOU ruou = data.RUOUs.Single(t => t.MaRuou == iMaruou);
            sTenruou = ruou.TenRuou;
            sAnhruou = ruou.AnhRuou;
            dDongia = double.Parse(ruou.Giaban.ToString());
            iSoluong = 1;
        }
    }
}