using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace easychat_api.Api_Controllers
{
    [Authorize, RoutePrefix("api/user")]
    public class ApiUserController : ApiController
    {
        public Api_Data.easychatdbDataContext db = new Api_Data.easychatdbDataContext();

        [HttpGet, Route("list")]
        public List<Api_Models.MstUserModel> UserList()
        {
            var userList = from d in db.MstUsers
                           select new Api_Models.MstUserModel
                           {
                               Id = d.Id,
                               UserName = d.UserName,
                               FullName = d.FullName
                           };

            return userList.ToList();
        }
    }
}
