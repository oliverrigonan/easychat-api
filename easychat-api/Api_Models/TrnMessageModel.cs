using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace easychat_api.Api_Models
{
    public class TrnMessageModel
    {
        public Int32 Id { get; set; }
        public Int32? ChatId { get; set; }
        public Int32 UserId { get; set; }
        public String UserFullName { get; set; }
        public String Message { get; set; }
        public String MessageDateTime { get; set; }
        public Boolean IsRead { get; set; }
        public String ReadDateTime { get; set; }
    }
}