using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using QuickDate.Activities.Default;
using QuickDate.Activities.SettingsUser;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.CacheLoaders;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Utils;
using QuickDate.OneSignal;
using QuickDate.SQLite;
using QuickDateClient;

namespace QuickDate.Activities
{
    [Activity(MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/SplashScreenTheme", NoHistory = true, ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class SplashScreenActivity : Activity
    {
        #region Variables Basic

        public SqLiteDatabase DbDatabase;
        public AppSettings AppSettings;

        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                View mContentView = Window.DecorView;
                var uiOptions = (int)mContentView.SystemUiVisibility;
                var newUiOptions = (int)uiOptions;

                newUiOptions |= (int)SystemUiFlags.Fullscreen;
                newUiOptions |= (int)SystemUiFlags.HideNavigation;
                mContentView.SystemUiVisibility = (StatusBarVisibility)newUiOptions;

                Window.AddFlags(WindowManagerFlags.Fullscreen);

                AppSettings = new AppSettings();
                DbDatabase = new SqLiteDatabase();
                DbDatabase.CheckTablesStatus();

                ClassMapper.SetMappers();

                var client = new Client(AppSettings.TripleDesAppServiceProvider);
                GlideImageLoader.InitImageLoader();

                MainSettings.Init();

                if (AppSettings.Lang != "")
                { 
                    LangController.SetApplicationLang(this, AppSettings.Lang);
                }
                else
                {
                    var langLocale = Resources.Configuration.Locale;
                    LangController.SetAppLanguage(this, langLocale.Language);
                } 
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
                Task startupWork = new Task(SimulateStartup);
                startupWork.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void SimulateStartup()
        {
            try
            {
                FirstRunExcite();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        public void FirstRunExcite()
        {
            try
            {
                DbDatabase.GetSettings();
                OneSignalNotification.RegisterNotificationDevice();

                var result = DbDatabase.Get_data_Login_Credentials();
                if (result != null)
                {
                    Current.AccessToken = result.AccessToken;

                    switch (result.Status)
                    {
                        case "Active":
                        case "Pending":
                            StartActivity(new Intent(Application.Context, typeof(HomeActivity)));
                            break;
                        default:
                            StartActivity(new Intent(Application.Context, typeof(FirstActivity)));
                            break;
                    }
                }
                else
                {
                    StartActivity(new Intent(Application.Context, typeof(FirstActivity)));
                }
                DbDatabase.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Toast.MakeText(this, e.Message, ToastLength.Short).Show();
            }
        }
    }
}