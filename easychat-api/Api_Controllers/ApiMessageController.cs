using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace easychat_api.Api_Controllers
{
    [Authorize, RoutePrefix("api/message")]
    public class ApiMessageController : ApiController
    {
        public Api_Data.easychatdbDataContext db = new Api_Data.easychatdbDataContext();

        [HttpGet, Route("list/{chatId}")]
        public List<Api_Models.TrnMessageModel> MessageList(String chatId)
        {
            var messages = from d in db.TrnMessages
                           where d.ChatId == Convert.ToInt32(chatId)
                           select new Api_Models.TrnMessageModel
                           {
                               Id = d.Id,
                               ChatId = d.ChatId,
                               UserId = d.UserId,
                               UserFullName = d.MstUser.FullName,
                               Message = d.Message,
                               MessageDateTime = d.MessageDateTime.ToShortDateString(),
                               IsRead = d.IsRead,
                               ReadDateTime = d.ReadDateTime.ToShortDateString()
                           };

            return messages.ToList();
        }

        [HttpPost, Route("send/{chatId}/{receiverUserId}")]
        public HttpResponseMessage SendMessage(String chatId, String receiverUserId, Api_Models.TrnMessageModel objMessage)
        {
            try
            {
                var senderUser = from d in db.MstUsers
                                 where d.AspNetUserId == User.Identity.GetUserId()
                                 select d;

                Int32 newChatId = 0;

                if (Convert.ToInt32(chatId) == 0)
                {
                    Api_Data.TrnChat newChat = new Api_Data.TrnChat()
                    {
                        ChatDate = DateTime.Today,
                        CreatedByUserId = senderUser.FirstOrDefault().Id
                    };

                    db.TrnChats.InsertOnSubmit(newChat);
                    db.SubmitChanges();

                    Api_Data.MstUserChat newSenderUserChat = new Api_Data.MstUserChat()
                    {
                        SenderUserId = senderUser.FirstOrDefault().Id,
                        ReceiverUserId = Convert.ToInt32(receiverUserId),
                        ChatId = newChat.Id
                    };

                    db.MstUserChats.InsertOnSubmit(newSenderUserChat);
                    db.SubmitChanges();

                    Api_Data.MstUserChat newReceiverUserChat = new Api_Data.MstUserChat()
                    {
                        SenderUserId = Convert.ToInt32(receiverUserId),
                        ReceiverUserId = senderUser.FirstOrDefault().Id,
                        ChatId = newChat.Id
                    };

                    db.MstUserChats.InsertOnSubmit(newReceiverUserChat);
                    db.SubmitChanges();

                    newChatId = newChat.Id;
                }
                else
                {
                    newChatId = Convert.ToInt32(chatId);

                    var senderUserChat = from d in db.MstUserChats
                                         where d.SenderUserId == senderUser.FirstOrDefault().Id
                                         && d.ReceiverUserId == Convert.ToInt32(receiverUserId)
                                         select d;

                    if (!senderUserChat.Any())
                    {
                        Api_Data.MstUserChat newSenderUserChat = new Api_Data.MstUserChat()
                        {
                            SenderUserId = senderUser.FirstOrDefault().Id,
                            ReceiverUserId = Convert.ToInt32(receiverUserId),
                            ChatId = newChatId
                        };

                        db.MstUserChats.InsertOnSubmit(newSenderUserChat);
                        db.SubmitChanges();
                    }

                    var receiverUserChats = from d in db.MstUserChats
                                            where d.SenderUserId == Convert.ToInt32(receiverUserId)
                                            && d.ReceiverUserId == senderUser.FirstOrDefault().Id
                                            select d;

                    if (!receiverUserChats.Any())
                    {
                        Api_Data.MstUserChat newReceiverUserChat = new Api_Data.MstUserChat()
                        {
                            SenderUserId = Convert.ToInt32(receiverUserId),
                            ReceiverUserId = senderUser.FirstOrDefault().Id,
                            ChatId = newChatId
                        };

                        db.MstUserChats.InsertOnSubmit(newReceiverUserChat);
                        db.SubmitChanges();
                    }
                }

                Api_Data.TrnMessage newMessage = new Api_Data.TrnMessage()
                {
                    UserId = senderUser.FirstOrDefault().Id,
                    ChatId = newChatId,
                    Message = objMessage.Message,
                    MessageDateTime = DateTime.Now,
                    IsRead = false,
                    ReadDateTime = DateTime.Now
                };

                db.TrnMessages.InsertOnSubmit(newMessage);
                db.SubmitChanges();

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
