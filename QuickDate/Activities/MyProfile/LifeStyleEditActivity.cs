using System;
using System.Collections.Generic;
using System.Linq;
using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using Java.Lang;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Fonts;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDate.SQLite;
using QuickDateClient.Classes.Global;
using QuickDateClient.Classes.Users;
using QuickDateClient.Requests;
using Exception = System.Exception;

namespace QuickDate.Activities.MyProfile
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class LifeStyleEditActivity : AppCompatActivity, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback
    {
        #region Variables Basic

        public TextView BackIcon, LiveWithIcon, CarIcon, ReligionIcon, SmokeIcon, DrinkIcon, TravelIcon;
        public EditText EdtLiveWith, EdtCar, EdtReligion, EdtSmoke, EdtDrink, EdtTravel;
        public Button BtnSave;
        public string TypeDialog;
        public int IdLiveWith, IdCar, IdReligion, IdSmoke, IdDrink, IdTravel;
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
                SetContentView(Resource.Layout.ButtomSheetLifeStyleEdit);

                //Get Value And Set Toolbar
                InitComponent();
                InitAdView();

                GetMyInfoData();
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

        #endregion

        #region Functions

        public void InitComponent()
        {
            try
            {
                BackIcon = FindViewById<TextView>(Resource.Id.IconBack);

                LiveWithIcon = FindViewById<TextView>(Resource.Id.IconLiveWith);
                EdtLiveWith = FindViewById<EditText>(Resource.Id.LiveWithEditText);

                CarIcon = FindViewById<TextView>(Resource.Id.IconCar);
                EdtCar = FindViewById<EditText>(Resource.Id.CarEditText);

                ReligionIcon = FindViewById<TextView>(Resource.Id.IconReligion);
                EdtReligion = FindViewById<EditText>(Resource.Id.ReligionEditText);

                SmokeIcon = FindViewById<TextView>(Resource.Id.IconSmoke);
                EdtSmoke = FindViewById<EditText>(Resource.Id.SmokeEditText);

                DrinkIcon = FindViewById<TextView>(Resource.Id.IconDrink);
                EdtDrink = FindViewById<EditText>(Resource.Id.DrinkEditText);

                TravelIcon = FindViewById<TextView>(Resource.Id.IconTravel);
                EdtTravel = FindViewById<EditText>(Resource.Id.TravelEditText);
                 
                BtnSave = FindViewById<Button>(Resource.Id.ApplyButton);

                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, BackIcon, IonIconsFonts.ChevronLeft);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, LiveWithIcon, FontAwesomeIcon.GlobeAmericas);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, CarIcon, FontAwesomeIcon.Car);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, ReligionIcon, FontAwesomeIcon.Church);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, SmokeIcon, FontAwesomeIcon.Smoking);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, DrinkIcon, FontAwesomeIcon.Beer);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, TravelIcon, FontAwesomeIcon.PlaneDeparture);

                EdtLiveWith.SetFocusable(ViewFocusability.NotFocusable);
                EdtCar.SetFocusable(ViewFocusability.NotFocusable);
                EdtReligion.SetFocusable(ViewFocusability.NotFocusable);
                EdtSmoke.SetFocusable(ViewFocusability.NotFocusable);
                EdtDrink.SetFocusable(ViewFocusability.NotFocusable);
                EdtTravel.SetFocusable(ViewFocusability.NotFocusable); 
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
                    BackIcon.Click += BackIconOnClick;
                    BtnSave.Click += BtnSaveOnClick;
                    EdtLiveWith.Click += EdtLiveWithOnClick;
                    EdtCar.Click += EdtCarOnClick;
                    EdtReligion.Click += EdtReligionOnClick;
                    EdtSmoke.Click += EdtSmokeOnClick;
                    EdtDrink.Click += EdtDrinkOnClick;
                    EdtTravel.Click += EdtTravelOnClick;  
                }
                else
                {
                    BackIcon.Click -= BackIconOnClick;
                    BtnSave.Click -= BtnSaveOnClick;
                    EdtLiveWith.Click -= EdtLiveWithOnClick;
                    EdtCar.Click -= EdtCarOnClick;
                    EdtReligion.Click -= EdtReligionOnClick;
                    EdtSmoke.Click -= EdtSmokeOnClick;
                    EdtDrink.Click -= EdtDrinkOnClick;
                    EdtTravel.Click -= EdtTravelOnClick;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        #endregion

        #region Events
         
        //Travel
        private void EdtTravelOnClick(object sender, EventArgs e)
        {
            try
            {
                TypeDialog = "Travel";
                string[] travelArray = Application.Context.Resources.GetStringArray(Resource.Array.TravelArray);

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(this);

                foreach (var item in travelArray)
                    arrayAdapter.Add(item);

                dialogList.Title(GetText(Resource.String.Lbl_Travel));
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

        //Drink
        private void EdtDrinkOnClick(object sender, EventArgs e)
        {
            try
            {
                TypeDialog = "Drink";
                string[] drinkArray = Application.Context.Resources.GetStringArray(Resource.Array.DrinkArray);

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(this);

                foreach (var item in drinkArray)
                    arrayAdapter.Add(item);
                
                dialogList.Title(GetText(Resource.String.Lbl_Drink));
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

        //Smoke
        private void EdtSmokeOnClick(object sender, EventArgs e)
        {
            try
            {
                TypeDialog = "Smoke";
                string[] smokeArray = Application.Context.Resources.GetStringArray(Resource.Array.SmokeArray);

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(this);

                foreach (var item in smokeArray)
                    arrayAdapter.Add(item);

                dialogList.Title(GetText(Resource.String.Lbl_Smoke));
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

        //Religion
        private void EdtReligionOnClick(object sender, EventArgs e)
        {
            try
            {
                TypeDialog = "Religion";
                string[] religionArray = Application.Context.Resources.GetStringArray(Resource.Array.ReligionArray);

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(this);

                foreach (var item in religionArray)
                    arrayAdapter.Add(item);

                dialogList.Title("Religion");
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

        //Car
        private void EdtCarOnClick(object sender, EventArgs e)
        {
            try
            {
                TypeDialog = "Car";
                string[] carArray = Application.Context.Resources.GetStringArray(Resource.Array.CarArray);

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(this);

                foreach (var item in carArray)
                    arrayAdapter.Add(item);

                dialogList.Title(GetString(Resource.String.Lbl_Car));
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

        //LiveWith
        private void EdtLiveWithOnClick(object sender, EventArgs e)
        {
            try
            {
                TypeDialog = "LiveWith";
                string[] liveWithArray = Application.Context.Resources.GetStringArray(Resource.Array.LiveWithArray);

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(this);

                foreach (var item in liveWithArray)
                    arrayAdapter.Add(item);

                dialogList.Title(GetText(Resource.String.Lbl_LiveWith));
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

        //Click save data and sent api
        private async void BtnSaveOnClick(object sender, EventArgs e)
        {
            try
            {
                if (IMethods.CheckConnectivity())
                {
                    //Show a progress
                    AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading));

                    var dictionary = new Dictionary<string, string>
                    {
                        {"live_with", IdLiveWith.ToString()},
                        {"car", IdCar.ToString()},
                        {"religion", IdReligion.ToString()},
                        {"smoke",IdSmoke.ToString()},
                        {"drink",IdDrink.ToString()},
                        {"travel",IdTravel.ToString()},
                    };

                    (int apiStatus, var respond) = await RequestsAsync.Users.UpdateProfileAsync(dictionary);
                    if (apiStatus == 200)
                    {
                        if (respond is UpdateProfileObject result)
                        {
                            var local = ListUtils.MyUserInfo.FirstOrDefault(a => a.Id == UserDetails.UserId);
                            if (local != null)
                            {
                                local.LiveWith = IdLiveWith;
                                local.Car = IdCar;
                                local.Religion = IdReligion;
                                local.Smoke = IdSmoke;
                                local.Drink = IdDrink;
                                local.Travel = IdTravel;

                                SqLiteDatabase database = new SqLiteDatabase();
                                database.InsertOrUpdate_DataMyInfo(local);
                                database.Dispose();

                            }

                            Toast.MakeText(this, GetText(Resource.String.Lbl_Done), ToastLength.Short).Show();
                            AndHUD.Shared.Dismiss(this);
                             
                            Intent resultIntent = new Intent();
                            SetResult(Result.Ok, resultIntent);
                            Finish();
                        }
                    }
                    else
                    {
                        //Show a Error image with a message
                        if (respond is ErrorObject error)
                        {
                            var errorText = error.ErrorData.ErrorText;
                            AndHUD.Shared.ShowError(this, errorText, MaskType.Clear, TimeSpan.FromSeconds(2));

                            if (errorText.Contains("Permission Denied"))
                                ApiRequest.Logout(this);
                        }
                    }
                    AndHUD.Shared.Dismiss(this);
                }
                else
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                AndHUD.Shared.Dismiss(this);
            } 
        }

        private void BackIconOnClick(object sender, EventArgs e)
        {
            try
            {
                Intent resultIntent = new Intent();
                SetResult(Result.Canceled, resultIntent);
                Finish();
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

                var dataUser = ListUtils.MyUserInfo.FirstOrDefault(a => a.Id == UserDetails.UserId);
                if (dataUser != null)
                {
                    string liveWith = QuickDateTools.GetLiveWith(Convert.ToInt32(dataUser.LiveWith));
                    if (IMethods.Fun_String.StringNullRemover(liveWith) != "-----")
                    {
                        IdLiveWith = Convert.ToInt32(dataUser.LiveWith);
                        EdtLiveWith.Text = liveWith;
                    }

                    string car = QuickDateTools.GetCar(Convert.ToInt32(dataUser.Car));
                    if (IMethods.Fun_String.StringNullRemover(car) != "-----")
                    {
                        IdCar = Convert.ToInt32(dataUser.Car);
                        EdtCar.Text = car;
                    }

                    string religion = QuickDateTools.GetReligion(Convert.ToInt32(dataUser.Religion));
                    if (IMethods.Fun_String.StringNullRemover(religion) != "-----")
                    {
                        IdReligion = Convert.ToInt32(dataUser.Religion);
                        EdtReligion.Text = religion;
                    }

                    string smoke = QuickDateTools.GetSmoke(Convert.ToInt32(dataUser.Smoke));
                    if (IMethods.Fun_String.StringNullRemover(smoke) != "-----")
                    {
                        IdSmoke = Convert.ToInt32(dataUser.Smoke);
                        EdtSmoke.Text = smoke;
                    }

                    string drink = QuickDateTools.GetDrink(Convert.ToInt32(dataUser.Drink));
                    if (IMethods.Fun_String.StringNullRemover(drink) != "-----")
                    {
                        IdDrink = Convert.ToInt32(dataUser.Drink);
                        EdtDrink.Text = drink;
                    }

                    string travel = QuickDateTools.GetTravel(Convert.ToInt32(dataUser.Travel));
                    if (IMethods.Fun_String.StringNullRemover(travel) != "-----")
                    {
                        IdTravel = Convert.ToInt32(dataUser.Travel);
                        EdtTravel.Text = travel;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            } 
        }

        #region MaterialDialog

        public void OnSelection(MaterialDialog p0, View p1, int itemId, ICharSequence itemString)
        {
            try
            {
                if (TypeDialog == "LiveWith")
                {
                    IdLiveWith = itemId + 1;
                    EdtLiveWith.Text = itemString.ToString();
                }
                else if (TypeDialog == "Car")
                {
                    IdCar = itemId + 1;
                    EdtCar.Text = itemString.ToString();
                }
                else if (TypeDialog == "Religion")
                {
                    IdReligion = itemId + 1;
                    EdtReligion.Text = itemString.ToString();
                }
                else if (TypeDialog == "Smoke")
                {
                    IdSmoke = itemId + 1;
                    EdtSmoke.Text = itemString.ToString();
                }
                else if (TypeDialog == "Drink")
                {
                    IdDrink = itemId + 1;
                    EdtDrink.Text = itemString.ToString();
                }
                else if (TypeDialog == "Travel")
                {
                    IdTravel = itemId + 1;
                    EdtTravel.Text = itemString.ToString();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

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

        #endregion

    }
}