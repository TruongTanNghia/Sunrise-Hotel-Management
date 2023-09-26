using DoAnHMS.Common;
using DoAnHMS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnHMS.Controllers
{
    public class ThongTinTaiKhoanController : BaseController
    {
        DoAnHMSEntities db = new DoAnHMSEntities();
        public ActionResult Index()
        {
            var session = (UserLogin)HttpContext.Session[CommonConstants.USER_SESSION];
            var quanTri = db.QuanTris.Where(x => x.username == session.UserName).SingleOrDefault();
            var nhanVien = db.NhanViens.Where(x => x.maNV == quanTri.maNV).SingleOrDefault();
            return View(nhanVien);
        }

        public ActionResult Edit()
        {
            var session = (UserLogin)HttpContext.Session[CommonConstants.USER_SESSION];
            var quanTri = db.QuanTris.Where(x => x.username == session.UserName).SingleOrDefault();
            var nhanVien = db.NhanViens.Where(x => x.maNV == quanTri.maNV).SingleOrDefault();
            ViewBag.ngaySinh = nhanVien.ngaySinh.ToShortDateString();
            return View(nhanVien);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
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
            ViewBag.ngaySinh = nhanVien.ngaySinh.ToShortDateString();
            return View(nhanVien);
        }
        public ActionResult DoiMatKhau()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DoiMatKhau(ChangePass model)
        {
            if(ModelState.IsValid)
            {
                var session = (UserLogin)HttpContext.Session[CommonConstants.USER_SESSION];
                var quanTri = db.QuanTris.Where(x => x.username == session.UserName).SingleOrDefault();
                if(model.newPass != model.confirmPass)
                {
                    ModelState.AddModelError("", "Mật khẩu mới không khớp.");
                    return View();
                }
                if (Encryptor.MD5Hash(model.newPass) == quanTri.password)
                {
                    ModelState.AddModelError("", "Mật khẩu mới phải khác mật khẩu cũ.");
                    return View();
                }
                if (Encryptor.MD5Hash(model.oldPass) == quanTri.password)
                {
                    quanTri.password = Encryptor.MD5Hash(model.newPass);
                    db.Entry(quanTri).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "Mật khẩu cũ không đúng.");
            }
            return View();
        }
    }
}