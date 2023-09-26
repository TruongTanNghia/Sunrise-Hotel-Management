using DoAnHMS.Common;
using DoAnHMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace DoAnHMS.Controllers
{
    public class LoginController : Controller
    {
        private readonly DoAnHMSEntities db = new DoAnHMSEntities();
        public int CheckUser(string username, string password)
        {
            var kq = db.QuanTris.Where(x => x.username == username && x.password == password).SingleOrDefault();
            if (kq != null)
            {
                if(kq.tinhTrang == true)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return 0;
            }
        }

        public List<string> GetListCredential(string username)
        {
            var user = db.QuanTris.Single(x => x.username == username);
            var data = (from a in db.DanhSachQuyens
                        join b in db.NhomNhanViens on a.IDNhom equals b.IDNhom
                        join c in db.Quyens on a.IDQuyen equals c.IDQuyen
                        where user.IDNhom == b.IDNhom
                        select new
                        {
                            a.IDQuyen,
                            a.IDNhom
                        }).AsEnumerable().Select(x => new DanhSachQuyen()
                        {
                            IDQuyen = x.IDQuyen,
                            IDNhom = x.IDNhom
                        });
            return data.Select(x => x.IDQuyen).ToList();
        }
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginModel model)
        {
            QuanTri qt = new QuanTri();
            if (ModelState.IsValid)
            {
                qt.username = model.UserName;
                qt.password = Encryptor.MD5Hash(model.Password);
                if (CheckUser(qt.username, qt.password) == 1)
                {
                    var user = db.QuanTris.Single(x => x.username == qt.username);
                    var userSession = new UserLogin
                    {
                        UserName = qt.username,
                        IDNhom = user.IDNhom,
                        Avatar = user.NhanVien.hinhAnh
                    };
                    Session.Add(CommonConstants.USER_SESSION, userSession);
                    var listCredentials = GetListCredential(model.UserName);
                    Session.Add(CommonConstants.SESSION_CREDENTIAL, listCredentials);
                    HttpContext.Response.Cookies.Clear();
                    FormsAuthentication.SetAuthCookie(model.UserName, false);
                    HttpCookie cookie = FormsAuthentication.GetAuthCookie(model.UserName, false);
                    cookie.Path = "/";
                    HttpContext.Response.Cookies.Add(cookie);
                    if (model.RememberMe)
                    {
                        FormsAuthentication.SetAuthCookie(model.UserName, true);
                    }
                    return RedirectToAction("Index", "Home");
                }
                else if(CheckUser(qt.username, qt.password) == -1)
                {
                    ModelState.AddModelError("", "Tài khoản của bạn đang bị tạm khóa.");
                }
                else
                    ModelState.AddModelError("", "Tên đăng nhập hoặc tài khoản không đúng.");
            }
            return View(model);
        }

        public ActionResult Logout()
        {
            Session[CommonConstants.USER_SESSION] = null;
            return RedirectToAction("Index", "Login");
        }
    }
}