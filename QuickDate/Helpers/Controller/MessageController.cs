using System;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Widget;
using QuickDate.Activities.Chat;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDate.SQLite;
using QuickDateClient.Classes.Chat;
using QuickDateClient.Classes.Global;
using QuickDateClient.Requests;

namespace QuickDate.Helpers.Controller
{
    public class MessageController
    {
        //############# DON'T MODIFY HERE #############
        //========================= Functions =========================

        public static async Task SendMessageTask(int userId, string text, string stickerId, string path, string hashId, UserInfoObject userData)
        {
            try
            {
                var (apiStatus, respond) = await RequestsAsync.Chat.SendMessageAsync(userId.ToString(), text, stickerId, path, hashId);
                if (apiStatus == 200)
                {
                    if (respond is SendMessageObject result)
                    {
                        if (result.data != null)
                        {
                            UpdateLastIdMessage(result, userData );
                        }
                    }
                }
                else if (apiStatus == 400)
                {
                    if (respond is ErrorObject error)
                    {
                        var errorText = error.ErrorData.ErrorText;
                        Toast.MakeText(Application.Context, errorText, ToastLength.Short);
                    }
                }
                else if (apiStatus == 404)
                {
                    var error = respond.ToString();
                    Toast.MakeText(Application.Context, error, ToastLength.Short);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void UpdateLastIdMessage(SendMessageObject messages, UserInfoObject userData)
        {
            try
            {
                var checker = MessagesBoxActivity.MAdapter.MessageList.FirstOrDefault(a => a.Id == Convert.ToInt32(messages.HashId));
                if (checker != null)
                {
                    checker.Id = messages.data.Id;
                    checker.FromName = UserDetails.FullName;
                    checker.FromAvater = UserDetails.Avatar;
                    checker.ToName = userData?.Fullname ?? "";
                    checker.ToAvater = userData?.Avater ?? "";
                    checker.From = messages.data.From;
                    checker.To = messages.data.To;
                    checker.Text = messages.data.Text;
                    checker.Media = messages.data.Media;
                    checker.FromDelete = messages.data.FromDelete;
                    checker.ToDelete = messages.data.ToDelete;
                    checker.Sticker = messages.data.Sticker;
                    checker.CreatedAt = messages.data.CreatedAt;
                    checker.Seen = messages.data.Seen;
                    checker.Type = "Sent";
                    checker.MessageType = messages.data.MessageType;

                    string text = messages.data.Text;

                    switch (checker.MessageType)
                    {
                        case "text":
                        {
                            text = string.IsNullOrEmpty(text) ? Application.Context.GetText(Resource.String.Lbl_SendMessage) : messages.data.Text;
                            break;
                        }
                        case "media":
                        {
                            text = Application.Context.GetText(Resource.String.Lbl_SendImageFile);
                            break;
                        }
                        case "sticker" when checker.Sticker.Contains(".gif"):
                        {
                            text = Application.Context.GetText(Resource.String.Lbl_SendGifFile);
                            break;
                        }
                        case "sticker":
                        {
                            text = Application.Context.GetText(Resource.String.Lbl_SendStickerFile);
                            break;
                        }
                    }

                    var dataUser = LastChatActivity.MAdapter.UserList?.FirstOrDefault(a => a.User.Id == messages.data.To);
                    if (dataUser != null)
                    { 
                        var index = LastChatActivity.MAdapter.UserList?.IndexOf(LastChatActivity.MAdapter.UserList?.FirstOrDefault(x => x.User.Id == messages.data.To));
                        if (index > -1)
                        { 
                            dataUser.Text = text;

                            LastChatActivity.MAdapter.UserList.Move(Convert.ToInt32(index), 0);
                            LastChatActivity.MAdapter.NotifyItemMoved(Convert.ToInt32(index), 0);

                            var data = LastChatActivity.MAdapter.UserList.FirstOrDefault(a => a.User.Id == dataUser.User.Id);
                            if (data != null)
                            {
                                data.Id = dataUser.Id;
                                data.Owner = dataUser.Owner;
                                data.User = dataUser.User;
                                data.Seen = dataUser.Seen;
                                data.Text = dataUser.Text;
                                data.Media = dataUser.Media;
                                data.Sticker = dataUser.Sticker;
                                data.Time = dataUser.Time;
                                data.CreatedAt = dataUser.CreatedAt;
                                data.NewMessages = dataUser.NewMessages;
                                data.MessageType = dataUser.MessageType;

                                LastChatActivity.MAdapter.NotifyItemChanged(LastChatActivity.MAdapter.UserList.IndexOf(data));
                            } 
                        }
                    }
                    else
                    {
                        if (userData != null)
                        { 
                            LastChatActivity.MAdapter?.UserList?.Insert(0,new GetConversationListObject.Data()
                            {
                                Id = userData.Id,
                                Owner = 0,
                                User = userData,
                                Seen = 1,
                                Text = text,
                                Media = messages.data.Media,
                                Sticker = messages.data.Sticker,
                                Time = messages.data.CreatedAt,
                                CreatedAt = messages.data.CreatedAt,
                                NewMessages = 0
                            });

                            LastChatActivity.MAdapter?.NotifyItemInserted(0);
                        }
                    }

                    SqLiteDatabase dbDatabase = new SqLiteDatabase();
                    GetChatConversationsObject.Messages message = new GetChatConversationsObject.Messages
                    {
                        Id = messages.data.Id,
                        FromName = UserDetails.FullName,
                        FromAvater = UserDetails.Avatar,
                        ToName = userData?.Fullname ?? "",
                        ToAvater = userData?.Avater ?? "",
                        From = messages.data.From,
                        To = messages.data.To,
                        Text = messages.data.Text,
                        Media = messages.data.Media,
                        FromDelete = messages.data.FromDelete,
                        ToDelete = messages.data.ToDelete,
                        Sticker = messages.data.Sticker,
                        CreatedAt = messages.data.CreatedAt,
                        Seen = messages.data.Seen,
                        Type = "Sent",
                        MessageType = messages.data.MessageType, 
                    };
                    //Update All data users to database
                    dbDatabase.InsertOrUpdateToOneMessages(message);
                    dbDatabase.Dispose();

                    MessagesBoxActivity.UpdateOneMessage(message); 
                    MessagesBoxActivity.GetInstance()?.ChatBoxRecyclerView.ScrollToPosition(MessagesBoxActivity.MAdapter.MessageList.IndexOf(MessagesBoxActivity.MAdapter.MessageList.Last()));

                    if (AppSettings.RunSoundControl)
                        IMethods.AudioRecorderAndPlayer.PlayAudioFromAsset("Popup_SendMesseges.mp3");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

    }
}