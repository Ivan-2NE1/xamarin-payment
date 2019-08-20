using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Support.V4.App;
using Android.Widget;
using Com.OneSignal.Abstractions;
using Com.OneSignal.Android;
using Org.Json;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.Model;
using OSNotification = Com.OneSignal.Abstractions.OSNotification;
using OSNotificationPayload = Com.OneSignal.Abstractions.OSNotificationPayload;

namespace QuickDate.OneSignal
{
    public static class OneSignalNotification
    {
        //Force your app to Register notifcation derictly without loading it from server (For Best Result)

        public static string OneSignalAPP_ID = "edf94b48-7647-4e89-97b8-a023b45c1be0";
        public static string userid;
       
        public static void RegisterNotificationDevice()
        {
            try
            {
                if (UserDetails.NotificationPopup)
                { 
                    if (OneSignalAPP_ID != "")
                    {
                        Com.OneSignal.OneSignal.Current.StartInit(OneSignalAPP_ID)
                            .InFocusDisplaying(OSInFocusDisplayOption.Notification)
                            .HandleNotificationReceived(HandleNotificationReceived)
                            .HandleNotificationOpened(HandleNotificationOpened)
                            .EndInit();
                        Com.OneSignal.OneSignal.Current.IdsAvailable(IdsAvailable);
                        Com.OneSignal.OneSignal.Current.RegisterForPushNotifications();

                        AppSettings.ShowNotification = true;
                    }
                }
                else
                {
                    UnRegisterNotificationDevice();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static void UnRegisterNotificationDevice()
        {
            try
            {
                Com.OneSignal.OneSignal.Current.SetSubscription(false);
                AppSettings.ShowNotification = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void IdsAvailable(string userID, string pushToken)
        {
            try
            {
                UserDetails.DeviceId = userID;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void HandleNotificationReceived(OSNotification notification)
        {
            try
            {

                OSNotificationPayload payload = notification.payload;
                Dictionary<string, object> additionalData = payload.additionalData;
                 
                string message = payload.body;

            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.ToString(), ToastLength.Long).Show(); //Allen
                Console.WriteLine(ex);
            }
        }

        private static void HandleNotificationOpened(OSNotificationOpenedResult result)
        {
            try
            {
                OSNotificationPayload payload = result.notification.payload;
                Dictionary<string, object> additionalData = payload.additionalData;
                string message = payload.body;
                string actionID = result.action.actionID;

                if (additionalData != null)
                {
                    foreach (var item in additionalData)
                    {
                        //if (item.Key == "user_id")
                        //{
                        //    userid = item.Value.ToString();
                        //}
                        //if (item.Key == "notification_info")
                        //{
                        //    notificationInfo = JsonConvert.DeserializeObject<OneSignalObject.NotificationInfoObject>(item.Value.ToString());
                        //}
                        //if (item.Key == "user_data")
                        //{
                        //    userData = JsonConvert.DeserializeObject<OneSignalObject.UserDataObject>(item.Value.ToString());
                        //}
                        //if (item.Key == "url")
                        //{
                        //    string url = item.Value.ToString();
                        //}
                    }

                    //to : do
                    //go to activity or fragment depending on data

                    Intent intent = new Intent(Application.Context, typeof(HomeActivity));
                    intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                    intent.AddFlags(ActivityFlags.SingleTop);
                    intent.SetAction(Intent.ActionView);
                    //intent.PutExtra("TypeNotification", notificationInfo.TypeText);
                    Application.Context.StartActivity(intent);

                    if (additionalData.ContainsKey("discount"))
                    {
                        // Take user to your store..

                    }
                }
                if (actionID != null)
                {
                    // actionSelected equals the id on the button the user pressed.
                    // actionSelected will equal "__DEFAULT__" when the notification itself was tapped when buttons were present.  
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }

    public class NotificationExtenderServiceHandeler : NotificationExtenderService, NotificationCompat.IExtender
    {
        protected override void OnHandleIntent(Intent intent)
        {

        }

        protected override bool OnNotificationProcessing(OSNotificationReceivedResult p0)
        {
            OverrideSettings overrideSettings = new OverrideSettings();
            overrideSettings.Extender = new NotificationCompat.CarExtender();

            Com.OneSignal.Android.OSNotificationPayload payload = p0.Payload;
            JSONObject additionalData = payload.AdditionalData;

            if (additionalData.Has("room_name"))
            {
                string room_name = additionalData.Get("room_name").ToString();
                string Call_type = additionalData.Get("call_type").ToString();
                string Call_id = additionalData.Get("call_id").ToString();
                string From_id = additionalData.Get("from_id").ToString();
                string to_id = additionalData.Get("to_id").ToString();

                return false;
            }
            else
            {
                return true;
            }
        }

        public NotificationCompat.Builder Extend(NotificationCompat.Builder builder)
        {
            return builder;
        }
    }
}