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
    public class PhongsController : BaseController
    {
        private DoAnHMSEntities db = new DoAnHMSEntities();

        // GET: Phongs
        public ActionResult Index()
        {
            var phongs = db.Phongs.Include(p => p.LoaiPhong);
            return View(phongs.ToList());
        }

        // GET: Phongs/Details/5
        [HasCredential(IDQuyen = "QUANLYPHONG")]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Phong phong = db.Phongs.Find(id);
            if (phong == null)
            {
                return HttpNotFound();
            }
            return View(phong);
        }

        // GET: Phongs/Create
        [HasCredential(IDQuyen = "QUANLYPHONG")]
        public ActionResult Create()
        {
            ViewBag.maLP = new SelectList(db.LoaiPhongs, "maLP", "tenLP");
            return View();
        }

        // POST: Phongs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasCredential(IDQuyen = "QUANLYPHONG")]
        public ActionResult Create([Bind(Include = "maP,maLP,tinhTrang")] Phong phong)
        {
            if (ModelState.IsValid)
            {
                Phong oldphong = db.Phongs.Find(phong.maP);
                if (oldphong == null)
                {
                    db.Phongs.Add(phong);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Mã phòng đã tồn tại.");
                }
            }

            ViewBag.maLP = new SelectList(db.LoaiPhongs, "maLP", "tenLP", phong.maLP);
            return View(phong);
        }

        [HttpGet]
        public ActionResult TimKiem(string tinhTrang = "", string maLP = "")
        {
            ViewBag.tinhTrang = tinhTrang;
            ViewBag.maLP = new SelectList(db.LoaiPhongs, "maLP", "tenLP");
            IQueryable<Phong> phongs;
            if (tinhTrang == "")
            {
                phongs = db.Phongs.Where(abc => abc.tinhTrang.Contains(tinhTrang) && abc.maLP.Contains(maLP));
            }
            else
            {
                phongs = db.Phongs.Where(abc => abc.tinhTrang == tinhTrang && abc.maLP.Contains(maLP));
            }
            if (phongs.Count() == 0)
                ViewBag.TB = "Không có thông tin tìm kiếm.";
            return View(phongs.ToList());
        }

        // GET: Phongs/Edit/5
        [HasCredential(IDQuyen = "QUANLYPHONG")]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Phong phong = db.Phongs.Find(id);
            if (phong == null)
            {
                return HttpNotFound();
            }
            ViewBag.maLP = new SelectList(db.LoaiPhongs, "maLP", "tenLP", phong.maLP);
            return View(phong);
        }

        // POST: Phongs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasCredential(IDQuyen = "QUANLYPHONG")]
        public ActionResult Edit([Bind(Include = "maP,maLP,tinhTrang")] Phong phong)
        {
            if (ModelState.IsValid)
            {
                db.Entry(phong).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.maLP = new SelectList(db.LoaiPhongs, "maLP", "tenLP", phong.maLP);
            return View(phong);
        }

        // GET: Phongs/Delete/5
        [HasCredential(IDQuyen = "QUANLYPHONG")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Phong phong = db.Phongs.Find(id);
            if (phong == null)
            {
                return HttpNotFound();
            }
            return View(phong);
        }

        // POST: Phongs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [HasCredential(IDQuyen = "QUANLYPHONG")]
        public ActionResult DeleteConfirmed(string id)
        {
            Phong phong = db.Phongs.Find(id);
            db.Phongs.Remove(phong);
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
