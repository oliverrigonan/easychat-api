using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace easychat_api.Api_Models
{
    public class TrnGroupChatModel
    {
        public Int32 Id { get; set; }
        public String GroupChatDate { get; set; }
        public String GroupName { get; set; }
        public Int32 CreatedByUserId { get; set; }
        public String CreatedByUserFullName { get; set; }
    }
}