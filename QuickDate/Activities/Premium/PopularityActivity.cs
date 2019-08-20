using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Fonts;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDate.SQLite;
using QuickDateClient.Classes.Authorization;
using QuickDateClient.Classes.Global;
using QuickDateClient.Requests;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace QuickDate.Activities.Premium
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class PopularityActivity : AppCompatActivity
    {
        #region Variables Basic

        public CollapsingToolbarLayout CollapsingToolbar;
        public AppBarLayout AppBarLayout;
        public Toolbar ActionBarToolBar;
        public TextView Circle1, Circle2, Circle3, TxtVisitsCost , TxtMatchesCost , TxtLikesCost;
        public Button BtnVisits, BtnMatches, BtnLikes;
        public HomeActivity GlobalContext;

        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                 
                IMethods.IApp.FullScreenApp(this);

                // Create your application here
                SetContentView(Resource.Layout.PopularityLayout);

                //Get Value And Set Toolbar
                InitComponent();
                InitToolbar();
                GetDataOption();

                GlobalContext = HomeActivity.GetInstance();
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
                CollapsingToolbar = FindViewById<CollapsingToolbarLayout>(Resource.Id.collapsingToolbar);
                CollapsingToolbar.Title = GetText(Resource.String.Lbl_IncreasePopularity);

                AppBarLayout = FindViewById<AppBarLayout>(Resource.Id.mainAppBarLayout);
                AppBarLayout.SetExpanded(true);

                Circle1 = FindViewById<TextView>(Resource.Id.circle);
                Circle2 = FindViewById<TextView>(Resource.Id.circle2);
                Circle3 = FindViewById<TextView>(Resource.Id.circle3);

                TxtVisitsCost = FindViewById<TextView>(Resource.Id.visitsCost);
                TxtMatchesCost = FindViewById<TextView>(Resource.Id.matchesCost);
                TxtLikesCost = FindViewById<TextView>(Resource.Id.likesCost);

                BtnVisits = FindViewById<Button>(Resource.Id.visitsButton);
                BtnMatches = FindViewById<Button>(Resource.Id.matchesButton);
                BtnLikes = FindViewById<Button>(Resource.Id.likesButton);

                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, Circle1, IonIconsFonts.Record);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, Circle2, IonIconsFonts.Record);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, Circle3, IonIconsFonts.Record);

                Circle1.SetTextColor(Color.ParseColor("#8e24aa"));
                Circle2.SetTextColor(Color.ParseColor("#3949ab"));
                Circle3.SetTextColor(Color.ParseColor("#1E88E5")); 
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
                ActionBarToolBar = (Toolbar)FindViewById(Resource.Id.maintoolbar);
                if (ActionBarToolBar != null)
                {
                    ActionBarToolBar.Title = GetText(Resource.String.Lbl_IncreasePopularity);
                    ActionBarToolBar.SetTitleTextColor(Color.White);
                    SetSupportActionBar(ActionBarToolBar);
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

        public void AddOrRemoveEvent(bool addEvent)
        {
            try
            {
                // true +=  // false -=
                if (addEvent)
                {
                    BtnVisits.Click += BtnVisitsOnClick;
                    BtnMatches.Click += BtnMatchesOnClick;
                    BtnLikes.Click += BtnLikesOnClick;
                }
                else
                {
                    BtnVisits.Click -= BtnVisitsOnClick;
                    BtnMatches.Click -= BtnMatchesOnClick;
                    BtnLikes.Click -= BtnLikesOnClick;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Events

        //Likes
        private async void BtnLikesOnClick(object sender, EventArgs e)
        {
            try
            {
                var myBalance = ListUtils.MyUserInfo.FirstOrDefault(a => a.Id == UserDetails.UserId)?.Balance ?? "0.00";
                if (!string.IsNullOrEmpty(myBalance) && myBalance != "0.00")
                {
                    //Show a progress
                    AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading));
                    //sent new api
                    (int apiStatus, var respond) = await RequestsAsync.Users.ManagePopularityAsync("likes").ConfigureAwait(false);
                    if (apiStatus == 200)
                    {
                        if (respond is AmountObject result)
                        {
                            RunOnUiThread(() =>
                            {
                                try
                                {
                                    myBalance = result.CreditAmount.ToString();

                                    if ( GlobalContext?.ProfileFragment.WalletNumber != null)
                                        GlobalContext.ProfileFragment.WalletNumber.Text = result.CreditAmount.ToString();
                                     
                                    BtnLikes.Text = GetText(Resource.String.Lbl_IncreaseStarted);
                                }
                                catch (Exception exception)
                                {
                                    Console.WriteLine(exception); 
                                } 
                            });
                        }
                    }
                    else if (apiStatus == 400)
                    {
                        if (respond is ErrorObject error)
                        {
                            var errorText = error.ErrorData.ErrorText;
                            if (errorText.Contains("Permission Denied"))
                                ApiRequest.Logout(this);
                        }
                    }
                    else if (apiStatus == 404)
                    {
                        var error = respond.ToString();
                        //Toast.MakeText(this, error, ToastLength.Short).Show();
                    }

                    AndHUD.Shared.Dismiss(this);
                }
                else
                {
                    var window = new PopupController(this);
                    window.DisplayCreditWindow("credits");
                } 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                AndHUD.Shared.Dismiss(this);
            }
        }

        //Matches
        private async void BtnMatchesOnClick(object sender, EventArgs e)
        {
            try
            {
                var myBalance = ListUtils.MyUserInfo.FirstOrDefault(a => a.Id == UserDetails.UserId)?.Balance ?? "0.00";
                if (!string.IsNullOrEmpty(myBalance) && myBalance != "0.00")
                {
                    //Show a progress
                    AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading));

                    //sent new api
                    (int apiStatus, var respond) = await RequestsAsync.Users.ManagePopularityAsync("matches").ConfigureAwait(false);
                    if (apiStatus == 200)
                    {
                        if (respond is AmountObject result)
                        {
                            RunOnUiThread(() =>
                            {
                                try
                                {
                                    myBalance = result.CreditAmount.ToString();

                                    if (GlobalContext?.ProfileFragment.WalletNumber != null)
                                        GlobalContext.ProfileFragment.WalletNumber.Text = result.CreditAmount.ToString();
                                     
                                    BtnMatches.Text = GetText(Resource.String.Lbl_IncreaseStarted);
                                }
                                catch (Exception exception)
                                {
                                    Console.WriteLine(exception); 
                                } 
                            });
                        }
                    }
                    else if (apiStatus == 400)
                    {
                        if (respond is ErrorObject error)
                        {
                            var errorText = error.ErrorData.ErrorText;
                            if (errorText.Contains("Permission Denied"))
                                ApiRequest.Logout(this);
                        }
                    }
                    else if (apiStatus == 404)
                    {
                        var error = respond.ToString();
                        //Toast.MakeText(this, error, ToastLength.Short).Show();
                    }

                    AndHUD.Shared.Dismiss(this);
                }
                else
                {
                    var window = new PopupController(this);
                    window.DisplayCreditWindow("credits");
                } 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                AndHUD.Shared.Dismiss(this);
            }
        }

        //Visits
        private async void BtnVisitsOnClick(object sender, EventArgs e)
        {
            try
            {
                var myBalance = ListUtils.MyUserInfo.FirstOrDefault(a => a.Id == UserDetails.UserId)?.Balance ?? "0.00";
                if (!string.IsNullOrEmpty(myBalance) && myBalance != "0.00")
                {
                    //Show a progress
                    AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading));

                    //sent new api
                    (int apiStatus, var respond) = await RequestsAsync.Users.ManagePopularityAsync("visits").ConfigureAwait(false);
                    if (apiStatus == 200)
                    {
                        if (respond is AmountObject result)
                        {
                            RunOnUiThread(() =>
                            {
                                try
                                {
                                    myBalance = result.CreditAmount.ToString();

                                    if (GlobalContext?.ProfileFragment != null)
                                        GlobalContext.ProfileFragment.WalletNumber.Text = result.CreditAmount.ToString();

                                    BtnVisits.Text = GetText(Resource.String.Lbl_IncreaseStarted);
                                }
                                catch (Exception exception)
                                {
                                    Console.WriteLine(exception);
                                }
                            });
                        }
                    }
                    else if (apiStatus == 400)
                    {
                        if (respond is ErrorObject error)
                        {
                            var errorText = error.ErrorData.ErrorText;
                            if (errorText.Contains("Permission Denied"))
                                ApiRequest.Logout(this);
                        }
                    }
                    else if (apiStatus == 404)
                    {
                        var error = respond.ToString();
                        //Toast.MakeText(this, error, ToastLength.Short).Show();
                    }

                    AndHUD.Shared.Dismiss(this);
                }
                else
                {
                    var window = new PopupController(this);
                    window.DisplayCreditWindow("credits");
                } 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                AndHUD.Shared.Dismiss(this);
            }
        }
        
        #endregion

        public void GetDataOption()
        {
            try
            {
                var option = ListUtils.SettingsSiteList.FirstOrDefault();
                if (option != null)
                {
                    TxtVisitsCost.Text = option.CostPerXvisits + " " + GetText(Resource.String.Lbl_Credits);
                    TxtMatchesCost.Text = option.CostPerXmatche + " " + GetText(Resource.String.Lbl_Credits);
                    TxtLikesCost.Text = option.CostPerXlike + " " + GetText(Resource.String.Lbl_Credits);
                }

                if (ListUtils.MyUserInfo.Count == 0)
                {
                    var sqlEntity = new SqLiteDatabase();
                    sqlEntity.GetDataMyInfo();
                    sqlEntity.Dispose();
                }

                var data = ListUtils.MyUserInfo.FirstOrDefault(a => a.Id == UserDetails.UserId);
                if (data != null)
                {
                    int xLikes = Convert.ToInt32(data.XlikesCreatedAt);
                    int xMatches = Convert.ToInt32(data.XmatchesCreatedAt);
                    int xVisits = Convert.ToInt32(data.XvisitsCreatedAt);
                    if (xLikes != 0)
                    {
                        BtnLikes.Text = GetText(Resource.String.Lbl_IncreaseStarted);
                    }
                    else if (xMatches != 0)
                    {
                        BtnMatches.Text = GetText(Resource.String.Lbl_IncreaseStarted);
                    }
                    else if (xVisits != 0)
                    {
                        BtnVisits.Text = GetText(Resource.String.Lbl_IncreaseStarted);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

    }
}