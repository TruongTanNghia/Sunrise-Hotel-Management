using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DoAnHMS.Common;
using DoAnHMS.Controllers;
using DoAnHMS.Models;

namespace doan.Controllers
{
    public class DichVusController : BaseController
    {
        private DoAnHMSEntities db = new DoAnHMSEntities();

        string LayMaDV()
        {
            var maMax = db.DichVus.ToList().Select(n => n.maDV).Max();
            int maDV = int.Parse(maMax.Substring(2)) + 1;
            string DV = String.Concat("000", maDV.ToString());
            return "DV" + DV.Substring(maDV.ToString().Length - 1);
        }
        // GET: DichVus
        public ActionResult Index()
        {
            return View(db.DichVus.ToList());
        }

        // GET: DichVus/Create
        [HasCredential(IDQuyen = "QUANLYDICHVU")]
        public ActionResult Create()
        {
            ViewBag.maDV = LayMaDV();
            return View();
        }

        // POST: DichVus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasCredential(IDQuyen = "QUANLYDICHVU")]
        public ActionResult Create([Bind(Include = "maDV,tenDV,gia")] DichVu dichVu)
        {
            if (ModelState.IsValid)
            {
                dichVu.maDV = LayMaDV();
                db.DichVus.Add(dichVu);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.maDV = LayMaDV();
            return View(dichVu);
        }

        // GET: DichVus/Edit/5
        [HasCredential(IDQuyen = "QUANLYDICHVU")]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DichVu dichVu = db.DichVus.Find(id);
            if (dichVu == null)
            {
                return HttpNotFound();
            }
            return View(dichVu);
        }

        // POST: DichVus/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasCredential(IDQuyen = "QUANLYDICHVU")]
        public ActionResult Edit([Bind(Include = "maDV,tenDV,gia")] DichVu dichVu)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dichVu).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.maDV = LayMaDV();
            return View(dichVu);
        }

        // GET: DichVus/Delete/5
        [HasCredential(IDQuyen = "QUANLYDICHVU")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DichVu dichVu = db.DichVus.Find(id);
            if (dichVu == null)
            {
                return HttpNotFound();
            }
            return View(dichVu);
        }

        // POST: DichVus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [HasCredential(IDQuyen = "QUANLYDICHVU")]
        public ActionResult DeleteConfirmed(string id)
        {
            DichVu dichVu = db.DichVus.Find(id);
            db.DichVus.Remove(dichVu);
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
