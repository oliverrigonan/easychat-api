using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace easychat_api.Api_Controllers
{
    [Authorize, RoutePrefix("api/chat")]
    public class ApiChatController : ApiController
    {
        public Api_Data.easychatdbDataContext db = new Api_Data.easychatdbDataContext();

        [HttpGet, Route("list")]
        public List<Api_Models.TrnChatModel> ChatList()
        {
            List<Api_Models.TrnChatModel> chatList = new List<Api_Models.TrnChatModel>();

            var currentUser = from d in db.MstUsers
                              where d.AspNetUserId == User.Identity.GetUserId()
                              select d;

            var userChats = from d in db.MstUserChats
                            where d.SenderUserId == currentUser.FirstOrDefault().Id
                            select d;

            if (userChats.Any())
            {
                foreach (var userChat in userChats)
                {
                    var chats = from d in db.TrnChats
                                where d.Id == userChat.ChatId
                                select d;

                    if (chats.Any())
                    {
                        chatList.Add(new Api_Models.TrnChatModel()
                        {
                            Id = chats.FirstOrDefault().Id,
                            ChatDate = chats.FirstOrDefault().ChatDate.ToShortDateString(),
                            ChatName = userChat.MstUser1.FullName,
                            CreatedByUserId = chats.FirstOrDefault().CreatedByUserId,
                            CreatedByUserFullName = chats.FirstOrDefault().MstUser.FullName
                        });
                    }
                }
            }

            var userGroupChats = from d in db.MstUserGroupChats
                                 where d.UserId == currentUser.FirstOrDefault().Id
                                 select d;

            if (userGroupChats.Any())
            {
                foreach (var userGroupChat in userGroupChats)
                {
                    var chats = from d in db.TrnChats
                                where d.Id == userGroupChat.ChatId
                                select d;

                    if (chats.Any())
                    {
                        chatList.Add(new Api_Models.TrnChatModel()
                        {
                            Id = chats.FirstOrDefault().Id,
                            ChatDate = chats.FirstOrDefault().ChatDate.ToShortDateString(),
                            ChatName = chats.FirstOrDefault().ChatName,
                            CreatedByUserId = chats.FirstOrDefault().CreatedByUserId,
                            CreatedByUserFullName = chats.FirstOrDefault().MstUser.FullName
                        });
                    }
                }
            }

            return chatList.OrderByDescending(d => d.ChatDate).ToList();
        }

        [HttpGet, Route("detail/{id}")]
        public Api_Models.TrnChatModel ChatDetail(String id)
        {
            var chatDetail = from d in db.TrnChats
                             where d.Id == Convert.ToInt32(id)
                             select new Api_Models.TrnChatModel
                             {
                                 Id = d.Id,
                                 ChatDate = d.ChatDate.ToShortDateString(),
                                 CreatedByUserId = d.CreatedByUserId,
                                 CreatedByUserFullName = d.MstUser.FullName
                             };

            return chatDetail.FirstOrDefault();
        }

        [HttpGet, Route("detail/user/{receiverUserId}")]
        public Api_Models.TrnChatModel ChatDetailPerUser(String receiverUserId)
        {
            Int32 chatId = 0;

            var senderUser = from d in db.MstUsers
                             where d.AspNetUserId == User.Identity.GetUserId()
                             select d;

            var senderUserChats = from d in db.MstUserChats
                                  where d.SenderUserId == senderUser.FirstOrDefault().Id
                                  && d.ReceiverUserId == Convert.ToInt32(receiverUserId)
                                  select d;

            if (senderUserChats.Any())
            {
                chatId = senderUserChats.FirstOrDefault().ChatId;
            }
            else
            {
                var receiverUserChats = from d in db.MstUserChats
                                        where d.SenderUserId == Convert.ToInt32(receiverUserId)
                                        && d.ReceiverUserId == senderUser.FirstOrDefault().Id
                                        select d;

                if (receiverUserChats.Any())
                {
                    chatId = receiverUserChats.FirstOrDefault().ChatId;
                }
            }

            var chatDetail = from d in db.TrnChats
                             where d.Id == Convert.ToInt32(chatId)
                             select new Api_Models.TrnChatModel
                             {
                                 Id = d.Id,
                                 ChatDate = d.ChatDate.ToShortDateString(),
                                 CreatedByUserId = d.CreatedByUserId,
                                 CreatedByUserFullName = d.MstUser.FullName
                             };

            return chatDetail.FirstOrDefault();
        }

        [HttpPost, Route("createRoom")]
        public HttpResponseMessage CreateRoom(Int32[] userIds)
        {
            try
            {
                var currentUser = from d in db.MstUsers
                                  where d.AspNetUserId == User.Identity.GetUserId()
                                  select d;

                if (userIds.Length > 0)
                {
                    String users = "";

                    foreach (var userId in userIds)
                    {
                        var user = from d in db.MstUsers
                                   where d.Id == userId
                                   select d;

                        if (user.Any())
                        {
                            users += user.FirstOrDefault().FullName + " ";
                        }
                    }

                    Api_Data.TrnChat newChat = new Api_Data.TrnChat()
                    {
                        ChatDate = DateTime.Today,
                        ChatName = users,
                        CreatedByUserId = currentUser.FirstOrDefault().Id
                    };

                    db.TrnChats.InsertOnSubmit(newChat);
                    db.SubmitChanges();

                    List<Api_Data.MstUserGroupChat> newUserGroupChat = new List<Api_Data.MstUserGroupChat>();
                    foreach (var userId in userIds)
                    {
                        newUserGroupChat.Add(new Api_Data.MstUserGroupChat()
                        {
                            ChatId = newChat.Id,
                            UserId = userId
                        });
                    }

                    db.MstUserGroupChats.InsertAllOnSubmit(newUserGroupChat);
                    db.SubmitChanges();

                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "No users.");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
