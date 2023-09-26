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
    public class PhieuDatPhongsController : BaseController
    {
        private readonly DoAnHMSEntities db = new DoAnHMSEntities();

        string LayMaPDP()
        {
            var maMax = db.PhieuDatPhongs.ToList().Select(n => n.maPDP).Max();
            int maPDP = int.Parse(maMax.Substring(3)) + 1;
            string PDP = String.Concat("000", maPDP.ToString());
            return "PDP" + PDP.Substring(maPDP.ToString().Length - 1);
        }

        // GET: PhieuDatPhong
        public ActionResult Index()
        {
            var phieuDatPhongs = db.PhieuDatPhongs.Include(p => p.KhachHang).Include(p => p.NhanVien);
            return View(phieuDatPhongs.ToList());
        }

        // GET: PhieuDatPhong/Details/5
        [HasCredential(IDQuyen = "QUANLYPHIEUDATPHONG")]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PhieuDatPhong phieuDatPhong = db.PhieuDatPhongs.Find(id);
            if (phieuDatPhong == null)
            {
                return HttpNotFound();
            }
            return View(phieuDatPhong);
        }

        // GET: PhieuDatPhong/Create
        [HasCredential(IDQuyen = "QUANLYPHIEUDATPHONG")]
        public ActionResult Create()
        {
            ViewBag.maPDP = LayMaPDP();
            ViewBag.maKH = new SelectList(db.KhachHangs, "maKH", "tenKH");
            var session = (UserLogin)HttpContext.Session[CommonConstants.USER_SESSION];
            var maNV = db.QuanTris.Where(x => x.username == session.UserName).Select(x => x.maNV).SingleOrDefault();
            ViewBag.maNV = new SelectList(db.NhanViens, "maNV", "tenNV", maNV);
            ViewBag.maP = new SelectList(db.Phongs, "maP", "maP");
            return View();
        }

        [HasCredential(IDQuyen = "QUANLYPHIEUDATPHONG")]
        public ActionResult SaveOrder(string maPDP, string maKH, string ngayDen, string ngayDi, string soNguoi, string tinhTrang, string maNV, CTPhieuDatPhong[] order)
        {
            string result = "Xảy ra lỗi. Thêm không thành công";
            if (maPDP != null && maKH != null && ngayDen != null && ngayDi != null && soNguoi != null && tinhTrang != null && maNV != null && order != null)
            {
                decimal tongTien = 0;
                PhieuDatPhong model = new PhieuDatPhong
                {
                    maPDP = maPDP,
                    maKH = maKH,
                    ngayDen = DateTime.Parse(ngayDen),
                    ngayDi = DateTime.Parse(ngayDi),
                    soNguoi = int.Parse(soNguoi),
                    tinhTrang = Boolean.Parse(tinhTrang),
                    maNV = maNV
                };


                foreach (var item in order)
                {
                    CTPhieuDatPhong cTPhieuDat = new CTPhieuDatPhong
                    {
                        maPDP = maPDP,
                        maP = item.maP,
                        tienCoc = item.tienCoc
                    };
                    tongTien += item.tienCoc;
                    db.CTPhieuDatPhongs.Add(cTPhieuDat);
                }
                model.tongTienCoc = tongTien;
                db.PhieuDatPhongs.Add(model);
                db.SaveChanges();
                result = "Thêm thành công";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: PhieuDatPhong/Edit/5
        [HasCredential(IDQuyen = "QUANLYPHIEUDATPHONG")]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PhieuDatPhong phieuDatPhong = db.PhieuDatPhongs.Find(id);
            if (phieuDatPhong == null)
            {
                return HttpNotFound();
            }
            ViewBag.maNV = new SelectList(db.NhanViens, "maNV", "tenNV", phieuDatPhong.maNV);
            ViewBag.maKH = new SelectList(db.KhachHangs, "maKH", "tenKH", phieuDatPhong.maKH);
            return View(phieuDatPhong);
        }

        // POST: PhieuDatPhong/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasCredential(IDQuyen = "QUANLYPHIEUDATPHONG")]
        public ActionResult Edit([Bind(Include = "maPDP,maKH,ngayDen,ngayDi,tongTienCoc,soNguoi,tinhTrang,maNV")] PhieuDatPhong phieuDatPhong)
        {
            // Tìm kiếm pdp tương ứng qua mã PDP
            var checkPSP = db.PhieuDatPhongs.SingleOrDefault(n => n.maPDP.Equals(phieuDatPhong.maPDP));
            if (checkPSP == null)
            {
                ViewBag.maKH = new SelectList(db.KhachHangs, "maKH", "tenKH", phieuDatPhong.maKH);
                return View(phieuDatPhong);
            }
            else
            {
                PhieuDatPhong pdp = new PhieuDatPhong();
                checkPSP.maKH = phieuDatPhong.maKH;
                checkPSP.ngayDen = phieuDatPhong.ngayDen;
                checkPSP.ngayDi = phieuDatPhong.ngayDi;
                checkPSP.tongTienCoc = phieuDatPhong.tongTienCoc;
                checkPSP.soNguoi = phieuDatPhong.soNguoi;
                checkPSP.tinhTrang = phieuDatPhong.tinhTrang;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }

        // GET: PhieuDatPhong/Delete/5
        [HasCredential(IDQuyen = "QUANLYPHIEUDATPHONG")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PhieuDatPhong phieuDatPhong = db.PhieuDatPhongs.Find(id);
            if (phieuDatPhong == null)
            {
                return HttpNotFound();
            }
            return View(phieuDatPhong);
        }

        // POST: PhieuDatPhong/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [HasCredential(IDQuyen = "QUANLYPHIEUDATPHONG")]
        public ActionResult DeleteConfirmed(string id)
        {
            PhieuDatPhong phieuDatPhong = db.PhieuDatPhongs.Find(id);
            List<CTPhieuDatPhong> cTPhieuDatPhongs = db.CTPhieuDatPhongs.Where(x => x.maPDP == id).ToList();
            foreach (var item in cTPhieuDatPhongs)
            {
                db.CTPhieuDatPhongs.Remove(item);
            }
            List<PhieuThuePhong> phieuThuePhongs = db.PhieuThuePhongs.Where(x => x.maPDP == id).ToList();
            if (phieuThuePhongs != null)
            {
                foreach (var item in phieuThuePhongs)
                {
                    List<CTPhieuThuePhong> cTPhieuThuePhongs = db.CTPhieuThuePhongs.Where(x => x.maPTP == item.maPTP).ToList();
                    db.PhieuThuePhongs.Remove(item);
                    foreach (var detailItem in cTPhieuThuePhongs)
                    {
                        db.CTPhieuThuePhongs.Remove(detailItem);
                    }
                }
            }
            db.PhieuDatPhongs.Remove(phieuDatPhong);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        [HasCredential(IDQuyen = "QUANLYPHIEUDATPHONG")]
        public ActionResult CreateCTPhieuDat(string maPDP)
        {
            var phongDaChon = db.CTPhieuDatPhongs.Where(x => x.maPDP == maPDP).Select(x => x.maP).ToList();
            var phongs = db.Phongs.Select(x => x.maP).ToList();
            foreach (var item in phongDaChon)
            {
                if (phongs.Contains(item))
                {
                    phongs.Remove(item);
                }
            }
            ViewBag.maP = new SelectList(phongs, "maP");
            ViewBag.maPDP = maPDP;
            return View();
        }

        // POST: CTPhieuDatPhongs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasCredential(IDQuyen = "QUANLYPHIEUDATPHONG")]
        public ActionResult CreateCTPhieuDat([Bind(Include = "maPDP,maP,tienCoc")] CTPhieuDatPhong cTPhieuDatPhong)
        {
            if (ModelState.IsValid)
            {
                db.CTPhieuDatPhongs.Add(cTPhieuDatPhong);
                var phieuDatPhong = db.PhieuDatPhongs.Find(cTPhieuDatPhong.maPDP);
                phieuDatPhong.tongTienCoc += cTPhieuDatPhong.tienCoc;
                db.Entry(phieuDatPhong).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "PhieuDatPhongs", new { id = cTPhieuDatPhong.maPDP });
            }

            var phongDaChon = db.CTPhieuDatPhongs.Where(x => x.maPDP == cTPhieuDatPhong.maPDP).Select(x => x.maP).ToList();
            var phongs = db.Phongs.Select(x => x.maP).ToList();
            foreach (var item in phongDaChon)
            {
                if (phongs.Contains(item))
                {
                    phongs.Remove(item);
                }
            }
            ViewBag.maP = new SelectList(phongs, "maP");
            ViewBag.maPDP = cTPhieuDatPhong.maPDP;
            return View(cTPhieuDatPhong);
        }

        [HasCredential(IDQuyen = "QUANLYPHIEUDATPHONG")]
        public ActionResult DeleteCTPhieuDat(string maPDP, string maP)
        {
            if (maPDP == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CTPhieuDatPhong cTPhieuDatPhong = db.CTPhieuDatPhongs.Find(maPDP, maP);
            if (cTPhieuDatPhong == null)
            {
                return HttpNotFound();
            }
            ViewBag.maPDP = maPDP;
            return View(cTPhieuDatPhong);
        }

        // POST: PhieuDatPhong/Delete/5
        [HttpPost, ActionName("DeleteCTPhieuDat")]
        [ValidateAntiForgeryToken]
        [HasCredential(IDQuyen = "QUANLYPHIEUDATPHONG")]
        public ActionResult DeleteCTPhieuDatConfirmed(string maPDP, string maP)
        {
            CTPhieuDatPhong cTPhieuDatPhong = db.CTPhieuDatPhongs.Find(maPDP, maP);
            PhieuDatPhong phieuDatPhong = db.PhieuDatPhongs.Find(maPDP);
            db.CTPhieuDatPhongs.Remove(cTPhieuDatPhong);
            phieuDatPhong.tongTienCoc -= cTPhieuDatPhong.tienCoc;
            db.Entry(phieuDatPhong).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Details", "PhieuDatPhongs", new { id = maPDP });
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
