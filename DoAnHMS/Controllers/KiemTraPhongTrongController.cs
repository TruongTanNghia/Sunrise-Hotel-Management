using DoAnHMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnHMS.Controllers
{
    public class KiemTraPhongTrongController : BaseController
    {
        DoAnHMSEntities db = new DoAnHMSEntities();

        [HttpGet]
        public ActionResult Index()
        {
            var chiTietPhieuThuePhong = db.CTPhieuThuePhongs.ToList();
            return View(chiTietPhieuThuePhong);
        }

        [HttpPost]
        public ActionResult Index(string ngayDen, string ngayDi)
        {
            DateTime dt1 = Convert.ToDateTime(Convert.ToDateTime(ngayDen).ToString("dd/MM/yyyy"));
            DateTime dt2 = Convert.ToDateTime(Convert.ToDateTime(ngayDi).ToString("dd/MM/yyyy"));
            var chiTietPhieuThuePhong = db.CTPhieuThuePhongs.Where(n => n.PhieuThuePhong.ngayThue <= dt1 && n.PhieuThuePhong.ngayTra >= dt2).ToList();
            return View(chiTietPhieuThuePhong);
        }
    }
}