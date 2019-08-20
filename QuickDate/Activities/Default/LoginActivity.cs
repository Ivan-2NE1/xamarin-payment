using System;
using System.Collections.Generic;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Auth.Api;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Org.Json;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.SocialLogins;
using QuickDate.Helpers.Utils;
using QuickDate.SQLite;
using QuickDateClient;
using QuickDateClient.Classes.Authorization;
using QuickDateClient.Classes.Global;
using QuickDateClient.Requests;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;
using Xamarin.Facebook.Login.Widget;
using Object = Java.Lang.Object;
using Profile = Xamarin.Facebook.Profile;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace QuickDate.Activities.Default
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/ProfileTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, LaunchMode = LaunchMode.SingleInstance)]
    public class LoginActivity : AppCompatActivity, IFacebookCallback, GraphRequest.IGraphJSONObjectCallback,GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener, IResultCallback
    {
        #region Variables Basic

        public EditText EmailEditText, PasswordEditText;
        public ProgressBar ProgressBar;
        public Button BtnSignIn;
        public TextView ForgotPassTextView, TermsTextView;
        public LinearLayout LinearRegister;

        public LoginButton FbLoginButton;
        public SignInButton GoogleSignInButton;

        public ICallbackManager MFbCallManager;
        public FB_MyProfileTracker MprofileTracker;
        public static GoogleApiClient MGoogleApiClient;
        public AppSettings St;
        public Toolbar Toolbar;
        #endregion

        #region General
 
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                //Set Full screen 
                View mContentView = Window.DecorView;
                var uiOptions = (int)mContentView.SystemUiVisibility;
                var newUiOptions = uiOptions;

                newUiOptions |= (int)SystemUiFlags.Fullscreen;
                newUiOptions |= (int)SystemUiFlags.HideNavigation;
                mContentView.SystemUiVisibility = (StatusBarVisibility)newUiOptions;

                Window.AddFlags(WindowManagerFlags.Fullscreen);

                // Create your application here
                SetContentView(Resource.Layout.LoginLayout);

                St = new AppSettings();
                var client = new Client(St.TripleDesAppServiceProvider);

                //Get Value And Set Toolbar
                InitComponent();
                InitToolbar();
                InitSocialLogins();
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

        protected override void OnStop()
        {
            try
            {
                base.OnStop();
                if (MGoogleApiClient.IsConnected) MGoogleApiClient.Disconnect();
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
                EmailEditText = FindViewById<EditText>(Resource.Id.edt_email);
                PasswordEditText = FindViewById<EditText>(Resource.Id.edt_password);
                ProgressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);
                BtnSignIn = FindViewById<Button>(Resource.Id.SignInButton);
                ForgotPassTextView = FindViewById<TextView>(Resource.Id.txt_forgot_pass);
                TermsTextView = FindViewById<TextView>(Resource.Id.terms);
                LinearRegister = FindViewById<LinearLayout>(Resource.Id.tvRegister);

                ProgressBar.Visibility = ViewStates.Invisible;
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
                    //Toolbar.Title = GetString(Resource.String.Lbl_SignIn);
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

        public void InitSocialLogins()
        {
            try
            {
                //#Facebook
                if (AppSettings.ShowFacebookLogin)
                {
                    FacebookSdk.SdkInitialize(this);

                    MprofileTracker = new FB_MyProfileTracker();
                    MprofileTracker.mOnProfileChanged += MProfileTrackerOnMOnProfileChanged;
                    MprofileTracker.StartTracking();

                    FbLoginButton = FindViewById<LoginButton>(Resource.Id.fblogin_button);
                    FbLoginButton.Visibility = ViewStates.Visible;
                    FbLoginButton.SetReadPermissions(new List<string>
                    {
                        "email",
                        "public_profile"
                    });

                    MFbCallManager = CallbackManagerFactory.Create();
                    FbLoginButton.RegisterCallback(MFbCallManager, this);

                    //FB accessToken
                    var accessToken = AccessToken.CurrentAccessToken;
                    var isLoggedIn = accessToken != null && !accessToken.IsExpired;
                    if (isLoggedIn && Profile.CurrentProfile != null)
                    {
                        LoginManager.Instance.LogOut();
                    }

                   string hash = IMethods.IApp.GetKeyHashesConfigured(this);
                }
                else
                {
                    FbLoginButton = FindViewById<LoginButton>(Resource.Id.fblogin_button);
                    FbLoginButton.Visibility = ViewStates.Gone;
                }
                 
                //#Google
                if (AppSettings.ShowGoogleLogin)
                {
                    // Configure sign-in to request the user's ID, email address, and basic profile. ID and basic profile are included in DEFAULT_SIGN_IN.
                    var gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                        .RequestIdToken(GoogleServices.ClientId)
                        .RequestScopes(new Scope(Scopes.Profile))
                        .RequestScopes(new Scope(Scopes.PlusLogin))
                        .RequestScopes(new Scope(Scopes.PlusMe))
                        .RequestServerAuthCode(GoogleServices.ClientId)
                        .RequestProfile().RequestEmail().Build();

                    // Build a GoogleApiClient with access to the Google Sign-In API and the options specified by gso.
                    MGoogleApiClient = new GoogleApiClient.Builder(this, this, this)
                        .EnableAutoManage(this, this)
                        .AddApi(Auth.GOOGLE_SIGN_IN_API, gso)
                        .Build();

                    GoogleSignInButton = FindViewById<SignInButton>(Resource.Id.Googlelogin_button);
                    GoogleSignInButton.Click += GoogleSignInButtonOnClick;
                }
                else
                {
                    GoogleSignInButton = FindViewById<SignInButton>(Resource.Id.Googlelogin_button);
                    GoogleSignInButton.Visibility = ViewStates.Gone;
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
                    BtnSignIn.Click += BtnSignInOnClick;
                    ForgotPassTextView.Click += ForgotPassTextViewOnClick;
                    TermsTextView.Click += TermsTextViewOnClick;
                    LinearRegister.Click += LinearRegisterOnClick;
                }
                else
                {
                    BtnSignIn.Click -= BtnSignInOnClick;
                    ForgotPassTextView.Click -= ForgotPassTextViewOnClick;
                    TermsTextView.Click -= TermsTextViewOnClick;
                    LinearRegister.Click -= LinearRegisterOnClick;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Events

        //Login QuickDate
        private async void BtnSignInOnClick(object sender, EventArgs e)
        {
            try
            {
                if (IMethods.CheckConnectivity())
                {
                    if (!string.IsNullOrEmpty(EmailEditText.Text) || !string.IsNullOrEmpty(PasswordEditText.Text))
                    {
                        ProgressBar.Visibility = ViewStates.Visible;
                        BtnSignIn.Visibility = ViewStates.Gone;
                        ForgotPassTextView.Visibility = ViewStates.Gone;

                        (int apiStatus, var respond) = await RequestsAsync.Auth.LoginAsync(EmailEditText.Text, PasswordEditText.Text,UserDetails.DeviceId);
                        if (apiStatus == 200)
                        {
                            if (respond is LoginObject auth)
                            {
                                SetDataLogin(auth);

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
                                    case 1:
                                        IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_1), GetText(Resource.String.Lbl_Ok));
                                        break;
                                    case 2:
                                        IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_2), GetText(Resource.String.Lbl_Ok));
                                        break;
                                    case 3:
                                        IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_3), GetText(Resource.String.Lbl_Ok));
                                        break;
                                    case 4:
                                        IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_4), GetText(Resource.String.Lbl_Ok));
                                        break;
                                    case 5:
                                        IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_5), GetText(Resource.String.Lbl_Ok));
                                        break;
                                    default:
                                        IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), errorText, GetText(Resource.String.Lbl_Ok));
                                        break;
                                } 
                            }

                            ProgressBar.Visibility = ViewStates.Gone;
                            BtnSignIn.Visibility = ViewStates.Visible;
                            ForgotPassTextView.Visibility = ViewStates.Visible;
                        }
                        else if (apiStatus == 404)
                        {
                            ProgressBar.Visibility = ViewStates.Gone;
                            BtnSignIn.Visibility = ViewStates.Visible;
                            ForgotPassTextView.Visibility = ViewStates.Visible;
                            IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetText(Resource.String.Lbl_Error_Login), GetText(Resource.String.Lbl_Ok));
                        }
                    }
                    else
                    {
                        ProgressBar.Visibility = ViewStates.Gone;
                        BtnSignIn.Visibility = ViewStates.Visible;
                        ForgotPassTextView.Visibility = ViewStates.Visible;
                        IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetText(Resource.String.Lbl_Please_enter_your_data), GetText(Resource.String.Lbl_Ok));
                    }
                }
                else
                {
                    ProgressBar.Visibility = ViewStates.Gone;
                    BtnSignIn.Visibility = ViewStates.Visible;
                    ForgotPassTextView.Visibility = ViewStates.Visible;
                    IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetText(Resource.String.Lbl_CheckYourInternetConnection), GetText(Resource.String.Lbl_Ok));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                ProgressBar.Visibility = ViewStates.Gone;
                BtnSignIn.Visibility = ViewStates.Visible;
                ForgotPassTextView.Visibility = ViewStates.Visible;
                IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), ex.Message, GetText(Resource.String.Lbl_Ok));

            }
        }

        //Open Register
        private void LinearRegisterOnClick(object sender, EventArgs e)
        {
            try
            {
                StartActivity(new Intent(this, typeof(RegisterActivity)));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Open Terms of use 
        private void TermsTextViewOnClick(object sender, EventArgs e)
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

        //Open Forgot Password
        private void ForgotPassTextViewOnClick(object sender, EventArgs e)
        {
            try
            {
                StartActivity(new Intent(this, typeof(ForgotPasswordActivity)));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Login With Google
        private void GoogleSignInButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                MGoogleApiClient.Connect();

                var opr = Auth.GoogleSignInApi.SilentSignIn(MGoogleApiClient);
                if (opr.IsDone)
                {
                    // If the user's cached credentials are valid, the OptionalPendingResult will be "done"
                    // and the GoogleSignInResult will be available instantly.
                    Log.Debug("Login_Activity", "Got cached sign-in");
                    var result = opr.Get() as GoogleSignInResult;
                    HandleSignInResult(result);

                    //Auth.GoogleSignInApi.SignOut(mGoogleApiClient).SetResultCallback(this);
                }
                else
                {
                    // If the user has not previously signed in on this device or the sign-in has expired,
                    // this asynchronous branch will attempt to sign in the user silently.  Cross-device
                    // single sign-on will occur in this branch.
                    opr.SetResultCallback(new SignInResultCallback { Activity = this });
                }

                // Check if we're running on Android 5.0 or higher
                if ((int)Build.VERSION.SdkInt < 23)
                {
                    if (!MGoogleApiClient.IsConnecting)
                        ResolveSignInError();
                    else if (MGoogleApiClient.IsConnected) MGoogleApiClient.Disconnect();
                }
                else
                {
                    if (CheckSelfPermission(Manifest.Permission.GetAccounts) == Permission.Granted &&
                        CheckSelfPermission(Manifest.Permission.UseCredentials) == Permission.Granted)
                    {
                        if (!MGoogleApiClient.IsConnecting)
                            ResolveSignInError();
                        else if (MGoogleApiClient.IsConnected) MGoogleApiClient.Disconnect();
                    }
                    else
                    {
                        new PermissionsController(this).RequestPermission(106);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        //Login With Facebook
        private void MProfileTrackerOnMOnProfileChanged(object sender, OnProfileChangedEventArgs e)
        {
            try
            {
                if (e.mProfile != null)
                {
                    FbFirstName = e.mProfile.FirstName;
                    FbLastName = e.mProfile.LastName;
                    FbName = e.mProfile.Name;
                    FbProfileId = e.mProfile.Id;

                    var request = GraphRequest.NewMeRequest(AccessToken.CurrentAccessToken, this);
                    var parameters = new Bundle();
                    parameters.PutString("fields", "id,name,age_range,email");
                    request.Parameters = parameters;
                    request.ExecuteAsync();
                } 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void SetDataLogin(LoginObject auth)
        {
            try
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
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        #endregion 

        #region Social Logins

        public string FbFirstName, FbLastName, FbName, FbEmail, FbAccessToken, FbProfileId;
        public string GFirstName, GLastName, GProfileId;
        public string GAccountName, GAccountType, GDisplayName, GEmail, GImg, GIdtoken, GAccessToken, GServerCode;

        #region Facebook

        public void OnCancel()
        {
            try
            {
                ProgressBar.Visibility = ViewStates.Gone;
                BtnSignIn.Visibility = ViewStates.Visible;
                ForgotPassTextView.Visibility = ViewStates.Visible;

                SetResult(Result.Canceled);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnError(FacebookException error)
        {
            try
            {

                ProgressBar.Visibility = ViewStates.Gone;
                BtnSignIn.Visibility = ViewStates.Visible;
                ForgotPassTextView.Visibility = ViewStates.Visible;

                // Handle e
                IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), error.Message, GetText(Resource.String.Lbl_Ok));

                SetResult(Result.Canceled);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        public void OnSuccess(Object result)
        {
            try
            {
                var loginResult = result as LoginResult;
                var id = AccessToken.CurrentAccessToken.UserId;

                ProgressBar.Visibility = ViewStates.Visible;
                BtnSignIn.Visibility = ViewStates.Gone;
                ForgotPassTextView.Visibility = ViewStates.Gone;

                SetResult(Result.Ok);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async void OnCompleted(JSONObject json, GraphResponse response)
        {
            try
            {
                var data = json.ToString();
                var result = JsonConvert.DeserializeObject<FacebookResult>(data);
                FbEmail = result.email;

                ProgressBar.Visibility = ViewStates.Visible;
                BtnSignIn.Visibility = ViewStates.Gone;
                ForgotPassTextView.Visibility = ViewStates.Gone;

                var accessToken = AccessToken.CurrentAccessToken;
                if (accessToken != null)
                {
                    FbAccessToken = accessToken.Token;

                    //Login Api 
                    (int apiStatus, var respond) = await RequestsAsync.Auth.SocialLoginAsync(FbAccessToken, "facebook",UserDetails.DeviceId);
                    if (apiStatus == 200)
                    {
                        if (respond is LoginObject auth)
                        {
                            SetDataLogin(auth);

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
                                case 1:
                                    IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_1), GetText(Resource.String.Lbl_Ok));
                                    break;
                                case 2:
                                    IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_2), GetText(Resource.String.Lbl_Ok));
                                    break;
                                case 3:
                                    IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_3), GetText(Resource.String.Lbl_Ok));
                                    break;
                                case 4:
                                    IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_4), GetText(Resource.String.Lbl_Ok));
                                    break;
                                case 5:
                                    IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_5), GetText(Resource.String.Lbl_Ok));
                                    break;
                                default:
                                    IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), errorText, GetText(Resource.String.Lbl_Ok));
                                    break;
                            }
                        }

                        ProgressBar.Visibility = ViewStates.Gone;
                        BtnSignIn.Visibility = ViewStates.Visible;
                        ForgotPassTextView.Visibility = ViewStates.Visible;
                    }
                    else if (apiStatus == 404)
                    {
                        ProgressBar.Visibility = ViewStates.Gone;
                        BtnSignIn.Visibility = ViewStates.Visible;
                        ForgotPassTextView.Visibility = ViewStates.Visible;
                        IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetText(Resource.String.Lbl_Error_Login), GetText(Resource.String.Lbl_Ok));
                    }
                }
            }
            catch (Exception e)
            {
                ProgressBar.Visibility = ViewStates.Gone;
                BtnSignIn.Visibility = ViewStates.Visible;
                ForgotPassTextView.Visibility = ViewStates.Visible;
                IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), e.Message, GetText(Resource.String.Lbl_Ok));
                Console.WriteLine(e);
            }
        }
         
        #endregion

        //======================================================

        #region Google
             
        public void HandleSignInResult(GoogleSignInResult result)
        {
            try
            {
                Log.Debug("Login_Activity", "handleSignInResult:" + result.IsSuccess);
                if (result.IsSuccess)
                {
                    // Signed in successfully, show authenticated UI.
                    var acct = result.SignInAccount;
                    SetContentGoogle(acct);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void ResolveSignInError()
        {
            try
            {
                if (MGoogleApiClient.IsConnecting) return;

                var signInIntent = Auth.GoogleSignInApi.GetSignInIntent(MGoogleApiClient);
                StartActivityForResult(signInIntent, 0);
            } 
            catch (Exception e)
            {
                //The intent was cancelled before it was sent. Return to the default
                //state and attempt to connect to get an updated ConnectionResult
                Console.WriteLine(e);
                MGoogleApiClient.Connect();
                
            }
        }

        public void OnConnected(Bundle connectionHint)
        {
            try
            {
                var opr = Auth.GoogleSignInApi.SilentSignIn(MGoogleApiClient);
                if (opr.IsDone)
                {
                    // If the user's cached credentials are valid, the OptionalPendingResult will be "done"
                    // and the GoogleSignInResult will be available instantly.
                    Log.Debug("Login_Activity", "Got cached sign-in");
                    var result = opr.Get() as GoogleSignInResult;
                    HandleSignInResult(result);
                }
                else
                {
                    // If the user has not previously signed in on this device or the sign-in has expired,
                    // this asynchronous branch will attempt to sign in the user silently.  Cross-device
                    // single sign-on will occur in this branch.

                    opr.SetResultCallback(new SignInResultCallback { Activity = this });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async void SetContentGoogle(GoogleSignInAccount acct)
        {
            try
            {
                //Successful log in hooray!!
                if (acct != null)
                {
                    ProgressBar.Visibility = ViewStates.Visible;
                    BtnSignIn.Visibility = ViewStates.Gone;
                    ForgotPassTextView.Visibility = ViewStates.Gone;

                    GAccountName = acct.Account.Name;
                    GAccountType = acct.Account.Type;
                    GDisplayName = acct.DisplayName;
                    GFirstName = acct.GivenName;
                    GLastName = acct.FamilyName;
                    GProfileId = acct.Id;
                    GEmail = acct.Email;
                    GImg = acct.PhotoUrl.Path;
                    GIdtoken = acct.IdToken;
                    GServerCode = acct.ServerAuthCode;

                    var api = new GoogleAPI();
                    GAccessToken = await api.GetAccessTokenAsync(GServerCode);

                    if (!string.IsNullOrEmpty(GAccessToken))
                    {
                        //Login Api 
                        string key = IMethods.IApp.GetValueFromManifest(this, "com.google.android.geo.API_KEY");
                        (int apiStatus, var respond) = await RequestsAsync.Auth.SocialLoginAsync(FbAccessToken, "google", UserDetails.DeviceId);
                        if (apiStatus == 200)
                        {
                            if (respond is LoginObject auth)
                            {
                                SetDataLogin(auth);

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
                                    case 1:
                                        IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_1), GetText(Resource.String.Lbl_Ok));
                                        break;
                                    case 2:
                                        IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_2), GetText(Resource.String.Lbl_Ok));
                                        break;
                                    case 3:
                                        IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_3), GetText(Resource.String.Lbl_Ok));
                                        break;
                                    case 4:
                                        IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_4), GetText(Resource.String.Lbl_Ok));
                                        break;
                                    case 5:
                                        IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_5), GetText(Resource.String.Lbl_Ok));
                                        break;
                                    default:
                                        IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), errorText, GetText(Resource.String.Lbl_Ok));
                                        break;
                                }
                            }

                            ProgressBar.Visibility = ViewStates.Gone;
                            BtnSignIn.Visibility = ViewStates.Visible;
                            ForgotPassTextView.Visibility = ViewStates.Visible;
                        }
                        else if (apiStatus == 404)
                        {
                            ProgressBar.Visibility = ViewStates.Gone;
                            BtnSignIn.Visibility = ViewStates.Visible;
                            ForgotPassTextView.Visibility = ViewStates.Visible;
                            IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetText(Resource.String.Lbl_Error_Login), GetText(Resource.String.Lbl_Ok));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ProgressBar.Visibility = ViewStates.Gone;
                BtnSignIn.Visibility = ViewStates.Visible;
                ForgotPassTextView.Visibility = ViewStates.Visible;
                IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), e.Message, GetText(Resource.String.Lbl_Ok));
                Console.WriteLine(e);
            }
        }

        public void OnConnectionSuspended(int cause)
        {
            try
            {
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            try
            {
                // An unresolvable error has occurred and Google APIs (including Sign-In) will not
                // be available.
                Log.Debug("Login_Activity", "onConnectionFailed:" + result);

                //The user has already clicked 'sign-in' so we attempt to resolve all
                //errors until the user is signed in, or the cancel
                ResolveSignInError();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnResult(Object result)
        {
            try
            {

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        //======================================================

        #endregion

        #region Permissions && Result

        //Result
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data); 
                if (requestCode == 0)
                {
                    var result = Auth.GoogleSignInApi.GetSignInResultFromIntent(data);
                    HandleSignInResult(result);
                }
                else
                {
                    // Logins Facebook
                    MFbCallManager.OnActivityResult(requestCode, (int)resultCode, data);
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

                if (requestCode == 106)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        if (!MGoogleApiClient.IsConnecting)
                            ResolveSignInError();
                        else if (MGoogleApiClient.IsConnected) MGoogleApiClient.Disconnect();
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long).Show();
                    }
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