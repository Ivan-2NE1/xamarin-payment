using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Auth.Api;
using Android.Widget;
using Java.IO;
using QuickDate.Activities.Default;
using QuickDate.Activities.SettingsUser;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDate.OneSignal;
using QuickDate.SQLite;
using QuickDateClient;
using QuickDateClient.Classes.Common;
using QuickDateClient.Classes.Users;
using QuickDateClient.Requests;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;
using Console = System.Console;

namespace QuickDate.Helpers.Controller
{
    public class ApiRequest
    { 
        public static async Task GetSettings_Api()
        {
            try
            {
                if (IMethods.CheckConnectivity())
                {
                    (int apiStatus, var respond) = await Current.GetOptionsAsync();
                    if (apiStatus == 200)
                    {
                        if (respond is GetOptionsObject result)
                        {
                            if (result.data != null)
                            {
                                ListUtils.SettingsSiteList.Clear();
                                ListUtils.SettingsSiteList.Add(result.data);

                                SqLiteDatabase dbDatabase = new SqLiteDatabase();
                                dbDatabase.InsertOrUpdateSettings(result.data);
                                dbDatabase.Dispose();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
               Console.WriteLine(e);
            }
        }

        public static async Task<ProfileObject> GetInfoData(string userId)
        {
            try
            {
                if (IMethods.CheckConnectivity())
                {
                    var (apiStatus, respond) = await RequestsAsync.Users.ProfileAsync(userId, "data,media").ConfigureAwait(false);
                    if (apiStatus == 200)
                    {
                        if (respond is ProfileObject result)
                        {
                            if (userId == UserDetails.UserId.ToString())
                            {
                                UserDetails.Avatar = result.data.Avater; 
                                UserDetails.Username = result.data.Username;
                                UserDetails.FullName = result.data.Fullname;
                                UserDetails.IsPro = result.data.IsPro;
                                UserDetails.Url = Client.WebsiteUrl + "@" + result.data?.Username;
                                ListUtils.MyUserInfo.Clear();
                                ListUtils.MyUserInfo.Add(result.data);

                                SqLiteDatabase dbDatabase = new SqLiteDatabase();
                                dbDatabase.InsertOrUpdate_DataMyInfo(result.data);
                                dbDatabase.Dispose();
                                
                                return result;
                            }
                            else
                            {
                                return result;
                            }
                        }
                    }
                }

                return null;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }
        }
         
        public static async Task<(int,int)> GetCountNotifications()
        {
            try
            {
                if (IMethods.CheckConnectivity())
                {
                    (int apiStatus, var respond) = await RequestsAsync.Common.GetNotificationsAsync("25","");
                    if (apiStatus == 200)
                    {
                        if (respond is GetNotificationsObject result)
                        { 
                            return (result.NewNotificationCount, result.NewMessagesCount);
                        }
                    }
                }
                return (0, 0);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return (0, 0);
            }
        }

        public static async Task UpdateAvatarApi(string path)
        {
            try
            {
                if (IMethods.CheckConnectivity())
                {
                    (int apiStatus, var respond) = await RequestsAsync.Users.UpdateAvatarAsync(path);
                    if (apiStatus == 200)
                    {
                        if (respond is UpdateAvatarObject result)
                        {
                            var local = ListUtils.MyUserInfo.FirstOrDefault(a => a.Id == UserDetails.UserId);
                            if (local != null)
                            {
                                local.Avater = path;

                                SqLiteDatabase database = new SqLiteDatabase();
                                database.InsertOrUpdate_DataMyInfo(local);
                                database.Dispose();
                            }
                        }
                    }
                }
                else
                {
                    Toast.MakeText(Application.Context, Application.Context.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception); 
            }
        }
         
        public static async Task GetGifts()
        {
            try
            {
                if (IMethods.CheckConnectivity())
                {
                    var (apiStatus, respond) = await RequestsAsync.Common.GetGiftsAsync().ConfigureAwait(false);
                    if (apiStatus == 200)
                    {
                        if (respond is GetGiftsObject result)
                        {
                            if (result.Data.Count > 0)
                            {
                                ListUtils.GiftsList.Clear();
                                ListUtils.GiftsList = new ObservableCollection<DataFile>(result.Data);

                                SqLiteDatabase sqLiteDatabase = new SqLiteDatabase();
                                sqLiteDatabase.InsertAllGifts(ListUtils.GiftsList);
                                sqLiteDatabase.Dispose();

                                foreach (var item in result.Data)
                                {
                                    var url = item.File.Contains("media3.giphy.com/");
                                    if (url)
                                    {
                                        item.File = item.File.Replace(Client.WebsiteUrl, "");
                                    }
                                     
                                    IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskGif, item.File);
                                }  
                            } 
                        }
                    }
                }
                else
                {
                    Toast.MakeText(Application.Context, Application.Context.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception); 
            }
        }

        public static async Task GetStickers()
        {
            try
            {
                if (IMethods.CheckConnectivity())
                {
                    var (apiStatus, respond) = await RequestsAsync.Common.GetStickersAsync();
                    if (apiStatus == 200)
                    {
                        if (respond is GetStickersObject result)
                        {
                            if (result.Data.Count > 0)
                            {
                                ListUtils.StickersList.Clear();
                                ListUtils.StickersList = new ObservableCollection<DataFile>(result.Data);
                                 
                                SqLiteDatabase sqLiteDatabase = new SqLiteDatabase();
                                sqLiteDatabase.InsertAllStickers(ListUtils.StickersList);
                                sqLiteDatabase.Dispose();

                                foreach (var item in result.Data)
                                {
                                    var url = item.File.Contains("media3.giphy.com/");
                                    if (url)
                                    {
                                        item.File = item.File.Replace(Client.WebsiteUrl, "");
                                    }

                                    IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskSticker, item.File); 
                                } 
                            }
                        }
                    }
                }
                else
                {
                    Toast.MakeText(Application.Context, Application.Context.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);  
            }
        }


        public static bool RunLogout;

        public static async void Delete(Activity context)
        {
            try
            {
                if (RunLogout == false)
                {
                    RunLogout = true;

                    await RemoveData("Delete");

                    context.RunOnUiThread(() =>
                    {
                        IMethods.IPath.DeleteAll_MyFolderDisk();

                        SqLiteDatabase dbDatabase = new SqLiteDatabase();

                        Java.Lang.Runtime.GetRuntime().RunFinalization();
                        Java.Lang.Runtime.GetRuntime().Gc();
                        TrimCache(context);

                        dbDatabase.ClearAll();
                        dbDatabase.DropAll();

                        ListUtils.ClearAllList();

                        UserDetails.ClearAllValueUserDetails();

                        dbDatabase.CheckTablesStatus();
                        dbDatabase.Dispose();

                        MainSettings.SharedData.Edit().Clear().Commit();

                        Intent intent = new Intent(context, typeof(FirstActivity));
                        intent.AddCategory(Intent.CategoryHome);
                        intent.SetAction(Intent.ActionMain);
                        intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.ClearTask);
                        context.StartActivity(intent);
                        context.FinishAffinity();
                    });

                    RunLogout = false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        public static async void Logout(Activity context)
        {
            try
            {
                if (RunLogout == false)
                {
                    RunLogout = true;

                    await RemoveData("Logout");

                    context.RunOnUiThread(() =>
                    {
                        IMethods.IPath.DeleteAll_MyFolderDisk();

                        SqLiteDatabase dbDatabase = new SqLiteDatabase();

                        Java.Lang.Runtime.GetRuntime().RunFinalization();
                        Java.Lang.Runtime.GetRuntime().Gc();
                        TrimCache(context);

                        dbDatabase.ClearAll();
                        dbDatabase.DropAll();

                        ListUtils.ClearAllList();

                        UserDetails.ClearAllValueUserDetails();

                        dbDatabase.CheckTablesStatus();
                        dbDatabase.Dispose();

                        MainSettings.SharedData.Edit().Clear().Commit();

                        Intent intent = new Intent(context, typeof(FirstActivity));
                        intent.AddCategory(Intent.CategoryHome);
                        intent.SetAction(Intent.ActionMain);
                        intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.ClearTask);
                        context.StartActivity(intent);
                        context.FinishAffinity();
                    });

                    RunLogout = false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void TrimCache(Activity context)
        {
            try
            {
                File dir = context.CacheDir;
                if (dir != null && dir.IsDirectory)
                {
                    DeleteDir(dir);
                }

                context.DeleteDatabase("PlTube_Vi_dat.db");
                context.DeleteDatabase(SqLiteDatabase.PathCombine);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static bool DeleteDir(File dir)
        {
            try
            {
                if (dir == null || !dir.IsDirectory) return dir != null && dir.Delete();
                string[] children = dir.List();
                foreach (string child in children)
                {
                    bool success = DeleteDir(new File(dir, child));
                    if (!success)
                    {
                        return false;
                    }
                }

                // The directory is now empty so delete it
                return dir.Delete();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public static async Task RemoveData(string type)
        {
            try
            {
                if (type == "Logout")
                {
                    if (IMethods.CheckConnectivity())
                    {
                        await RequestsAsync.Auth.LogoutAsync();
                    }
                }
                else if (type == "Delete")
                {
                    IMethods.IPath.DeleteAll_MyFolder();

                    if (IMethods.CheckConnectivity())
                    {
                        await RequestsAsync.Auth.DeleteAccountAsync(UserDetails.Password);
                    }
                }

                try
                {
                    if (AppSettings.ShowGoogleLogin && LoginActivity.MGoogleApiClient != null)
                        if (Auth.GoogleSignInApi != null)
                            Auth.GoogleSignInApi.SignOut(LoginActivity.MGoogleApiClient);

                    if (AppSettings.ShowFacebookLogin)
                    {
                        var accessToken = AccessToken.CurrentAccessToken;
                        var isLoggedIn = accessToken != null && !accessToken.IsExpired;
                        if (isLoggedIn && Profile.CurrentProfile != null)
                        {
                            LoginManager.Instance.LogOut();
                        }
                    }
                     
                    OneSignalNotification.UnRegisterNotificationDevice();

                    UserDetails.ClearAllValueUserDetails();

                    
                    

                    

                    GC.Collect();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}