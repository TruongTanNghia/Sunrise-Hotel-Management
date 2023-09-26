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
    public class PhieuThuePhongsController : BaseController
    {
        private readonly DoAnHMSEntities db = new DoAnHMSEntities();
        string LayMaPTP()
        {
            // Kiểm tra mã trước khi sub (Thuộc trường hợp db đang trống)

            var maMax = db.PhieuThuePhongs.ToList().Select(n => n.maPTP).Max();
            if (maMax != null)
            {
                int maPTP = int.Parse(maMax.Substring(3)) + 1;
                string PTP = String.Concat("000", maPTP.ToString());
                return "PTP" + PTP.Substring(maPTP.ToString().Length - 1);
            }
            else
            {
                return "PTP0001";
            }

        }
        // GET: PhieuThuePhongs
        public ActionResult Index()
        {
            var phieuThuePhongs = db.PhieuThuePhongs.Include(p => p.KhachHang).Include(p => p.NhanVien).Include(p => p.PhieuDatPhong);
            return View(phieuThuePhongs.ToList());
        }

        // GET: PhieuThuePhongs/Details/5
        [HasCredential(IDQuyen = "QUANLYPHIEUTHUEPHONG")]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PhieuThuePhong phieuThuePhong = db.PhieuThuePhongs.Find(id);
            if (phieuThuePhong == null)
            {
                return HttpNotFound();
            }
            return View(phieuThuePhong);
        }

        // GET: PhieuThuePhongs/Create
        [HasCredential(IDQuyen = "QUANLYPHIEUTHUEPHONG")]
        public ActionResult Create()
        {
            ViewBag.maPTP = LayMaPTP();
            ViewBag.maKH = new SelectList(db.KhachHangs, "maKH", "tenKH");
            var session = (UserLogin)HttpContext.Session[CommonConstants.USER_SESSION];
            var maNV = db.QuanTris.Where(x => x.username == session.UserName).Select(x => x.maNV).SingleOrDefault();
            ViewBag.maNV = new SelectList(db.NhanViens, "maNV", "tenNV", maNV);
            ViewBag.maPDP = new SelectList(db.PhieuDatPhongs, "maPDP", "maPDP");
            ViewBag.maP = new SelectList(db.Phongs, "maP", "maP");
            ViewBag.maDV = new SelectList(db.DichVus, "maDV", "tenDV");
            return View();
        }

        [HasCredential(IDQuyen = "QUANLYPHIEUTHUEPHONG")]
        public ActionResult SaveOrder(string maPTP, string maPDP, string maKH, string ngayThue, string ngayTra, string maNV, CTPhieuThuePhong[] order)
        {
            string result = "Xảy ra lỗi. Thêm không thành công";
            if (maPTP != null && maPDP != null && maKH != null && ngayThue != null && ngayTra != null && maNV != null && order != null)
            {
                PhieuThuePhong model = new PhieuThuePhong();
                model.maPTP = maPTP;
                model.maPDP = maPDP;
                model.maKH = maKH;
                model.maNV = maNV;
                model.ngayThue = DateTime.Parse(ngayThue);
                model.ngayTra = DateTime.Parse(ngayTra);
                db.PhieuThuePhongs.Add(model);
                db.SaveChanges();

                foreach (var item in order)
                {
                    CTPhieuThuePhong cTPhieuThue = new CTPhieuThuePhong();
                    cTPhieuThue.maPTP = maPTP;
                    cTPhieuThue.maP = item.maP;
                    cTPhieuThue.ngaySD = item.ngaySD;
                    cTPhieuThue.maDV = item.maDV;
                    cTPhieuThue.soLuong = item.soLuong;

                    Phong phong = db.Phongs.Find(item.maP);
                    if (phong != null)
                    {
                        if (DateTime.Now.Date >= model.ngayThue.Date && DateTime.Now.Date <= model.ngayTra.Date)
                        {
                            phong.tinhTrang = "Đang sử dụng";
                            db.Entry(phong).State = EntityState.Modified;
                        }
                    }
                    db.CTPhieuThuePhongs.Add(cTPhieuThue);
                    db.SaveChanges();
                }

                result = "Thêm thành công";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: PhieuThuePhongs/Edit/5
        [HasCredential(IDQuyen = "QUANLYPHIEUTHUEPHONG")]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PhieuThuePhong phieuThuePhong = db.PhieuThuePhongs.Find(id);
            if (phieuThuePhong == null)
            {
                return HttpNotFound();
            }
            ViewBag.maPDP = phieuThuePhong.maPDP == null ? "Không có" : phieuThuePhong.maPDP;
            ViewBag.maKH = new SelectList(db.KhachHangs, "maKH", "tenKH", phieuThuePhong.maKH);
            ViewBag.maNV = new SelectList(db.NhanViens, "maNV", "tenNV", phieuThuePhong.maNV);
            return View(phieuThuePhong);
        }

        // POST: PhieuThuePhongs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasCredential(IDQuyen = "QUANLYPHIEUTHUEPHONG")]
        public ActionResult Edit([Bind(Include = "maPTP,maPDP,maKH,maNV,ngayThue,ngayTra")] PhieuThuePhong phieuThuePhong)
        {
            if (ModelState.IsValid)
            {
                if (phieuThuePhong.maPDP == "Không có")
                {
                    phieuThuePhong.maPDP = null;
                }
                db.Entry(phieuThuePhong).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.maPDP = phieuThuePhong.maPDP == null ? "Không có" : phieuThuePhong.maPDP;
            ViewBag.maKH = new SelectList(db.KhachHangs, "maKH", "tenKH", phieuThuePhong.maKH);
            ViewBag.maNV = new SelectList(db.NhanViens, "maNV", "tenNV", phieuThuePhong.maNV);
            return View(phieuThuePhong);
        }

        // GET: PhieuThuePhongs/Delete/5
        [HasCredential(IDQuyen = "QUANLYPHIEUTHUEPHONG")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PhieuThuePhong phieuThuePhong = db.PhieuThuePhongs.Find(id);
            if (phieuThuePhong == null)
            {
                return HttpNotFound();
            }
            return View(phieuThuePhong);
        }

        // POST: PhieuThuePhongs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [HasCredential(IDQuyen = "QUANLYPHIEUTHUEPHONG")]
        public ActionResult DeleteConfirmed(string id)
        {
            PhieuThuePhong phieuThuePhong = db.PhieuThuePhongs.Find(id);
            List<CTPhieuThuePhong> cTPhieuThuePhongs = db.CTPhieuThuePhongs.Where(x => x.maPTP == id).ToList();
            HoaDon hoaDon = db.HoaDons.Find(id);
            foreach (var item in cTPhieuThuePhongs)
            {
                db.CTPhieuThuePhongs.Remove(item);
            }
            db.PhieuThuePhongs.Remove(phieuThuePhong);
            if (hoaDon != null)
            {
                db.HoaDons.Remove(hoaDon);
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HasCredential(IDQuyen = "QUANLYPHIEUTHUEPHONG")]
        public ActionResult CreateCTPhieuThue(string maPTP)
        {
            ViewBag.maPTP = maPTP;
            ViewBag.maP = new SelectList(db.Phongs, "maP", "maP");
            ViewBag.maDV = new SelectList(db.DichVus, "maDV", "tenDV");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasCredential(IDQuyen = "QUANLYPHIEUTHUEPHONG")]
        public ActionResult CreateCTPhieuThue([Bind(Include = "maPTP,maP,maDV,ngaySD,soLuong")] CTPhieuThuePhong cTPhieuThuePhong)
        {
            if (ModelState.IsValid)
            {
                var daSuDung = db.CTPhieuThuePhongs.Where(x => x.maPTP == cTPhieuThuePhong.maPTP && x.maP == cTPhieuThuePhong.maP && x.maDV == cTPhieuThuePhong.maDV && x.ngaySD == cTPhieuThuePhong.ngaySD).ToList();
                if (daSuDung.Count() <= 0)
                {
                    db.CTPhieuThuePhongs.Add(cTPhieuThuePhong);
                    db.SaveChanges();
                    return RedirectToAction("Details", "PhieuThuePhongs", new { id = cTPhieuThuePhong.maPTP });
                }
                else
                {
                    ModelState.AddModelError("", "Dịch vụ của phòng này sử dụng vào thời gian này đã tồn tại");
                }

            }
            ViewBag.maPTP = cTPhieuThuePhong.maPTP;
            ViewBag.maP = new SelectList(db.Phongs, "maP", "maP");
            ViewBag.maDV = new SelectList(db.DichVus, "maDV", "tenDV");
            return View(cTPhieuThuePhong);
        }

        [HasCredential(IDQuyen = "QUANLYPHIEUTHUEPHONG")]
        public ActionResult DeleteCTPhieuThue(string maPTP, string maP, string maDV, DateTime ngaySD)
        {
            if (maPTP == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var cTPhieuThuePhong = db.CTPhieuThuePhongs.Where(x => x.maPTP == maPTP && x.maP == maP && x.maDV == maDV && x.ngaySD == ngaySD).SingleOrDefault();
            if (cTPhieuThuePhong == null)
            {
                return HttpNotFound();
            }
            ViewBag.maPTP = maPTP;
            return View(cTPhieuThuePhong);
        }

        [HttpPost, ActionName("DeleteCTPhieuThue")]
        [ValidateAntiForgeryToken]
        [HasCredential(IDQuyen = "QUANLYPHIEUTHUEPHONG")]
        public ActionResult DeleteCTPhieuThueConfirmed(string maPTP, string maP, string maDV, DateTime ngaySD)
        {
            var cTPhieuThuePhong = db.CTPhieuThuePhongs.Where(x => x.maPTP == maPTP && x.maP == maP && x.maDV == maDV && x.ngaySD == ngaySD).SingleOrDefault();
            db.CTPhieuThuePhongs.Remove(cTPhieuThuePhong);
            db.SaveChanges();
            return RedirectToAction("Details", "PhieuThuePhongs", new { id = cTPhieuThuePhong.maPTP });
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
