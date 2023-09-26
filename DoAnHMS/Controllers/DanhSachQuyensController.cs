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
    public class DanhSachQuyensController : BaseController
    {
        private DoAnHMSEntities db = new DoAnHMSEntities();

        // GET: DanhSachQuyens
        [HasCredential(IDQuyen = "PHANQUYEN")]
        public ActionResult Index()
        {
            var danhSachQuyens = db.DanhSachQuyens.Include(d => d.NhomNhanVien).Include(d => d.Quyen);
            return View(danhSachQuyens.ToList());
        }

        // GET: DanhSachQuyens/Create
        [HasCredential(IDQuyen = "PHANQUYEN")]
        public ActionResult Create()
        {
            ViewBag.IDNhom = new SelectList(db.NhomNhanViens, "IDNhom", "TenNhom");
            ViewBag.IDQuyen = new SelectList(db.Quyens, "IDQuyen", "TenQuyen");
            return View();
        }

        // POST: DanhSachQuyens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasCredential(IDQuyen = "PHANQUYEN")]
        public ActionResult Create([Bind(Include = "IDNhom,IDQuyen,GhiChu")] DanhSachQuyen danhSachQuyen)
        {
            if (ModelState.IsValid)
            {
                DanhSachQuyen quyen = db.DanhSachQuyens.Find(danhSachQuyen.IDNhom, danhSachQuyen.IDQuyen);
                if (quyen != null)
                {
                    ModelState.AddModelError("", "Quyền này đã tồn tại.");
                }
                else
                {
                    db.DanhSachQuyens.Add(danhSachQuyen);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                
            }
            ViewBag.IDNhom = new SelectList(db.NhomNhanViens, "IDNhom", "TenNhom", danhSachQuyen.IDNhom);
            ViewBag.IDQuyen = new SelectList(db.Quyens, "IDQuyen", "TenQuyen", danhSachQuyen.IDQuyen);
            return View(danhSachQuyen);
        }

        // GET: DanhSachQuyens/Delete/5
        [HasCredential(IDQuyen = "PHANQUYEN")]
        public ActionResult Delete(string IDNhom, string IDQuyen)
        {
            if (IDNhom == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DanhSachQuyen danhSachQuyen = db.DanhSachQuyens.Find(IDNhom, IDQuyen);
            if (danhSachQuyen == null)
            {
                return HttpNotFound();
            }
            return View(danhSachQuyen);
        }

        // POST: DanhSachQuyens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [HasCredential(IDQuyen = "PHANQUYEN")]
        public ActionResult DeleteConfirmed(string IDNhom, string IDQuyen)
        {
            DanhSachQuyen danhSachQuyen = db.DanhSachQuyens.Find(IDNhom, IDQuyen);
            db.DanhSachQuyens.Remove(danhSachQuyen);
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
