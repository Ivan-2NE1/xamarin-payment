using System;
using System.Linq;
using System.Timers;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Com.Gigamole.Navigationtabbar.Ntb;
using Com.Theartofdev.Edmodo.Cropper;
using Java.IO;
using Newtonsoft.Json;
using QuickDate.Activities.Chat;
using QuickDate.ButtomSheets;
using QuickDate.Helpers.CacheLoaders;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDate.SQLite;
using QuickDateClient.Classes.Global;
using QuickDateClient.Requests;
using Console = System.Console;
using Uri = Android.Net.Uri;

namespace QuickDate.Activities.Tabbes
{
    [Activity(Icon = "@drawable/icon",Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Keyboard | ConfigChanges.Orientation | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenLayout | ConfigChanges.ScreenSize | ConfigChanges.SmallestScreenSize | ConfigChanges.UiMode)]
    public class HomeActivity : AppCompatActivity  
    { 
        #region Variables Basic

        public CardMachFragment CardFragment;
        public TrendingFragment TrendingFragment;
        public NotificationsFragment NotificationsFragment;
        public ProfileFragment ProfileFragment;
        public NavigationTabBar NavigationTabBar;
        public FragmentBottomNavigationView FragmentBottomNavigator;
        public Timer Timer;
        public string RunTimer = "Run", TypeAvatar = "";
        public static int CountNotificationsStatic = 0, CountMessagesStatic = 0;
        public static HomeActivity Instance;
        public TracksCounter TracksCounter;
        public PowerManager.WakeLock wl;

        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);

                AddFlagsWakeLock();

                // Create your application here
                SetContentView(Resource.Layout.TabbedMainLayout);

                Instance = this;

                TracksCounter = new TracksCounter(this);

                CardFragment = new CardMachFragment();
                TrendingFragment = new TrendingFragment();
                NotificationsFragment = new NotificationsFragment();
                ProfileFragment = new ProfileFragment();
                
                //Get Value
                SetupBottomNavigationView();
                 
                GetMyInfoData();

