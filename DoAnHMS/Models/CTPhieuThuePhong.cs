//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DoAnHMS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class CTPhieuThuePhong
    {
        public string maPTP { get; set; }
        public string maP { get; set; }
        public System.DateTime ngaySD { get; set; }
        public string maDV { get; set; }
        public int soLuong { get; set; }
    
        public virtual DichVu DichVu { get; set; }
        public virtual Phong Phong { get; set; }
        public virtual PhieuThuePhong PhieuThuePhong { get; set; }
    }
}