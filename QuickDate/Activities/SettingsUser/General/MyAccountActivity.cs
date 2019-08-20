using System;
using System.Collections.Generic;
using System.Linq;
using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using Java.Lang;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Fonts;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDate.SQLite;
using QuickDateClient.Classes.Global;
using QuickDateClient.Classes.Users;
using QuickDateClient.Requests;
using Exception = System.Exception;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace QuickDate.Activities.SettingsUser.General
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MyAccountActivity : AppCompatActivity, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback
    {
        #region Variables Basic

        public TextView BackIcon, UsernameIcon, EmailIcon, PhoneNumberIcon, CountryIcon;
        public EditText EdtUsername, EdtEmail, EdtPhoneNumber, EdtCountry;
        public Button BtnSave;
        public Toolbar Toolbar;
        public string Country;
        public HomeActivity GlobalContext;
        public AdView MAdView;

        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                // Create your application here
                SetContentView(Resource.Layout.MyAccountLayout);


                //Get Value And Set Toolbar
                InitComponent();
                InitToolbar();
                InitAdView();

                GetMyInfoData();

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
                BackIcon = FindViewById<TextView>(Resource.Id.IconBack);

                UsernameIcon = FindViewById<TextView>(Resource.Id.IconUsername);
                EdtUsername = FindViewById<EditText>(Resource.Id.UsernameEditText);

                EmailIcon = FindViewById<TextView>(Resource.Id.IconEmail);
                EdtEmail = FindViewById<EditText>(Resource.Id.EmailEditText);

                PhoneNumberIcon = FindViewById<TextView>(Resource.Id.IconPhoneNumber);
                EdtPhoneNumber = FindViewById<EditText>(Resource.Id.PhoneNumberEditText);

                CountryIcon = FindViewById<TextView>(Resource.Id.IconCountry);
                EdtCountry = FindViewById<EditText>(Resource.Id.CountryEditText);
                 
                BtnSave = FindViewById<Button>(Resource.Id.ApplyButton);

                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, BackIcon, IonIconsFonts.ChevronLeft);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, UsernameIcon, FontAwesomeIcon.Users);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, EmailIcon, FontAwesomeIcon.At);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, PhoneNumberIcon, FontAwesomeIcon.Phone);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, CountryIcon, FontAwesomeIcon.Flag);

                EdtCountry.SetFocusable(ViewFocusability.NotFocusable);

                //ROEO SI LA CUENTA YA  ESTA VERIFICADA  NO PERMITE EDITAR
                //var dataUser = ListUtils.MyUserInfo.FirstOrDefault(a => a.Id == UserDetails.UserId);
                //if (dataUser.PhoneVerified == 1)
                //{
                //    Toast.MakeText(this, GetText(Resource.String.Lbl_AccountVerified), ToastLength.Short).Show();
                //}
                //FIN


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
                Toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (Toolbar != null)
                {
                    Toolbar.Title = GetString(Resource.String.Lbl_MyAccount);
                    Toolbar.SetTitleTextColor(Color.Black);
                    SetSupportActionBar(Toolbar);
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
                    BtnSave.Click += BtnSaveOnClick;
                    EdtCountry.Click += EdtCountryOnClick;
                }
                else
                {
                    BtnSave.Click -= BtnSaveOnClick;
                    EdtCountry.Click += EdtCountryOnClick;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

       
        #endregion

        #region Events

        //Click save data and sent api
        private async void BtnSaveOnClick(object sender, EventArgs e)
        {
            try
            {
                //ROEO verifico si ya esta verificado numero
                var dataUser = ListUtils.MyUserInfo.FirstOrDefault(a => a.Id == UserDetails.UserId);
                if (dataUser.PhoneVerified == 0) //ROEO SI LA CUENTA NO  ESTA VERIFICADA  PERMITE EDITAR ESTA SECCION
                {
            /////////////
                var correctly = IMethods.Fun_String.IsPhoneNumber(EdtPhoneNumber.Text);
                 
                    if (IMethods.CheckConnectivity())
                {
                    //Show a progress
                    AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading));

                    var dictionary = new Dictionary<string, string>
                    {
                        {"username", EdtUsername.Text},
                        {"email", EdtEmail.Text},
                        {"country",Country},
                    };

                    if (correctly)
                        dictionary.Add("phone_number", EdtPhoneNumber.Text);

                    (int apiStatus, var respond) = await RequestsAsync.Users.UpdateProfileAsync(dictionary);
                    if (apiStatus == 200)
                    {
                        if (respond is UpdateProfileObject result)
                        {
                            var local = ListUtils.MyUserInfo.FirstOrDefault(a => a.Id == UserDetails.UserId);
                            if (local != null)
                            {
                                local.Username = EdtUsername.Text;
                                local.Email = EdtEmail.Text;
                                local.PhoneNumber = EdtPhoneNumber.Text;
                                local.Country = Country;

                                SqLiteDatabase database = new SqLiteDatabase();
                                database.InsertOrUpdate_DataMyInfo(local);
                                database.Dispose(); 
                            }
                          // ROEO
                          //  Toast.MakeText(this, GetText(Resource.String.Lbl_Done), ToastLength.Short).Show();
                            Toast.MakeText(this, GetText(Resource.String.Lbl_Verify_Success), ToastLength.Short).Show();
                          // FIN
                                AndHUD.Shared.Dismiss(this);
                             
                            Intent resultIntent = new Intent();
                            SetResult(Result.Ok, resultIntent);
                            Finish();
                        }
                    }
                    else if (apiStatus == 400)
                    {
                        if (respond is ErrorObject error)
                        {
                            var errorText = error.ErrorData.ErrorText;
                            AndHUD.Shared.ShowError(this, errorText, MaskType.Clear, TimeSpan.FromSeconds(2));

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
                        Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                    }

                   if (!correctly)
                       Toast.MakeText(this, GetText(Resource.String.Lbl_PleaseAddPhoneNumberCorrectly), ToastLength.Short).Show();
                }// ROEO FIN 
                else
                { Toast.MakeText(this, GetText(Resource.String.Lbl_AccountVerified), ToastLength.Short).Show(); }
                //fin
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                AndHUD.Shared.Dismiss(this);
            }
        }

        //Country
        private void EdtCountryOnClick(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    string[] countriesArray = Resources.GetStringArray(Resource.Array.countriesArray);

                    var arrayAdapter = new List<string>();
                    var dialogList = new MaterialDialog.Builder(this);

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
                    EdtUsername.Text = dataUser.Username;
                    EdtEmail.Text = dataUser.Email;
                    EdtPhoneNumber.Text = dataUser.PhoneNumber;
                    EdtCountry.Text = QuickDateTools.GetCountry(dataUser.Country);

                    
                    //if (dataUser.PhoneVerified == 1) //ROEO SI LA CUENTA NO  ESTA VERIFICADA  PERMITE EDITAR ESTA SECCION
                    //{
                    //    Toast.MakeText(this, GetText(Resource.String.Lbl_AccountVerified), ToastLength.Short).Show();
                    //}
                    ////FIN
                    if (dataUser.PhoneVerified == 0)
                    {
                        var window = new PopupController(this);
                        window.DisplayAddPhoneNumber();

                        if (GlobalContext.TracksCounter != null)
                            GlobalContext.TracksCounter.LastCounterEnum = TracksCounter.TracksCounterEnum.AddPhoneNumber;
                    } 
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

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

                string[] countriesArray = Resources.GetStringArray(Resource.Array.countriesArray_id);

                var data = countriesArray[id];
                Country = data;
                EdtCountry.Text = text;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

    }
}