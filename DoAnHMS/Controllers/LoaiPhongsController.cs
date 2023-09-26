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

namespace Nhom.Controllers
{
    public class LoaiPhongsController : BaseController
    {
        private DoAnHMSEntities db = new DoAnHMSEntities();

        // GET: LoaiPhongs
        public ActionResult Index()
        {
            return View(db.LoaiPhongs.ToList());
        }

        // GET: LoaiPhongs/Details/5
        [HasCredential(IDQuyen = "QUANLYLOAIPHONG")]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoaiPhong loaiPhong = db.LoaiPhongs.Find(id);
            if (loaiPhong == null)
            {
                return HttpNotFound();
            }
            return View(loaiPhong);
        }

        // GET: LoaiPhongs/Create
        [HasCredential(IDQuyen = "QUANLYLOAIPHONG")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: LoaiPhongs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasCredential(IDQuyen = "QUANLYLOAIPHONG")]
        public ActionResult Create([Bind(Include = "maLP,tenLP,hinhAnh,sucChua,donGia,moTa")] LoaiPhong loaiPhong)
        {
            if (ModelState.IsValid)
            {
                LoaiPhong oldLoaiPhong = db.LoaiPhongs.Find(loaiPhong.maLP);
                if (oldLoaiPhong == null)
                {
                    //System.Web.HttpPostedFileBase Avatar;
                    var imgLP = Request.Files["Avatar"];
                    //Lấy thông tin từ input type=file có tên Avatar
                    string postedFileName = System.IO.Path.GetFileName(imgLP.FileName);
                    //Lưu hình đại diện về Server
                    var path = Server.MapPath("/Images/" + postedFileName);
                    imgLP.SaveAs(path);
                    loaiPhong.hinhAnh = postedFileName;
                    db.LoaiPhongs.Add(loaiPhong);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else {
                    ModelState.AddModelError("", "Mã loại phòng đã tồn tại.");
                }
            }

            return View(loaiPhong);
        }

        // GET: LoaiPhongs/Edit/5
        [HasCredential(IDQuyen = "QUANLYLOAIPHONG")]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoaiPhong loaiPhong = db.LoaiPhongs.Find(id);
            if (loaiPhong == null)
            {
                return HttpNotFound();
            }
            return View(loaiPhong);
        }

        // POST: LoaiPhongs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasCredential(IDQuyen = "QUANLYLOAIPHONG")]
        public ActionResult Edit([Bind(Include = "maLP,tenLP,hinhAnh,sucChua,donGia,moTa")] LoaiPhong loaiPhong)
        {
            var imgLP = Request.Files["Avatar"];
            try
            {
                //Lấy thông tin từ input type=file có tên Avatar
                string postedFileName = System.IO.Path.GetFileName(imgLP.FileName);
                //Lưu hình đại diện về Server
                var path = Server.MapPath("/Images/" + postedFileName);
                imgLP.SaveAs(path);
            }
            catch { }
            if (ModelState.IsValid)
            {

                db.Entry(loaiPhong).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(loaiPhong);
        }

        // GET: LoaiPhongs/Delete/5
        [HasCredential(IDQuyen = "QUANLYLOAIPHONG")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoaiPhong loaiPhong = db.LoaiPhongs.Find(id);
            if (loaiPhong == null)
            {
                return HttpNotFound();
            }
            return View(loaiPhong);
        }

        // POST: LoaiPhongs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [HasCredential(IDQuyen = "QUANLYLOAIPHONG")]
        public ActionResult DeleteConfirmed(string id)
        {
            LoaiPhong loaiPhong = db.LoaiPhongs.Find(id);
            db.LoaiPhongs.Remove(loaiPhong);
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
