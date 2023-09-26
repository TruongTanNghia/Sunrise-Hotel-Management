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
    public class NhanViensController : BaseController
    {
        private readonly DoAnHMSEntities db = new DoAnHMSEntities();
        string LayMaNV()
        {
            var maMax = db.NhanViens.ToList().Select(n => n.maNV).Max();
            int maNV = int.Parse(maMax.Substring(2)) + 1;
            string NV = String.Concat("000", maNV.ToString());
            return "NV" + NV.Substring(maNV.ToString().Length - 1);
        }

        // GET: NhanViens
        [HasCredential(IDQuyen = "QUANLYNHANVIEN")]
        public ActionResult Index()
        {
            return View(db.NhanViens.ToList());
        }

        [HttpGet]
        [HasCredential(IDQuyen = "QUANLYNHANVIEN")]
        public ActionResult Search(string maNV = "", string tenNV = "", string gioiTinh = "", string diaChi = "", string email = "", string sdt = "", string idNhom = "")
        {
            if (gioiTinh != "1" && gioiTinh != "0")
                gioiTinh = null;
            ViewBag.maNV = maNV;
            ViewBag.tenNV = tenNV;
            ViewBag.gioiTinh = gioiTinh;
            ViewBag.diaChi = diaChi;
            ViewBag.email = email;
            ViewBag.sdt = sdt;
            ViewBag.idNhom = new SelectList(db.NhomNhanViens, "IDNhom", "TenNhom");
            var nhanViens = db.NhanViens.SqlQuery("NhanVien_TimKiem'" + maNV + "','" + tenNV + "','" + gioiTinh + "','" + diaChi + "','" + email + "','" + sdt + "','" + idNhom + "'");
            if (nhanViens.Count() == 0)

                ViewBag.TB = "Không có thông tin tìm kiếm.";
            return View(nhanViens.ToList());
        }

        // GET: NhanViens/Details/5
        [HasCredential(IDQuyen = "QUANLYNHANVIEN")]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NhanVien nhanVien = db.NhanViens.Find(id);
            if (nhanVien == null)
            {
                return HttpNotFound();
            }
            return View(nhanVien);
        }

        // GET: NhanViens/Create
        [HasCredential(IDQuyen = "QUANLYNHANVIEN")]
        public ActionResult Create()
        {
            ViewBag.MaNV = LayMaNV();
            return View();
        }

        // POST: NhanViens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasCredential(IDQuyen = "QUANLYNHANVIEN")]
        public ActionResult Create([Bind(Include = "maNV,tenNV,gioiTinh,ngaySinh,diaChi,email,sdt,hinhAnh")] NhanVien nhanVien)
        {
            if (ModelState.IsValid)
            {
                var imgNV = Request.Files["Avatar"];
                //Lấy thông tin từ input type=file có tên Avatar
                string postedFileName = System.IO.Path.GetFileName(imgNV.FileName);
                //Lưu hình đại diện về Server
                var path = Server.MapPath("/Images/" + postedFileName);
                imgNV.SaveAs(path);
                nhanVien.hinhAnh = postedFileName;
                db.NhanViens.Add(nhanVien);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaNV = LayMaNV();
            return View(nhanVien);
        }

        // GET: NhanViens/Edit/5
        [HasCredential(IDQuyen = "QUANLYNHANVIEN")]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NhanVien nhanVien = db.NhanViens.Find(id);
            ViewBag.ngaySinh = nhanVien.ngaySinh.ToShortDateString();
            if (nhanVien == null)
            {
                return HttpNotFound();
            }
            return View(nhanVien);
        }

        // POST: NhanViens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasCredential(IDQuyen = "QUANLYNHANVIEN")]
        public ActionResult Edit([Bind(Include = "maNV,tenNV,gioiTinh,ngaySinh,diaChi,email,sdt,hinhAnh")] NhanVien nhanVien)
        {
            var imgNV = Request.Files["Avatar"];
            try
            {
                //Lấy thông tin từ input type=file có tên Avatar
                string postedFileName = System.IO.Path.GetFileName(imgNV.FileName);
                //Lưu hình đại diện về Server
                var path = Server.MapPath("/Images/" + postedFileName);
                imgNV.SaveAs(path);
            }
            catch { }
            if (ModelState.IsValid)
            {
                db.Entry(nhanVien).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(nhanVien);
        }

        // GET: NhanViens/Delete/5
        [HasCredential(IDQuyen = "QUANLYNHANVIEN")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NhanVien nhanVien = db.NhanViens.Find(id);
            if (nhanVien == null)
            {
                return HttpNotFound();
            }
            return View(nhanVien);
        }

        // POST: NhanViens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [HasCredential(IDQuyen = "QUANLYNHANVIEN")]
        public ActionResult DeleteConfirmed(string id)
        {
            NhanVien nhanVien = db.NhanViens.Find(id);
            db.NhanViens.Remove(nhanVien);
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
