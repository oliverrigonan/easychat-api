using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace easychat_api.Api_Models
{
    public class MstUserModel
    {
        public Int32 Id { get; set; }
        public String UserName { get; set; }
        public String Password { get; set; }
        public String FullName { get; set; }
    }
}