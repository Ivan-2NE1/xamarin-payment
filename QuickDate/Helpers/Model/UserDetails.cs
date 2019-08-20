using System;
using QuickDate.Activities.Tabbes;

namespace QuickDate.Helpers.Model
{
    public static class UserDetails
    {
        public static string AccessToken = "";
        public static int UserId = 0;
        public static string Username = "";
        public static string FullName = "";
        public static string Password = "";
        public static string Email = "";
        public static string Cookie = "";
        public static string Status = "";
        public static string Avatar = "";
        public static string Cover = "";
        public static string DeviceId = "";
        public static string Lang = "";
        public static string IsPro = "";
        public static string Url = "";

        public static string Lat = "";
        public static string Lng = "";
        public static string Located = "35";
        public static int AgeMin = 18, AgeMax = 75;
        public static string Gender = "0,1", Location = "";
        public static bool SwitchState = false;
         
        public static bool NotificationPopup { get; set; } = true;
         
        public static int UnixTimestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        public static string Time = UnixTimestamp.ToString();

        public static void ClearAllValueUserDetails()
        {
            try
            {
                AccessToken = string.Empty;
                UserId = 0;
                Username = string.Empty;
                FullName = string.Empty;
                Password = string.Empty;
                Email = string.Empty;
                Cookie = string.Empty;
                Status = string.Empty;
                Avatar = string.Empty;
                Cover = string.Empty;
                DeviceId = string.Empty;
                Lang = string.Empty;
                Lat = string.Empty;
                Lng = string.Empty;
                Located = string.Empty;
                Gender = "0,1";
                Location = string.Empty;
                SwitchState = true;

                AgeMin = 18;
                AgeMax = 75;

                HomeActivity.CountNotificationsStatic = 0;
                HomeActivity.CountMessagesStatic = 0;
            }
            catch (Exception e)
            {
               Console.WriteLine(e);
            }
        }

    }
}