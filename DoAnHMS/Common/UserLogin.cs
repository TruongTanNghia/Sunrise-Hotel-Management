using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAnHMS.Common
{
    [Serializable]
    public class UserLogin
    {
        public string UserName { get; set; }
        public string IDNhom { get; set; }
        public string Avatar { get; set; }
    }
}