using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using Object = Java.Lang.Object;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace QuickDate.Activities
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class LocalWebViewActivity : AppCompatActivity
    {
        #region Variables Basic

        public static SwipeRefreshLayout SwipeRefreshLayout;
        public static WebView HybridView;
        public string Url = "" , TypeUrl = "";
        public AdView MAdView;

        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);

                // Create your application here
                SetContentView(Resource.Layout.LocalWebViewLayout);

                var data = Intent.GetStringExtra("URL") ?? "Data not available";
                if (data != "Data not available" && !string.IsNullOrEmpty(data))
                {
                    Url = data;
                }
                var type = Intent.GetStringExtra("Type") ?? "Data not available";
                if (type != "Data not available" && !string.IsNullOrEmpty(type))
                {
                    TypeUrl = type;
                }
                  
                //Get Value And Set Toolbar
                InitComponent();
                InitToolbar();
                SetWebView();
                InitAdView();
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
                AddOrRemoveEvent(true);

                MAdView?.Resume();
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
                AddOrRemoveEvent(false);

                MAdView?.Pause();
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
         
        protected override void OnDestroy()
        {
            try
            {
                
                
                
                MAdView?.Destroy();
                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region Menu

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        #endregion

        #region Functions

        public void InitComponent()
        {
            try
            {
                HybridView = FindViewById<WebView>(Resource.Id.LocalWebView);
                SwipeRefreshLayout = (SwipeRefreshLayout)FindViewById(Resource.Id.swipeRefreshLayout);
                SwipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight, Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight, Android.Resource.Color.HoloRedLight);
                SwipeRefreshLayout.Refreshing = true;
                SwipeRefreshLayout.Enabled = true;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void InitToolbar()
        {
            try
            {
                Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (toolbar != null)
                {
                    toolbar.Title = TypeUrl;
                    toolbar.SetTitleTextColor(Color.Black);
                    SetSupportActionBar(toolbar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void SetWebView()
        {
            try
            {
                //Set WebView and Load url to be rendered on WebView
                if (!IMethods.CheckConnectivity())
                {
                    HybridView.Visibility = ViewStates.Gone;

                    SwipeRefreshLayout.Refreshing = false;

                    Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long).Show();
                }
                else
                {
                    HybridView.Visibility = ViewStates.Visible;

                    //Set WebView
                    HybridView.SetWebViewClient(new MyWebViewClient(this));

                    //Load url to be rendered on WebView
                    HybridView.LoadUrl(Url);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void InitAdView()
        {
            try
            {
                MAdView = FindViewById<AdView>(Resource.Id.adView);
                var dataUser = ListUtils.MyUserInfo.FirstOrDefault(a => a.Id == UserDetails.UserId);
                if (dataUser?.IsPro == "0")
                {
                    if (AppSettings.ShowAdmobBanner)
                    {
                        MAdView.Visibility = ViewStates.Visible;
                        var adRequest = new AdRequest.Builder().Build();
                        MAdView.LoadAd(adRequest);
                    }
                    else
                    {
                        MAdView.Pause();
                        MAdView.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    MAdView.Pause();
                    MAdView.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        public void AddOrRemoveEvent(bool addEvent)
        {
            try
            {
                // true +=  // false -=
                if (addEvent)
                {
                    SwipeRefreshLayout.Refresh += SwipeRefreshLayoutOnRefresh;
                }
                else
                {
                    SwipeRefreshLayout.Refresh -= SwipeRefreshLayoutOnRefresh;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }



        #endregion

        #region Events

        //Event Refresh Data Page
        private void SwipeRefreshLayoutOnRefresh(object sender, EventArgs e)
        {
            try
            {
                HybridView.Reload();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        public class MyWebViewClient : WebViewClient, IValueCallback
        {
            public Activity MActivity;
            public MyWebViewClient(Activity mActivity)
            {
                MActivity = mActivity;
            }

            [Obsolete("deprecated")]
            public override bool ShouldOverrideUrlLoading(WebView view, string url)
            { 
                view.LoadUrl(url);
                return true;
            }

            public override void OnPageStarted(WebView view, string url, Bitmap favicon)
            {
                try
                {
                    base.OnPageStarted(view, url, favicon);
                    SwipeRefreshLayout.Refreshing = true;
                    SwipeRefreshLayout.Enabled = true;

                    view.Settings.JavaScriptEnabled = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                } 
            }

            public override void OnPageFinished(WebView view, string url)
            {
                try
                {
                    base.OnPageFinished(view, url);
                    SwipeRefreshLayout.Refreshing = false;
                    SwipeRefreshLayout.Enabled = false;

                    const string js = "javascript:" +
                                      "$('#nav-logged-in').hide();" +
                                      "$('#nav-not-logged-in').hide();" +
                                      "$('.page_footer').hide();" +
                                      "$('.page_footer').hide();" +
                                      "$('.col s12 m4 l3').hide();" +
                                      "$('.container.dt_terms .m4').hide();";

                    if (Build.VERSION.SdkInt >= (BuildVersionCodes) 19)
                    {
                        view.EvaluateJavascript(js, this);
                    }
                    else
                    {
                        view.LoadUrl(js);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            [Obsolete("deprecated")]
            public override void OnReceivedError(WebView view, ClientError errorCode, string description, string failingUrl)
            {
                try
                {
                    base.OnReceivedError(view, errorCode, description, failingUrl);
                    SwipeRefreshLayout.Refreshing = false;
                    SwipeRefreshLayout.Enabled = false;
                    const string js = "javascript:" +
                                      "$('#nav-logged-in').hide();" +
                                      "$('#nav-not-logged-in').hide();" +
                                      "$('.page_footer').hide();" +
                                      "$('.page_footer').hide();" +
                                      "$('.col s12 m4 l3').hide();" +
                                      "$('.container.dt_terms .m4').hide();";

                    if (Build.VERSION.SdkInt >= (BuildVersionCodes)19)
                    {
                        view.EvaluateJavascript(js, this);
                    }
                    else
                    {
                        view.LoadUrl(js);
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            public void OnReceiveValue(Object value)
            {
                try
                {

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        } 
    }
}