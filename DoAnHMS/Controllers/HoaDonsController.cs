using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DoAnHMS.Common;
using DoAnHMS.Models;

namespace DoAnHMS.Controllers
{
    public class HoaDonsController : BaseController
    {
        private DoAnHMSEntities db = new DoAnHMSEntities();

        string LayMaHD()
        {
            string maMax;
            int maHD;
            maMax = db.HoaDons.ToList().Select(n => n.maHD).Max();
            if (maMax != null)
            {
                maHD = int.Parse(maMax.Substring(2)) + 1;
            }
            else
            {
                maHD = 01;
            }
            string HD = String.Concat("000", maHD.ToString());
            return "HD" + HD.Substring(maHD.ToString().Length - 1);
        }

        // GET: HoaDons
        [HasCredential(IDQuyen = "LAPHOADON")]
        public ActionResult Index()
        {
            var hoaDons = db.HoaDons.Include(h => h.NhanVien).Include(h => h.PhieuThuePhong);
            //var hoaDons = db.HoaDons.Include(h => h.NhanVien).Include(h => h.PhieuThuePhong);
            return View(hoaDons.ToList());
        }

        // GET: HoaDons/Details/5
        [HasCredential(IDQuyen = "LAPHOADON")]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var hoaDon = db.HoaDons.SingleOrDefault(n => n.maHD == id);
            if (hoaDon == null)
            {
                return HttpNotFound();
            }
            return View(hoaDon);
        }

        // GET: HoaDons/Create
        [HasCredential(IDQuyen = "LAPHOADON")]
        public ActionResult Create()
        {
            ViewBag.maHD = LayMaHD();
            var session = (UserLogin)HttpContext.Session[CommonConstants.USER_SESSION];
            var maNV = db.QuanTris.Where(x => x.username == session.UserName).Select(x => x.maNV).SingleOrDefault();
            ViewBag.maNV = new SelectList(db.NhanViens, "maNV", "tenNV", maNV);
            var hoaDons = db.HoaDons.Select(x => x.maPTP).ToList();
            var phieuThuePhongs = db.PhieuThuePhongs.Where(x => !hoaDons.Contains(x.maPTP)).ToList();
            //var phi = db.PhieuThuePhongs.Where(n=>n.maPTP != )
            //var phieuThuePhongs = db.PhieuThuePhongs.ToList();
            ViewBag.maPTP = new SelectList(phieuThuePhongs, "maPTP", "maPTP");
            return View();
        }

        // POST: HoaDons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasCredential(IDQuyen = "LAPHOADON")]
        public ActionResult Create([Bind(Include = "maHD,ngayTT,maPTP,maNV")] HoaDon hoaDon)
        {
            var session = (UserLogin)HttpContext.Session[CommonConstants.USER_SESSION];
            var maNV = db.QuanTris.Where(x => x.username == session.UserName).Select(x => x.maNV).SingleOrDefault();
            if (ModelState.IsValid)
            {
                hoaDon.maNV = maNV;
                decimal tongTien = 0;
                decimal tienDV = 0;
                decimal tienPhong = 0;
                decimal tienCoc = 0;
                List<string> maPs = new List<string>();
                PhieuThuePhong phieuThuePhong = db.PhieuThuePhongs.Find(hoaDon.maPTP);
                var suDungDV = db.CTPhieuThuePhongs.Where(x => x.maPTP == phieuThuePhong.maPTP).ToList();
                foreach (var item in suDungDV)
                {
                    tienDV += item.DichVu.gia * item.soLuong;
                    if (!maPs.Contains(item.maP))
                    {
                        maPs.Add(item.maP);
                    }
                }
                foreach (var maP in maPs)
                {
                    var phong = db.Phongs.Where(x => x.maP == maP).SingleOrDefault();
                    tienPhong += phong.LoaiPhong.donGia;
                }
                TimeSpan difference = phieuThuePhong.ngayTra - phieuThuePhong.ngayThue;
                decimal days = (decimal)(difference.TotalDays + 1);
                tienPhong *= days;
                if (phieuThuePhong.maPDP != null)
                {
                    var phongDats = db.CTPhieuDatPhongs.Where(x => x.maPDP == phieuThuePhong.maPDP).ToList();
                    foreach (var item in phongDats)
                    {
                        if (maPs.Contains(item.maP))
                        {
                            tienCoc += item.tienCoc;
                        }
                    }
                }
                tongTien = tienDV + tienPhong - tienCoc;
                hoaDon.tongTien = tongTien;
                db.HoaDons.Add(hoaDon);
                if (DateTime.Now.Date <= phieuThuePhong.ngayTra.Date)
                {
                    foreach (var maP in maPs)
                    {
                        Phong phong = db.Phongs.Find(maP);
                        phong.tinhTrang = "Trống";
                        db.Entry(phong).State = EntityState.Modified;

                    }
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.maHD = LayMaHD();
            ViewBag.maNV = new SelectList(db.NhanViens, "maNV", "tenNV", maNV);
            ViewBag.maPTP = new SelectList(db.PhieuThuePhongs, "maPTP", "maPTP", hoaDon.maPTP);
            return View(hoaDon);
        }

        // GET: HoaDons/Delete/5
        [HasCredential(IDQuyen = "LAPHOADON")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var hoaDon = db.HoaDons.SingleOrDefault(n => n.maHD == id);
            if (hoaDon == null)
            {
                return HttpNotFound();
            }
            return View(hoaDon);
        }

        // POST: HoaDons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [HasCredential(IDQuyen = "LAPHOADON")]
        public ActionResult DeleteConfirmed(string id)
        {
            var hoaDon = db.HoaDons.SingleOrDefault(n => n.maHD == id);
            var phieuThuePhong = db.PhieuThuePhongs.SingleOrDefault(n => n.maPTP == hoaDon.maPTP);
            if (hoaDon.ngayTT.Date <= DateTime.Now.Date)
            {
                var cTPhieuThuePhong = db.CTPhieuThuePhongs.Where(x => x.maPTP == hoaDon.maPTP).ToList();
                foreach (var item in cTPhieuThuePhong)
                {
                    Phong phong = db.Phongs.Find(item.maP);
                    phong.tinhTrang = "Đang sử dụng";
                    db.Entry(phong).State = EntityState.Modified;
                }
            }
            db.HoaDons.Remove(hoaDon);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
