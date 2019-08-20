using System;
using System.Linq;
using System.Timers;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AT.Markushi.UI;
using ME.Itangqi.Waveloadingview;
using QuickDate.Activities.Favorite;
using QuickDate.Activities.InviteFriends;
using QuickDate.Activities.MyProfile;
using QuickDate.Activities.Premium;
using QuickDate.Activities.SettingsUser;
using QuickDate.Helpers.CacheLoaders;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDate.SQLite;
using QuickDateClient;
using QuickDateClient.Classes.Authorization;
using QuickDateClient.Classes.Global;
using QuickDateClient.Requests;
using Console = System.Console;
using Fragment = Android.Support.V4.App.Fragment;

namespace QuickDate.Activities.Tabbes
{
    public class ProfileFragment : Fragment
    {
        #region Variables Basic

        public HomeActivity GlobalContext;
        public TextView Username, WalletNumber, TXtBoostMe, TxtUpgrade;
        public ImageView ProfileImage;
        public CircleButton EditButton, SettingsButton, BoostButton;
        public RelativeLayout WalletButton, PopularityButton, UpgradeButton, FavoriteButton, HelpButton, InviteButton;
        public LinearLayout HeaderSection;
        public FavoriteUserFragment FavoriteFragment;
        public TimerTime Time;
        private WaveLoadingView mWaveLoadingView;
        #endregion

        #region General

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
            GlobalContext = (HomeActivity)Activity;
        }
          
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                View view = inflater.Inflate(Resource.Layout.TProfileLayout, container, false);

                //Get Value 
                InitComponent(view); 
              
                WalletButton.Click += WalletButtonOnClick;
                PopularityButton.Click += PopularityButtonOnClick;
                UpgradeButton.Click += UpgradeButtonOnClick;
                EditButton.Click += EditButtonOnClick;
                ProfileImage.Click += ProfileImageOnClick;
                SettingsButton.Click += SettingsButtonOnClick;
                FavoriteButton.Click += FavoriteButtonOnClick;
                HelpButton.Click += HelpButtonOnClick;
                InviteButton.Click += InviteButtonOnClick;
                BoostButton.Click += BoostButtonOnClick;
                                 
                return view;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
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
         
        #region Functions

