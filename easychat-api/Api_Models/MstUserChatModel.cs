using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace easychat_api.Api_Models
{
    public class MstUserChatModel
    {
        public Int32 Id { get; set; }
        public Int32 SenderUserId { get; set; }
        public String SenderUserFullName { get; set; }
        public Int32 ReceiverUserId { get; set; }
        public String ReceiverUserFullName { get; set; }
        public Int32 ChatId { get; set; }
    }
}