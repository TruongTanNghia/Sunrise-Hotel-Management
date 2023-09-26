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
    public class QuanTrisController : BaseController
    {
        private DoAnHMSEntities db = new DoAnHMSEntities();

        // GET: QuanTris
        [HasCredential(IDQuyen = "QUANLYTAIKHOANNV")]
        public ActionResult Index()
        {
            var quanTris = db.QuanTris.Include(q => q.NhanVien).Include(q => q.NhomNhanVien);
            return View(quanTris.ToList());
        }

        // GET: QuanTris/Create
        [HasCredential(IDQuyen = "QUANLYTAIKHOANNV")]
        public ActionResult Create()
        {
            ViewBag.maNV = new SelectList(db.NhanViens, "maNV", "tenNV");
            ViewBag.IDNhom = new SelectList(db.NhomNhanViens, "IDNhom", "TenNhom");
            return View();
        }

        // POST: QuanTris/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasCredential(IDQuyen = "QUANLYTAIKHOANNV")]
        public ActionResult Create([Bind(Include = "username,password,tinhTrang,maNV,IDNhom")] QuanTri quanTri)
        {
            if (ModelState.IsValid)
            {
                QuanTri oldQuanTri = db.QuanTris.Find(quanTri.username);
                if (oldQuanTri == null)
                {
                    string passwordMD5 = Encryptor.MD5Hash(quanTri.password);
                    quanTri.password = passwordMD5;
                    db.QuanTris.Add(quanTri);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Tên tài khoản đã tồn tại.");
                }
                    
            }

            ViewBag.maNV = new SelectList(db.NhanViens, "maNV", "tenNV", quanTri.maNV);
            ViewBag.IDNhom = new SelectList(db.NhomNhanViens, "IDNhom", "TenNhom", quanTri.IDNhom);
            return View(quanTri);
        }

        // GET: QuanTris/Edit/5
        [HasCredential(IDQuyen = "QUANLYTAIKHOANNV")]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuanTri quanTri = db.QuanTris.Find(id);
            if (quanTri == null)
            {
                return HttpNotFound();
            }
            ViewBag.oldPass = quanTri.password;
            ViewBag.maNV = new SelectList(db.NhanViens, "maNV", "tenNV", quanTri.maNV);
            ViewBag.IDNhom = new SelectList(db.NhomNhanViens, "IDNhom", "TenNhom", quanTri.IDNhom);
            return View(quanTri);
        }

        // POST: QuanTris/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasCredential(IDQuyen = "QUANLYTAIKHOANNV")]
        public ActionResult Edit([Bind(Include = "username,password,tinhTrang,maNV,IDNhom")] QuanTri quanTri, string oldPass)
        {
            if (ModelState.IsValid)
            {
                if(oldPass != quanTri.password)
                {
                    string passwordMD5 = Encryptor.MD5Hash(quanTri.password);
                    quanTri.password = passwordMD5;
                }
                db.Entry(quanTri).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.maNV = new SelectList(db.NhanViens, "maNV", "tenNV", quanTri.maNV);
            ViewBag.IDNhom = new SelectList(db.NhomNhanViens, "IDNhom", "TenNhom", quanTri.IDNhom);
            return View(quanTri);
        }

        // GET: QuanTris/Delete/5
        [HasCredential(IDQuyen = "QUANLYTAIKHOANNV")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuanTri quanTri = db.QuanTris.Find(id);
            if (quanTri == null)
            {
                return HttpNotFound();
            }
            return View(quanTri);
        }

        // POST: QuanTris/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [HasCredential(IDQuyen = "QUANLYTAIKHOANNV")]
        public ActionResult DeleteConfirmed(string id)
        {
            QuanTri quanTri = db.QuanTris.Find(id);
            db.QuanTris.Remove(quanTri);
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