        public void InitComponent(View view)
        {
            try
            {
                ProfileImage = view.FindViewById<ImageView>(Resource.Id.Iconimage2);
                Username = view.FindViewById<TextView>(Resource.Id.username);
                WalletNumber = view.FindViewById<TextView>(Resource.Id.walletnumber);
                TxtUpgrade = view.FindViewById<TextView>(Resource.Id.upgradeText);
                TXtBoostMe = view.FindViewById<TextView>(Resource.Id.tv_Boost);
                EditButton = view.FindViewById<CircleButton>(Resource.Id.EditButton);
                SettingsButton = view.FindViewById<CircleButton>(Resource.Id.SettingsButton);
                BoostButton = view.FindViewById<CircleButton>(Resource.Id.BoostButton);
                WalletButton = view.FindViewById<RelativeLayout>(Resource.Id.walletSection);
                PopularityButton = view.FindViewById<RelativeLayout>(Resource.Id.popularitySection);
                UpgradeButton = view.FindViewById<RelativeLayout>(Resource.Id.upgradeSection);
                FavoriteButton = view.FindViewById<RelativeLayout>(Resource.Id.StFirstLayout);
                InviteButton = view.FindViewById<RelativeLayout>(Resource.Id.StsecoundLayout);
                HelpButton = view.FindViewById<RelativeLayout>(Resource.Id.StthirdLayout);
                HeaderSection = view.FindViewById<LinearLayout>(Resource.Id.headerSection);

                mWaveLoadingView = (WaveLoadingView)view.FindViewById(Resource.Id.waveLoadingView);
                mWaveLoadingView.Visibility = ViewStates.Gone;


                //ROEO REQUISITOS PARA SER CLIENTE VERIFICADO
                var dataUser = ListUtils.MyUserInfo.FirstOrDefault(a => a.Id == UserDetails.UserId);
                if (dataUser.PhoneVerified == 0)
                {
                    Toast.MakeText(Context, Context.GetText(Resource.String.Verifyprofile), ToastLength.Long).Show();
                }
                //FIN

                BoostButton.Tag = "Off";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        #endregion

        #region Events

        //Open edit my info
        private void EditButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                StartActivity(new Intent(Activity, typeof(EditProfileActivity)));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Open Upgrade
        private void UpgradeButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                var window = new PopupController(Activity);
                window.DisplayPremiumWindow();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Open Popularity >> Very Low
        private void PopularityButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                StartActivity(new Intent(Context, typeof(PopularityActivity)));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Open Wallet
        private void WalletButtonOnClick(object sender, EventArgs e)
        {
            try
            { 
                var window = new PopupController(Activity);
                window.DisplayCreditWindow("credits");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Boost me
        private async void BoostButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                var dataUser = ListUtils.MyUserInfo.FirstOrDefault(a => a.Id == UserDetails.UserId);
                if (dataUser != null)
                {
                    if (BoostButton.Tag.ToString() == "Off")
                    {
                        BoostButton.Tag = "Run";
                        string myBalance = dataUser.Balance ?? "0.00";
                        if (!string.IsNullOrEmpty(myBalance) && myBalance != "0.00")
                        {
                            //sent new api
                            (int apiStatus, var respond) = await RequestsAsync.Users.ManagePopularityAsync("boost").ConfigureAwait(false);
                            if (apiStatus == 200)
                            {
                                if (respond is AmountObject result)
                                {
                                    Activity.RunOnUiThread(() =>
                                    {
                                        try
                                        {
                                            myBalance = result.CreditAmount.ToString();
                                            WalletNumber.Text = result.CreditAmount.ToString();

                                            var timeBoost = ListUtils.SettingsSiteList.FirstOrDefault()?.BoostExpireTime ?? "4";
                                            var timeBoostMilliseconds = IMethods.Time.ConvertMinutesToMilliseconds(Convert.ToDouble(timeBoost));
                                            dataUser.BoostedTime = Convert.ToInt32(timeBoostMilliseconds);
                                            dataUser.IsBoosted = 1;

                                            GetMyInfoData();
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
                                        ApiRequest.Logout(Activity);
                                }
                            }
                            else if (apiStatus == 404)
                            {
                                var error = respond.ToString();
                                //Toast.MakeText(this, error, ToastLength.Short).Show();
                            }
                        }
                        else
                        {
                            var window = new PopupController(Activity);
                            window.DisplayCreditWindow("credits");
                        }
                    }
                    else
                    {
                        Toast.MakeText(Context,GetText(Resource.String.Lbl_YourBoostExpireInMinutes),ToastLength.Long).Show();
                    } 
                } 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Open Settings
        private void SettingsButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                StartActivity(new Intent(Context, typeof(SettingsActivity)));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        //Invite Friends
        private void InviteButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                StartActivity(new Intent(Context, typeof(InviteFriendsActivity)));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Help
        private void HelpButtonOnClick(object sender, EventArgs e)
        {
            try
            { 
                var intent = new Intent(Context, typeof(LocalWebViewActivity));
                intent.PutExtra("URL", Client.WebsiteUrl + "/contact");
                intent.PutExtra("Type", GetText(Resource.String.Lbl_Help));
                Activity.StartActivity(intent);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Favorite
        private void FavoriteButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                FavoriteFragment = new FavoriteUserFragment();
                GlobalContext.FragmentBottomNavigator.DisplayFragment(FavoriteFragment);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        public void GetMyInfoData()
        {
            try
            {
                if (ListUtils.MyUserInfo.Count == 0)
                {
                    var sqlEntity = new SqLiteDatabase();
                    sqlEntity.GetDataMyInfo();
                    sqlEntity.Dispose();
                }
                ApiRequest.GetInfoData(UserDetails.UserId.ToString()).ConfigureAwait(false);

                var data = ListUtils.MyUserInfo.FirstOrDefault(a => a.Id == UserDetails.UserId);
                if (data != null)
                {
                    GlideImageLoader.LoadImage(Activity,data.Avater, ProfileImage, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);

                    Username.Text = QuickDateTools.GetNameFinal(data);
                     
                    WalletNumber.Text = data.Balance.Replace(".00", "");

                    if (data.IsPro == "1")
                    {
                        #region UpgradeButton >> ViewStates.Gone

                        //UpgradeButton.Visibility = ViewStates.Gone;

                        //LinearLayout.LayoutParams layoutParam1 = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, 100, 1f);
                        //LinearLayout.LayoutParams layoutParam2 = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, 100, 1f);

                        //((ViewGroup)WalletButton.Parent)?.RemoveView(WalletButton);
                        //((ViewGroup)PopularityButton.Parent)?.RemoveView(PopularityButton);
                        //((ViewGroup)UpgradeButton.Parent)?.RemoveView(UpgradeButton);

                        //HeaderSection.WeightSum = 2;

                        //layoutParam1.TopMargin = 20;
                        //layoutParam2.TopMargin = 20;
                        //layoutParam2.MarginStart = 20;

                        //WalletButton.LayoutParameters = layoutParam1;
                        //PopularityButton.LayoutParameters = layoutParam2;

                        //HeaderSection.AddView(WalletButton, layoutParam1);
                        //HeaderSection.AddView(PopularityButton, layoutParam2); 

                        #endregion

                        switch (data.ProType)
                        {
                            case "1":
                                TxtUpgrade.Text = GetText(Resource.String.Lbl_Weekly);
                                break;
                            case "2":
                                TxtUpgrade.Text = GetText(Resource.String.Lbl_Monthly);
                                break;
                            case "3":
                                TxtUpgrade.Text = GetText(Resource.String.Lbl_Yearly);
                                break;
                            case "4":
                                TxtUpgrade.Text = GetText(Resource.String.Lbl_Lifetime);
                                break;
                            default:
                                TxtUpgrade.Text = GetText(Resource.String.Lbl_Upgrade);
                                break;
                        }
                    }
                    else
                    {
                        TxtUpgrade.Text = GetText(Resource.String.Lbl_Upgrade);
                        UpgradeButton.Visibility = ViewStates.Visible;
                    }
                     
                    if (data.BoostedTime != 0)
                    {
                        var timeBoost = ListUtils.SettingsSiteList.FirstOrDefault()?.BoostExpireTime ?? "4";
                        var timeBoostSeconds = IMethods.Time.ConvertMinutesToSeconds(Convert.ToDouble(timeBoost)); //240

                        
                        double progressStart = 0;
                        double progress = 100 / timeBoostSeconds; //0.4

                        if (Time == null)
                        { 
                           double progressPlus = 100 / timeBoostSeconds;
                             
                            Time = new TimerTime();
                            TimerTime.TimerCount = Time.GetTimer();
                            var plus1 = progressPlus;
                            TimerTime.TimerCount.Elapsed += delegate (object sender, ElapsedEventArgs args)
                            {
                                var plus = plus1;
                                Activity.RunOnUiThread(() =>
                                {
                                    try
                                    {
                                        var (minutes, seconds) = Time.TimerCountOnElapsed();
                                        if ((minutes == "" || minutes == "0") && (seconds == "" || seconds == "0"))
                                        {
                                            Time.SetStopTimer();
                                            Time = null;
                                            TimerTime.TimerCount = null;

                                            data.BoostedTime = 0;
                                            TXtBoostMe.Text = Context.GetText(Resource.String.Lbl_BoostMe);
                                            SetStopAnimationPopularity();
                                            progress = 0;
                                            progressStart = 0;
                                            mWaveLoadingView.CancelAnimation();

                                            BoostButton.Tag = "Off";
                                        }
                                        else
                                        {
                                            TXtBoostMe.Text = minutes + ":" + seconds;
                                            progress += plus;

                                            progressStart = Math.Round(progress,MidpointRounding.AwayFromZero); 
                                            mWaveLoadingView.ProgressValue = Convert.ToInt32(progressStart);
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e);
                                    }
                                });
                            }; 
                        }

                        string countTime = Time.CheckCountTime(Convert.ToInt32(data.BoostedTime));
                        if (countTime != "0:0" && !countTime.Contains("-") && !string.IsNullOrEmpty(countTime))
                        { 
                            int min = Convert.ToInt32(countTime.Split(":")[0]); 
                            int sec = Convert.ToInt32(countTime.Split(":")[1]); 
                            Time.SetMinutes(min);
                            Time.SetSeconds(sec);
                            Time.SetStartTimer();
                            TXtBoostMe.Text = countTime;

                            var minSeconds = IMethods.Time.ConvertMinutesToSeconds(Convert.ToDouble(min));

                            //start in here  
                            progress = (timeBoostSeconds - minSeconds) * 100 / timeBoostSeconds; 

                            SetStartAnimationPopularity(); 
                        }
                        else
                        {
                            Time.SetStopTimer();
                            Time = null;
                            TimerTime.TimerCount = null;

                            TXtBoostMe.Text = Context.GetText(Resource.String.Lbl_BoostMe); 
                            SetStopAnimationPopularity();

                            BoostButton.Tag = "Off";
                        }
                    }
                    else
                    {
                        if (Time != null)
                        {
                            Time.SetStopTimer();
                            Time = null;
                            TimerTime.TimerCount = null;

                            TXtBoostMe.Text = Context.GetText(Resource.String.Lbl_BoostMe);
                            SetStopAnimationPopularity();

                            BoostButton.Tag = "Off";
                        }
                    }
                } 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Update Avatar Async
        private void ProfileImageOnClick(object sender, EventArgs e)
        {
            try
            {
                GlobalContext.OpenDialogGallery();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void SetStartAnimationPopularity()
        {
            try
            {
                BoostButton.Visibility = ViewStates.Invisible;
                mWaveLoadingView.Visibility = ViewStates.Visible;
                mWaveLoadingView.StartAnimation();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void SetStopAnimationPopularity()
        {
            try
            {
                BoostButton.Visibility = ViewStates.Visible;
                mWaveLoadingView.Visibility = ViewStates.Gone;

                mWaveLoadingView?.CancelAnimation();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        } 
    }
}