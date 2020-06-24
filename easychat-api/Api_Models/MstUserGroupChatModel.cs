using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace easychat_api.Api_Models
{
    public class MstUserGroupChatModel
    {
        public Int32 Id { get; set; }
        public Int32 UserId { get; set; }
        public String UserFullName { get; set; }
        public Int32 GroupChatId { get; set; }
    }
}