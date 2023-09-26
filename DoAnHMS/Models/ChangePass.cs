using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DoAnHMS.Models
{
    public class ChangePass
    {
        [Display(Name = "Mật khẩu cũ")]
        [Required(ErrorMessage = "Bạn chưa nhập mật khẩu cũ")]
        public string oldPass { get; set; }

        [Display(Name = "Mật khẩu mới")]
        [Required(ErrorMessage = "Bạn chưa nhập mật khẩu mới")]
        public string newPass { get; set; }

        [Display(Name = "Nhập lại mật khẩu mới")]
        [Required(ErrorMessage = "Bạn chưa nhập lại mật khẩu mới")]
        public string confirmPass { get; set; }
    }
}