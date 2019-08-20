using System;
using System.Collections.Generic;
using System.Linq;
using AFollestad.MaterialDialogs;
using Android.Gms.Ads;
using Android.Graphics;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using Java.Lang;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.Fonts;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDateClient.Requests;
using Xamarin.RangeSlider;
using Exception = System.Exception;

namespace QuickDate.ButtomSheets
{
    public class SearchFilterBottomDialogFragment : BottomSheetDialogFragment, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback
    {
        #region Variables Basic

        public TextView Title,IconBack,IconLocation,LocationTextView,LocationPlace,LocationMoreIcon,IconFire,GenderTextView,IconAge,AgeTextView, AgeNumberTextView, OnlineTextView,IconOnline, ResetTextView;
        public RelativeLayout LocationLayout;
        public Button ButtonMan , ButtonGirls, ButtonBoth, ButtonApply;
        public RangeSliderControl AgeSeekBar;
        public Switch OnlineSwitch;

        public int AgeMin = 18, AgeMax = 75;
        public string Gender = "0", Location = "";
        public bool SwitchState = true;
        public HomeActivity GlobalContext;
        public AdView MAdView;

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
                View view = inflater.Inflate(Resource.Layout.ButtomSheetSearchFilter, container, false);
                 
                //Get Value And Set Toolbar
                InitComponent(view);
                InitAdView(view);

