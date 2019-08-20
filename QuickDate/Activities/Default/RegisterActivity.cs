using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDate.SQLite;
using QuickDateClient;
using QuickDateClient.Classes.Authorization;
using QuickDateClient.Classes.Global;
using QuickDateClient.Requests;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace QuickDate.Activities.Default
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/ProfileTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, LaunchMode = LaunchMode.SingleInstance)]
    public class RegisterActivity : AppCompatActivity
    {
        #region Variables Basic

        public Toolbar Toolbar;
        public EditText FirstNameEditText, LastNameEditText, EmailEditText, UsernameEditText, PasswordEditText, ConfirmPasswordEditText;
        public Button RegisterButton;
        public LinearLayout TermsLayout, SignLayout;
        public ProgressBar ProgressBar;
        public AppSettings St;

        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                View mContentView = Window.DecorView;
                var uiOptions = (int)mContentView.SystemUiVisibility;
                var newUiOptions = uiOptions;

                newUiOptions |= (int)SystemUiFlags.Fullscreen;
                newUiOptions |= (int)SystemUiFlags.HideNavigation;
                mContentView.SystemUiVisibility = (StatusBarVisibility)newUiOptions;

                Window.AddFlags(WindowManagerFlags.Fullscreen);

                base.OnCreate(savedInstanceState);

                // Create your application here
                SetContentView(Resource.Layout.RegisterLayout);

                //Get Value And Set Toolbar
                InitComponent();
                InitToolbar();

                var client = new Client(new AppSettings().TripleDesAppServiceProvider);
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

        #region Functions

        public void InitComponent()
        {
            try
            {
                FirstNameEditText = FindViewById<EditText>(Resource.Id.edt_firstname);
                LastNameEditText = FindViewById<EditText>(Resource.Id.edt_lastname);
                EmailEditText = FindViewById<EditText>(Resource.Id.edt_email);
                UsernameEditText = FindViewById<EditText>(Resource.Id.edt_username);
                PasswordEditText = FindViewById<EditText>(Resource.Id.edt_password);
                ConfirmPasswordEditText = FindViewById<EditText>(Resource.Id.edt_Confirmpassword);
                ProgressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);
                RegisterButton = FindViewById<Button>(Resource.Id.SignInButton);
                TermsLayout = FindViewById<LinearLayout>(Resource.Id.termsLayout);
                SignLayout = FindViewById<LinearLayout>(Resource.Id.SignLayout); 
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
                    //Toolbar.Title = GetString(Resource.String.Lbl_Register);
                    Toolbar.Title = " ";
                    Toolbar.SetTitleTextColor(Color.White);
                    SetSupportActionBar(Toolbar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(false);
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
                    RegisterButton.Click += RegisterButtonOnClick;
                    TermsLayout.Click += TermsLayoutOnClick;
                    SignLayout.Click += SignLayoutOnClick;
                }
                else
                {
                    RegisterButton.Click -= RegisterButtonOnClick;
                    TermsLayout.Click -= TermsLayoutOnClick;
                    SignLayout.Click -= SignLayoutOnClick;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Events

        //Open Login Activity
        private void SignLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                StartActivity(new Intent(this, typeof(LoginActivity)));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Open Terms of Service
        private void TermsLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                var url = Client.WebsiteUrl + "/terms-of-use";
                IMethods.IApp.OpenbrowserUrl(this, url);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        //Register QuickDate
        private async void RegisterButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                if (IMethods.CheckConnectivity())
                { 
                    if (!string.IsNullOrEmpty(EmailEditText.Text) || !string.IsNullOrEmpty(UsernameEditText.Text) ||
                        !string.IsNullOrEmpty(PasswordEditText.Text) || !string.IsNullOrEmpty(ConfirmPasswordEditText.Text))
                    { 
                        var check = IMethods.Fun_String.IsEmailValid(EmailEditText.Text);
                        if (!check)
                        {
                            IMethods.DialogPopup.InvokeAndShowDialog(this,GetText(Resource.String.Lbl_VerificationFailed),GetText(Resource.String.Lbl_IsEmailValid), GetText(Resource.String.Lbl_Ok));
                        }
                        else
                        {
                            if (PasswordEditText.Text != ConfirmPasswordEditText.Text)
                            {
                                ProgressBar.Visibility = ViewStates.Gone;
                                RegisterButton.Visibility = ViewStates.Visible;
                                IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security),GetText(Resource.String.Lbl_Error_Register_password),GetText(Resource.String.Lbl_Ok));
                            }
                            else
                            {
                                ProgressBar.Visibility = ViewStates.Visible;
                                RegisterButton.Visibility = ViewStates.Gone;

                                (int apiStatus, var respond) = await RequestsAsync.Auth.RegisterAsync(FirstNameEditText.Text , LastNameEditText.Text, UsernameEditText.Text, EmailEditText.Text, PasswordEditText.Text, UserDetails.DeviceId);
                                if (apiStatus == 200)
                                {
                                    if (respond is RegisterObject auth)
                                    {
                                        SetDataRegister(auth);
                                        //ROEO INGRESA A LA APLICACION SI NO REQUIERE AUTENTICACION ENTRA DIRECTAMENTE AL MENU INTERNO PARA CLIENTES
                                        //DEAFAUT => BoardingActivity
                                        StartActivity(new Intent(this, typeof(BoardingActivity)));
                                        Finish(); 
                                    }
                                }
                                else if (apiStatus == 400)
                                {
                                    if (respond is ErrorObject error)
                                    {
                                        string errorText = error.Message;
                                        int errorId = error.Code;
                                        switch (errorId)
                                        {
                                            case 6:
                                                IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_6), GetText(Resource.String.Lbl_Ok));
                                                break;
                                            case 7:
                                                IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_7), GetText(Resource.String.Lbl_Ok));
                                                break;
                                            case 8:
                                                IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_8), GetText(Resource.String.Lbl_Ok));
                                                break;
                                            case 9:
                                                IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_9), GetText(Resource.String.Lbl_Ok));
                                                break;
                                            case 10:
                                                IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_10), GetText(Resource.String.Lbl_Ok));
                                                break;
                                            case 11:
                                                IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_11), GetText(Resource.String.Lbl_Ok));
                                                break;
                                            case 12:
                                                IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_12), GetText(Resource.String.Lbl_Ok));
                                                break;
                                            case 13:
                                                IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_13), GetText(Resource.String.Lbl_Ok));
                                                break;
                                            case 14:
                                                IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_14), GetText(Resource.String.Lbl_Ok));
                                                break;
                                            case 15:
                                                IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_15), GetText(Resource.String.Lbl_Ok));
                                                break;
                                            case 16:
                                                IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_16), GetText(Resource.String.Lbl_Ok));
                                                break;
                                           //ROEO ERROR DESPUES DE REGITRARSE APP APARECE ESTE MENSAJE
                                            case 17:
                                                IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_17), GetText(Resource.String.Lbl_Ok));
                                               break;

                                            case 18:
                                                IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_18), GetText(Resource.String.Lbl_Ok));
                                                break;
                                            default:
                                                IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), errorText, GetText(Resource.String.Lbl_Ok));
                                                break;
                                        }
                                    }

                                    ProgressBar.Visibility = ViewStates.Gone;
                                    RegisterButton.Visibility = ViewStates.Visible; 
                                }
                                else if (apiStatus == 404)
                                {
                                    ProgressBar.Visibility = ViewStates.Gone;
                                    RegisterButton.Visibility = ViewStates.Visible;
                                    IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetText(Resource.String.Lbl_Error_Login), GetText(Resource.String.Lbl_Ok));
                                }
                            }
                        }
                    }
                    else
                    {
                        ProgressBar.Visibility = ViewStates.Gone;
                        RegisterButton.Visibility = ViewStates.Visible;
                        IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetText(Resource.String.Lbl_Please_enter_your_data), GetText(Resource.String.Lbl_Ok));
                    }
                }
                else
                {
                    ProgressBar.Visibility = ViewStates.Gone;
                    RegisterButton.Visibility = ViewStates.Visible;
                    IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetText(Resource.String.Lbl_CheckYourInternetConnection), GetText(Resource.String.Lbl_Ok));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                ProgressBar.Visibility = ViewStates.Gone;
                RegisterButton.Visibility = ViewStates.Visible;
                IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), ex.Message, GetText(Resource.String.Lbl_Ok));

            }
        }
         
        public void SetDataRegister(RegisterObject auth)
        {
            try
            {
                if (auth.data != null)
                {
                    UserDetails.Username = EmailEditText.Text;
                    UserDetails.FullName = EmailEditText.Text;
                    UserDetails.Password = PasswordEditText.Text;
                    UserDetails.AccessToken = auth.data.AccessToken;
                    UserDetails.UserId = auth.data.UserId;
                    UserDetails.Status = "Pending";
                    UserDetails.Cookie = auth.data.AccessToken;
                    UserDetails.Email = EmailEditText.Text;

                    Current.AccessToken = auth.data.AccessToken;

                    //Insert user data to database
                    var user = new DataTables.LoginTb
                    {
                        UserId = UserDetails.UserId.ToString(),
                        AccessToken = UserDetails.AccessToken,
                        Cookie = UserDetails.Cookie,
                        Username = EmailEditText.Text,
                        Password = PasswordEditText.Text,
                        Status = "Pending",
                        Lang = "",
                        DeviceId = UserDetails.DeviceId,
                    };



                    ListUtils.DataUserLoginList.Add(user);

                    var dbDatabase = new SqLiteDatabase();
                    dbDatabase.InsertRow(user);
                    if (auth.data.UserInfo != null)
                    {
                        dbDatabase.InsertOrUpdate_DataMyInfo(auth.data.UserInfo);
                        ApiRequest.GetInfoData(UserDetails.UserId.ToString()).ConfigureAwait(false);
                    }
                     
                    dbDatabase.Dispose();
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