                //Run timer
                Timer = new Timer
                {
                    Interval = AppSettings.RefreshDataSeconds,
                    Enabled = true,
                };
                Timer.Elapsed += TimerOnElapsed;
                Timer.Start(); 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void OnResume()
        {
            try
            {
                base.OnResume();
               
                if (Timer != null)
                {
                    Timer.Enabled = true;
                    Timer.Start();
                }

                SetWakeLock();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void OnPause()
        {
            try
            {
                base.OnPause();
               
                if (Timer != null)
                {
                    Timer.Enabled = false;
                    Timer.Stop();
                }

                OffWakeLock();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            try
            { 
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                base.OnTrimMemory(level);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnLowMemory()
        {
            try
            {
                GC.Collect(GC.MaxGeneration);
                base.OnLowMemory();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static HomeActivity GetInstance()
        {
            try
            {
                return Instance;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        protected override void OnDestroy()
        {
            try
            {
                if (Timer != null)
                {
                    Timer.Enabled = false;
                    Timer.Stop();
                }

                ProfileFragment?.Time?.SetStopTimer();

                OffWakeLock();
                 
                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        #endregion

        #region Functions

        public void SetToolBar(Android.Support.V7.Widget.Toolbar toolbar, string title, bool showIconBack = true)
        {
            try
            {
                if (toolbar != null)
                {
                    if (!string.IsNullOrEmpty(title))
                        toolbar.Title = title;

                    toolbar.SetTitleTextColor(Color.Black);
                    SetSupportActionBar(toolbar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(showIconBack);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void SetupBottomNavigationView()
        {
            try
            {
                NavigationTabBar = FindViewById<NavigationTabBar>(Resource.Id.ntb_horizontal);
                FragmentBottomNavigator = new FragmentBottomNavigationView(this);

                CardFragment = new CardMachFragment();
                TrendingFragment = new TrendingFragment();
                NotificationsFragment = new NotificationsFragment();
                ProfileFragment = new ProfileFragment();

                FragmentBottomNavigator.FragmentListTab0.Add(CardFragment);
                FragmentBottomNavigator.FragmentListTab1.Add(TrendingFragment);
                FragmentBottomNavigator.FragmentListTab2.Add(NotificationsFragment);
                FragmentBottomNavigator.FragmentListTab4.Add(ProfileFragment);

                FragmentBottomNavigator.SetupNavigation(NavigationTabBar);
                NavigationTabBar.SetModelIndex(0, true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        #endregion

        #region Events

        //Open Chat 
        public void ShowChat()
        {
            try
            {
                //Convert to fragment 
                StartActivity(new Intent(this, typeof(LastChatActivity)));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void ShowMessagesBox(UserInfoObject dataUser)
        {
            try
            {
                Intent Int = new Intent(this, typeof(MessagesBoxActivity));
                Int.PutExtra("UserId", dataUser.Id.ToString());
                Int.PutExtra("TypeChat", "LastChat");
                Int.PutExtra("UserItem", JsonConvert.SerializeObject(dataUser));

                // Check if we're running on Android 5.0 or higher
                if ((int)Build.VERSION.SdkInt < 23)
                {
                    StartActivity(Int);
                }
                else
                {
                    //Check to see if any permission in our group is available, if one, then all are
                    if (CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted &&
                        CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Granted)
                    {
                        StartActivity(Int);

                    }
                    else
                        new PermissionsController(this).RequestPermission(100);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }


        public void OpenDialogGallery(string typeAvatar = "")
        {
            try
            {
                TypeAvatar = typeAvatar;
                // Check if we're running on Android 5.0 or higher
                if ((int)Build.VERSION.SdkInt < 23)
                {
                    IMethods.IPath.Chack_MyFolder();

                    //Open Image 
                    var myUri = Uri.FromFile(new File(IMethods.IPath.FolderDcimImage, IMethods.GetTimestamp(DateTime.Now) + ".jpeg"));
                    CropImage.Builder()
                        .SetInitialCropWindowPaddingRatio(0)
                        .SetAutoZoomEnabled(true)
                        .SetMaxZoom(4)
                        .SetGuidelines(CropImageView.Guidelines.On)
                        .SetCropMenuCropButtonTitle(GetText(Resource.String.Lbl_Done))
                        .SetOutputUri(myUri).Start(this);
                }
                else
                {
                    if (!CropImage.IsExplicitCameraPermissionRequired(this) && CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted &&
                        CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Granted && CheckSelfPermission(Manifest.Permission.Camera) == Permission.Granted)
                    {
                        IMethods.IPath.Chack_MyFolder();

                        //Open Image 
                        var myUri = Uri.FromFile(new File(IMethods.IPath.FolderDcimImage, IMethods.GetTimestamp(DateTime.Now) + ".jpeg"));
                        CropImage.Builder()
                            .SetInitialCropWindowPaddingRatio(0)
                            .SetAutoZoomEnabled(true)
                            .SetMaxZoom(4)
                            .SetGuidelines(CropImageView.Guidelines.On)
                            .SetCropMenuCropButtonTitle(GetText(Resource.String.Lbl_Done))
                            .SetOutputUri(myUri).Start(this);
                    }
                    else
                    {
                        //request Code 108
                        new PermissionsController(this).RequestPermission(108);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OpenAddPhotoFragment()
        {
            try
            {
                var addPhoto = new AddPhotoBottomDialogFragment();
                addPhoto.Show(SupportFragmentManager, "addPhoto");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Permissions && Result

        //Result
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);
                if (requestCode == 108 || requestCode == CropImage.CropImageActivityRequestCode)
                {
                    if (IMethods.CheckConnectivity())
                    {
                        var result = CropImage.GetActivityResult(data);
                        if (result.IsSuccessful)
                        {
                            var resultPathImage = result.Uri.Path;
                            if (!string.IsNullOrEmpty(resultPathImage))
                            {
                                if (TypeAvatar != "Avatar")
                                {
                                    GlideImageLoader.LoadImage(this,resultPathImage, ProfileFragment?.ProfileImage, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);
                                    ApiRequest.UpdateAvatarApi(resultPathImage).ConfigureAwait(false);
                                }
                                else
                                {
                                    //sent api 
                                    RequestsAsync.Users.UploadImageUserAsync(resultPathImage).ConfigureAwait(false); 
                                }
                                Toast.MakeText(this, GetText(Resource.String.Lbl_Done), ToastLength.Long).Show();
                            }
                        }
                        else
                        {
                            Toast.MakeText(this, GetText(Resource.String.Lbl_something_went_wrong), ToastLength.Long).Show();
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long).Show();
                    }
                }
                else if (requestCode == 1050) //1050
                {
                    //Get Location And Get Data Api
                    TrendingFragment?.CheckAndGetLocation();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Permissions
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            try
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                if (requestCode == 105)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        //Get Location And Get Data Api
                        TrendingFragment?.CheckAndGetLocation();
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long).Show();
                    }
                }
                else if (requestCode == 108)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        OpenDialogGallery(TypeAvatar);
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long).Show();
                    }
                }
                else if(requestCode == 110)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        Window.AddFlags(WindowManagerFlags.KeepScreenOn);
                    }
                    else
                    {
                        Toast.MakeText(this, this.GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long).Show();
                    }
                }
                else if(requestCode == 100)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        ShowMessagesBox(DialogController.DataUser);
                    }
                    else
                    {
                        Toast.MakeText(this, this.GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long).Show();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion
         
        #region Back Pressed 
      
        public override void OnBackPressed()
        {
            try
            {
                FragmentBottomNavigator.BackStackClickFragment();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Timer

        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                RunApiTimer();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public async void RunApiTimer()
        {
            try
            {
                if (RunTimer == "Run")
                {
                    RunTimer = "Off";

                    if (FragmentBottomNavigator.Models != null)
                    {
                        var (countNotifications, countMessages) = await ApiRequest.GetCountNotifications().ConfigureAwait(false);
                        NavigationTabBar.Model tabNotifications = FragmentBottomNavigator.Models.First(a => a.Title == GetText(Resource.String.Lbl_Notifications));
                        NavigationTabBar.Model tabMessages = FragmentBottomNavigator.Models.First(a => a.Title == GetText(Resource.String.Lbl_messages));
                        if (tabNotifications != null && countNotifications != 0 && countNotifications != CountNotificationsStatic)
                        {
                            RunOnUiThread(() =>
                            {
                                try
                                {
                                    CountNotificationsStatic = countNotifications;
                                    tabNotifications.BadgeTitle = countNotifications.ToString();
                                    tabNotifications.UpdateBadgeTitle(countNotifications.ToString());
                                    tabNotifications.ShowBadge(); 
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);
                                } 
                            });
                        }

                        if (tabMessages != null && countMessages != 0 && countMessages != CountMessagesStatic)
                        {
                            RunOnUiThread(() =>
                            {
                                try
                                {
                                    CountMessagesStatic = countMessages;
                                    tabMessages.BadgeTitle = countMessages.ToString();
                                    tabMessages.UpdateBadgeTitle(countMessages.ToString());
                                    tabMessages.ShowBadge();
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);
                                } 
                            });
                        }

                       await ApiRequest.GetInfoData(UserDetails.UserId.ToString()).ConfigureAwait(false);
                    } 
                }
                RunTimer = "Run";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                RunTimer = "Run";
            }
        }
         
        #endregion

        public async void GetMyInfoData()
        {
            try
            {
                var sqlEntity = new SqLiteDatabase();
                sqlEntity.GetDataMyInfo();
                sqlEntity.GetSettings();

                var listFavorite = sqlEntity.GetDataFavorite();
                
                if (ListUtils.FavoriteUserList.Count == 0 && listFavorite != null)
                    ListUtils.FavoriteUserList = listFavorite; 

                sqlEntity.Dispose();

                if (ListUtils.SettingsSiteList.Count == 0)
                  await ApiRequest.GetSettings_Api().ConfigureAwait(false);

                await ApiRequest.GetInfoData(UserDetails.UserId.ToString()).ConfigureAwait(false);
                  
                RunApiTimer(); 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

     
        #region WakeLock System

        public void AddFlagsWakeLock()
        {
            try
            {
                if ((int)Build.VERSION.SdkInt < 23)
                {
                    Window.AddFlags(WindowManagerFlags.KeepScreenOn);
                }
                else
                {
                    if (CheckSelfPermission(Manifest.Permission.WakeLock) == Permission.Granted)
                    {
                        Window.AddFlags(WindowManagerFlags.KeepScreenOn);
                    }
                    else
                    {
                        //request Code 110
                        new PermissionsController(this).RequestPermission(110);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void SetWakeLock()
        {
            try
            {
                if (wl == null)
                {
                    PowerManager pm = (PowerManager)GetSystemService(PowerService);
                    wl = pm.NewWakeLock(WakeLockFlags.ScreenBright, "My Tag");
                    wl.Acquire();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OffWakeLock()
        {
            try
            {
                // ..screen will stay on during this section..
                wl?.Release();
                wl = null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
          
        #endregion
         
    }
}