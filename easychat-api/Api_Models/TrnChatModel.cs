using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace easychat_api.Api_Models
{
    public class TrnChatModel
    {
        public Int32 Id { get; set; }
        public String ChatDate { get; set; }
        public String ChatName { get; set; }
        public Int32 ReceiverId { get; set; }
        public String ReceiverUserName { get; set; }
        public Int32 CreatedByUserId { get; set; }
        public String CreatedByUserFullName { get; set; }
    }
}