                IconBack.Click += IconBackOnClick;
                LocationLayout.Click += LocationLayoutOnClick; 
                ButtonMan.Click += ButtonManOnClick;
                ButtonGirls.Click += ButtonGirlsOnClick;
                ButtonBoth.Click += ButtonBothOnClick;
                ButtonApply.Click += ButtonApplyOnClick;
                ResetTextView.Click += ResetTextViewOnClick; 
                AgeSeekBar.DragCompleted += AgeSeekBarOnDragCompleted;

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
                Title = view.FindViewById<TextView>(Resource.Id.titlepage);
                IconBack = view.FindViewById<TextView>(Resource.Id.IconBack);
                IconLocation = view.FindViewById<TextView>(Resource.Id.IconLocation);
                LocationTextView = view.FindViewById<TextView>(Resource.Id.LocationTextView);
                LocationPlace = view.FindViewById<TextView>(Resource.Id.LocationPlace);
                LocationMoreIcon = view.FindViewById<TextView>(Resource.Id.LocationMoreIcon);
                GenderTextView = view.FindViewById<TextView>(Resource.Id.GenderTextView);
                IconFire = view.FindViewById<TextView>(Resource.Id.IconFire);
                IconAge = view.FindViewById<TextView>(Resource.Id.IconAge);
                AgeTextView = view.FindViewById<TextView>(Resource.Id.AgeTextView);
                AgeNumberTextView = view.FindViewById<TextView>(Resource.Id.Agenumber); 
                OnlineTextView = view.FindViewById<TextView>(Resource.Id.OnlineTextView);
                IconOnline = view.FindViewById<TextView>(Resource.Id.Icononline);
                ResetTextView = view.FindViewById<TextView>(Resource.Id.Resetbutton);
                LocationLayout = view.FindViewById<RelativeLayout>(Resource.Id.LayoutLocation);
                ButtonMan = view.FindViewById<Button>(Resource.Id.ManButton);
                ButtonGirls = view.FindViewById<Button>(Resource.Id.GirlsButton);
                ButtonBoth = view.FindViewById<Button>(Resource.Id.BothButton);
                ButtonApply = view.FindViewById<Button>(Resource.Id.ApplyButton);
                AgeSeekBar = view.FindViewById<RangeSliderControl>(Resource.Id.seekbar);
                OnlineSwitch = view.FindViewById<Switch>(Resource.Id.togglebutton);
                  
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, IconBack, IonIconsFonts.ChevronLeft);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, IconLocation, IonIconsFonts.IosLocationOutline);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, LocationMoreIcon, IonIconsFonts.ChevronRight);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, IconFire, IonIconsFonts.IosPersonOutline);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, IconAge, IonIconsFonts.Calendar);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, IconOnline, IonIconsFonts.Ionic);

                FontUtils.SetFont(LocationTextView, Fonts.SfRegular);
                FontUtils.SetFont(LocationPlace, Fonts.SfRegular);
                FontUtils.SetFont(Title, Fonts.SfMedium);
                FontUtils.SetFont(GenderTextView, Fonts.SfRegular);
                FontUtils.SetFont(AgeTextView, Fonts.SfRegular);
                FontUtils.SetFont(OnlineTextView, Fonts.SfRegular);

                AgeSeekBar.SetSelectedMinValue(18);
                AgeSeekBar.SetSelectedMaxValue(75);
                 
                OnlineSwitch.Checked = false;

                ButtonGirls.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                ButtonGirls.SetTextColor(Color.ParseColor("#444444"));

                ButtonBoth.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends_pressed);
                ButtonBoth.SetTextColor(Color.ParseColor("#ffffff"));

                ButtonMan.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                ButtonMan.SetTextColor(Color.ParseColor("#444444"));

                SetLocalData();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void InitAdView(View view)
        {
            try
            {
                MAdView = view.FindViewById<AdView>(Resource.Id.adView);
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
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Events

        //Save and sent data and set new search 
        private void ButtonApplyOnClick(object sender, EventArgs e)
        {
            try
            {
                // check current state of a Switch (true or false).
                SwitchState = OnlineSwitch.Checked;

                UserDetails.AgeMin = AgeMin = (int)AgeSeekBar.GetSelectedMinValue();
                UserDetails.AgeMax = AgeMax = (int)AgeSeekBar.GetSelectedMaxValue();
                UserDetails.Gender = Gender;
                UserDetails.Location = Location;
                UserDetails.SwitchState = SwitchState;

                if (GlobalContext?.TrendingFragment?.NearByAdapter?.NearByList?.Count > 0)
                {
                    GlobalContext.TrendingFragment.NearByAdapter.NearByList.Clear();
                    GlobalContext.TrendingFragment.NearByAdapter.NotifyDataSetChanged();
                }

                SetLocationUser();

                GlobalContext?.TrendingFragment?.LoadUsersJsonAsync();

                Dismiss();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Reset Value
        private void ResetTextViewOnClick(object sender, EventArgs e)
        {
            try
            {
                ButtonGirls.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                ButtonGirls.SetTextColor(Color.ParseColor("#444444"));

                ButtonBoth.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends_pressed);
                ButtonBoth.SetTextColor(Color.ParseColor("#ffffff"));

                ButtonMan.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                ButtonMan.SetTextColor(Color.ParseColor("#444444"));
                Gender = "0,1";

                LocationPlace.Text = GetText(Resource.String.Lbl_NearBy);

                AgeMin = 18;
                AgeMax = 75;
                  
                AgeNumberTextView.Text = AgeMin + " - " + AgeMax;
                  
                SwitchState = false;
                OnlineSwitch.Checked = false;

                Location = ListUtils.MyUserInfo?.FirstOrDefault()?.Country;
                 
                UserDetails.AgeMin = AgeMin;
                UserDetails.AgeMax = AgeMax;
                UserDetails.Gender = Gender;
                UserDetails.Location = Location;
                UserDetails.SwitchState = SwitchState;
                  
                if ((int) AgeSeekBar.GetSelectedMinValue() != 18)
                    AgeSeekBar.SetSelectedMinValue(18);

                if ((int)AgeSeekBar.GetSelectedMaxValue() != 75)
                    AgeSeekBar.SetSelectedMaxValue(75);
                 
                SetLocationUser(); 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Location
        private void LocationLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                string[] countriesArray = Context.Resources.GetStringArray(Resource.Array.countriesArray);

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(Context);

                foreach (var item in countriesArray)
                    arrayAdapter.Add(item);

                dialogList.Title(GetText(Resource.String.Lbl_Location));
                dialogList.Items(arrayAdapter);
                dialogList.PositiveText(GetText(Resource.String.Lbl_Close)).OnPositive(this);
                dialogList.AlwaysCallSingleChoiceCallback();
                dialogList.ItemsCallback(this).Build().Show(); 
             }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Back
        private void IconBackOnClick(object sender, EventArgs e)
        {
            try
            {
                Dismiss();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
          
        //Select gender >> Both (0,1)
        private void ButtonBothOnClick(object sender, EventArgs e)
        {
            try
            {
                //follow_button_profile_friends >> Un click
                //follow_button_profile_friends_pressed >> click
                ButtonBoth.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends_pressed);
                ButtonBoth.SetTextColor(Color.ParseColor("#ffffff"));
                 
                ButtonGirls.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                ButtonGirls.SetTextColor(Color.ParseColor("#444444"));

                ButtonMan.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                ButtonMan.SetTextColor(Color.ParseColor("#444444"));

                Gender = "0,1";
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Select gender >> Girls (1)
        private void ButtonGirlsOnClick(object sender, EventArgs e)
        {
            try
            {
                //follow_button_profile_friends >> Un click
                //follow_button_profile_friends_pressed >> click
                ButtonGirls.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends_pressed);
                ButtonGirls.SetTextColor(Color.ParseColor("#ffffff"));

                ButtonBoth.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                ButtonBoth.SetTextColor(Color.ParseColor("#444444"));

                ButtonMan.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                ButtonMan.SetTextColor(Color.ParseColor("#444444"));
                Gender = "1";
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Select gender >> Man (0)
        private void ButtonManOnClick(object sender, EventArgs e)
        {
            try
            {
                //follow_button_profile_friends >> Un click
                //follow_button_profile_friends_pressed >> click
                ButtonMan.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends_pressed);
                ButtonMan.SetTextColor(Color.ParseColor("#ffffff"));

                ButtonBoth.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                ButtonBoth.SetTextColor(Color.ParseColor("#444444"));

                ButtonGirls.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                ButtonGirls.SetTextColor(Color.ParseColor("#444444"));

                Gender = "0";
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Select Age SeekBar >> Right #Max and >> Left #Min
        private void AgeSeekBarOnDragCompleted(object sender, EventArgs e)
        {
            try
            {
                GC.Collect(GC.MaxGeneration);

                AgeMin = (int)AgeSeekBar.GetSelectedMinValue();
                AgeMax = (int)AgeSeekBar.GetSelectedMaxValue();

                AgeNumberTextView.Text = AgeMin + " - " + AgeMax;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

       #endregion

        #region MaterialDialog

        public void OnClick(MaterialDialog p0, DialogAction p1)
        {
            try
            {
                if (p1 == DialogAction.Positive)
                {

                }
                else if (p1 == DialogAction.Negative)
                {
                    p0.Dismiss();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
 
        public void OnSelection(MaterialDialog p0, View p1, int itemId, ICharSequence itemString)
        {
            try
            {
                var id = itemId;
                var text = itemString.ToString();

                string[] countriesArrayId = Context.Resources.GetStringArray(Resource.Array.countriesArray_id);

                var data = countriesArrayId[id];
                Location = data;
                LocationPlace.Text = text; 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        public void SetLocationUser()
        {
            try
            {
                var dictionary = new Dictionary<string, string>
                {
                    {"show_me_to", Location},
                };
                RequestsAsync.Users.UpdateProfileAsync(dictionary).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void SetLocalData()
        {
            try
            {
                // check current state of a Switch (true or false).
                AgeSeekBar.SetSelectedMinValue(UserDetails.AgeMin);
                AgeSeekBar.SetSelectedMaxValue(UserDetails.AgeMax);
                 
                OnlineSwitch.Checked = UserDetails.SwitchState;
               
                if (UserDetails.Gender == "0,1")
                {
                    ButtonBoth.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends_pressed);
                    ButtonBoth.SetTextColor(Color.ParseColor("#ffffff"));

                    ButtonGirls.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                    ButtonGirls.SetTextColor(Color.ParseColor("#444444"));

                    ButtonMan.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                    ButtonMan.SetTextColor(Color.ParseColor("#444444"));
                }
                else if (UserDetails.Gender == "1")
                {
                    ButtonGirls.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends_pressed);
                    ButtonGirls.SetTextColor(Color.ParseColor("#ffffff"));

                    ButtonBoth.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                    ButtonBoth.SetTextColor(Color.ParseColor("#444444"));

                    ButtonMan.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                    ButtonMan.SetTextColor(Color.ParseColor("#444444"));
                }
                else if (UserDetails.Gender == "0")
                {
                    ButtonMan.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends_pressed);
                    ButtonMan.SetTextColor(Color.ParseColor("#ffffff"));

                    ButtonBoth.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                    ButtonBoth.SetTextColor(Color.ParseColor("#444444"));

                    ButtonGirls.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                    ButtonGirls.SetTextColor(Color.ParseColor("#444444"));
                } 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }  
    }
}