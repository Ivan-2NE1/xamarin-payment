using System;
using System.Net;
using System.Net.Http;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Java.Lang;
using Plugin.CurrentActivity;
using QuickDate.Activities;
using Exception = System.Exception;

using Firebase.Messaging;
using Firebase.Iid;
using Android.Util;

namespace QuickDate
{
	//You can specify additional application information in this attribute
    [Application]
    public class MainApplication : Application, Application.IActivityLifecycleCallbacks
    {
        public static MainApplication Instance;
        public Activity Activity;
        
        public MainApplication(IntPtr handle, JniHandleOwnership transer):base(handle, transer)
        {
        }

        public override void OnCreate()
        {

            try
            {
                base.OnCreate();
                RegisterActivityLifecycleCallbacks(this);
                //A great place to initialize Xamarin.Insights and Dependency Services!

                Instance = this;

                //Bypass Web Errors 
                //======================================
                if (AppSettings.TurnSecurityProtocolType3072On)
                {
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                    HttpClient client = new HttpClient(new Xamarin.Android.Net.AndroidClientHandler());
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
                }

                if (AppSettings.TurnTrustFailureOnWebException)
                {
                    //If you are Getting this error >>> System.Net.WebException: Error: TrustFailure /// then Set it to true
                    ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                    System.Security.Cryptography.AesCryptoServiceProvider b = new System.Security.Cryptography.AesCryptoServiceProvider();
                }
                 
                //App restarted after crash
                AndroidEnvironment.UnhandledExceptionRaiser += AndroidEnvironmentOnUnhandledeRaiser; 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            } 
        }
         
        private void AndroidEnvironmentOnUnhandledeRaiser(object sender, RaiseThrowableEventArgs e)
        {
            try
            {
                Intent intent = new Intent(Activity, typeof(SplashScreenActivity));
                intent.AddCategory(Intent.CategoryHome);
                intent.PutExtra("crash", true);
                intent.SetAction(Intent.ActionMain);
                intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.ClearTask);

                PendingIntent pendingIntent = PendingIntent.GetActivity(GetInstance().BaseContext, 0, intent, PendingIntentFlags.OneShot);
                AlarmManager mgr = (AlarmManager)GetInstance().BaseContext.GetSystemService(AlarmService);
                mgr.Set(AlarmType.Rtc, JavaSystem.CurrentTimeMillis() + 100, pendingIntent);

                Activity.Finish();
                JavaSystem.Exit(2);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
         
       
        public static MainApplication GetInstance()
        {
            return Instance;
        }
        

        public override void OnTerminate() // on stop
        {
            try
            {
                base.OnTerminate();
                UnregisterActivityLifecycleCallbacks(this); 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
 
        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
            try
            {
                Activity = activity;
                CrossCurrentActivity.Current.Activity = activity;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnActivityDestroyed(Activity activity)
        {
            Activity = activity;
        }

        public void OnActivityPaused(Activity activity)
        {
            Activity = activity;
        }

        public void OnActivityResumed(Activity activity)
        {
            try
            {
                Activity = activity;
                CrossCurrentActivity.Current.Activity = activity;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {
            Activity = activity;
        }

        public void OnActivityStarted(Activity activity)
        {
            try
            {
                Activity = activity;
                CrossCurrentActivity.Current.Activity = activity;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }           
        }

        public void OnActivityStopped(Activity activity)
        {
            Activity = activity;
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
    }
}