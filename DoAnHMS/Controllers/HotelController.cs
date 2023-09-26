using DoAnHMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnHMS.Controllers
{
    public class HotelController : Controller
    {
        private readonly DoAnHMSEntities db = new DoAnHMSEntities();

        #region Lấy mã
        string LayMaKH()
        {
            var maMAX = db.KhachHangs.ToList().Select(n => n.maKH).Max();
            int MaKH = int.Parse(maMAX.Substring(2)) + 1;
            string KH = String.Concat("000", MaKH.ToString());
            return "KH" + KH.Substring(MaKH.ToString().Length - 1);
        }
        string LayMaPDP()
        {
            var maMax = db.PhieuDatPhongs.ToList().Select(n => n.maPDP).Max();
            int maPDP = int.Parse(maMax.Substring(3)) + 1;
            string PDP = String.Concat("000", maPDP.ToString());
            return "PDP" + PDP.Substring(maPDP.ToString().Length - 1);
        }
        #endregion

        // GET: Hotel
        public ActionResult Index()
        {
            return View(db.Phongs.ToList());
        }

        public ActionResult DatPhong(string maphong)
        {
            var phong = db.Phongs.SingleOrDefault(n => n.maP.Equals(maphong));
            ViewBag.MaPhongID = phong.maP;
            ViewBag.MaPhong = phong.maP + " (" + phong.LoaiPhong.tenLP + ") ";
            return View();
        }

        [HttpPost]
        public ActionResult DatPhong(KhachHang khachhang, PhieuDatPhong phieudatphong, CTPhieuDatPhong ctphieudatphong)
        {
            // Kiểm tra khách hàng này đã từng đặt phòng hay chưa? (Thông qua cmnd_passport và số điện thoại)
            var checkUser = db.KhachHangs.FirstOrDefault(n => n.cmnd_passport.Equals(khachhang.cmnd_passport) && n.sdt.Equals(khachhang.sdt));
            if (checkUser == null) // trường hợp chưa tồn tại khách hàng trong hệ thống
            {
                // lưu thông tin khách hàng
                KhachHang kh = new KhachHang();
                kh.maKH = LayMaKH();
                kh.tenKH = khachhang.tenKH;
                kh.gioiTinh = true;
                kh.diaChi = "Việt Nam";
                kh.quocTich = "Việt Nam";
                kh.cmnd_passport = khachhang.cmnd_passport;
                kh.email = khachhang.email;
                kh.sdt = khachhang.sdt;
                db.KhachHangs.Add(kh);
                db.SaveChanges();
                // Lưu thông tin Phiếu đặt phòng
                PhieuDatPhong pdp = new PhieuDatPhong();
                pdp.maPDP = LayMaPDP();
                if (checkUser == null)
                {
                    pdp.maKH = kh.maKH;
                }
                pdp.ngayDen = phieudatphong.ngayDen;
                pdp.ngayDi = phieudatphong.ngayDi;
                pdp.tongTienCoc = 0;
                pdp.soNguoi = 1;
                pdp.tinhTrang = false;
                pdp.maNV = "NV0001";
                db.PhieuDatPhongs.Add(pdp);
                db.SaveChanges();

                // Lưu chi tiết phiếu thuê phòng
                CTPhieuDatPhong ctptp = new CTPhieuDatPhong();
                ctptp.maPDP = pdp.maPDP;
                ctptp.maP = ctphieudatphong.maP;
                ctptp.tienCoc = 0;
                db.CTPhieuDatPhongs.Add(ctptp);
                db.SaveChanges();
            }
            else // đã tồn tại khách hàng
            {
                // Lưu thông tin Phiếu đặt phòng
                PhieuDatPhong pdp = new PhieuDatPhong();
                pdp.maPDP = LayMaPDP();
                checkUser = null;
                pdp.maKH = checkUser.tenKH;
                pdp.ngayDen = phieudatphong.ngayDen;
                pdp.ngayDi = phieudatphong.ngayDi;
                pdp.tongTienCoc = 0;
                pdp.soNguoi = 1;
                pdp.tinhTrang = false;
                pdp.maNV = "NV0001";
                db.PhieuDatPhongs.Add(pdp);
                db.SaveChanges();

                // Lưu chi tiết phiếu thuê phòng
                CTPhieuDatPhong ctptp = new CTPhieuDatPhong();
                ctptp.maPDP = pdp.maPDP;
                ctptp.maP = ctphieudatphong.maP;
                ctptp.tienCoc = 0;
                db.CTPhieuDatPhongs.Add(ctptp);
                db.SaveChanges();
            }
            return RedirectToAction("Announce");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "hoTen,sdt,email")] LienHe lienHe)
        {
            if (ModelState.IsValid)
            {
                lienHe.ngayGui = DateTime.Now;
                lienHe.tinhTrang = false;
                db.LienHes.Add(lienHe);
                db.SaveChanges();
                return RedirectToAction("Announce");
            }
            return RedirectToAction("Index");
        }

        public ActionResult Announce()
        {
            return View();
        }

        public ActionResult PhanHoi()
        {
            return View();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PhanHoi([Bind(Include = "hoTen,sdt,email,noiDung")] PhanHoi phanHoi)
        {
            if (ModelState.IsValid)
            {
                phanHoi.ngayGui = DateTime.UtcNow;
                phanHoi.TinhTrang = false;
                db.PhanHois.Add(phanHoi);
                db.SaveChanges();
                return RedirectToAction("ThankYou");
            }
            return View(phanHoi);
        }

        public ActionResult ThankYou()
        {
            return View();
        }
    }
}