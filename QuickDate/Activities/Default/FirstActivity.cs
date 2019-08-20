using System;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using QuickDate.Helpers.CacheLoaders;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Utils;

namespace QuickDate.Activities.Default
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/DarkHeader", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, LaunchMode = LaunchMode.SingleInstance)]
    public class FirstActivity : AppCompatActivity
    {
        #region Variables Basic

        public Button LoginButton;
        public Button RegisterButton;
        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                // Create your application here
                SetContentView(Resource.Layout.FirstLayout);
                //Get Value  
                InitComponent();

                ApiRequest.GetSettings_Api().ConfigureAwait(false);
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

                // Check if we're running on Android 5.0 or higher
                if ((int)Build.VERSION.SdkInt < 23)
                {
                    // Check Created My Folder Or Not 
                    IMethods.IPath.Chack_MyFolder();
                }
                else
                {
                    if (CheckSelfPermission(Manifest.Permission.ReadExternalStorage) != Permission.Granted && CheckSelfPermission(Manifest.Permission.WriteExternalStorage) != Permission.Granted)
                    {
                        new PermissionsController(this).RequestPermission(100);
                    }
                }
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
                AddOrRemoveEvent(false);
                base.OnPause();
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
                ImageView Image1 = FindViewById<ImageView>(Resource.Id.Iconimage);
                ImageView Image2 = FindViewById<ImageView>(Resource.Id.Iconimage2);

                LoginButton = FindViewById<Button>(Resource.Id.LoginButton);
                RegisterButton = FindViewById<Button>(Resource.Id.RegisterButton);

                GlideImageLoader.LoadImage(this,"FirstImageOne.jpg", Image1, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);
                GlideImageLoader.LoadImage(this,"FirstImageTwo.jpg", Image2, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);
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
                    RegisterButton.Click += RegisterButton_Click;
                    LoginButton.Click += LoginButton_Click;
                }
                else
                {
                    RegisterButton.Click -= RegisterButton_Click;
                    LoginButton.Click -= LoginButton_Click;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        #endregion

        #region Events

        //Event Click open Register Activity
        private void RegisterButton_Click(object sender, EventArgs e)
        {
            try
            {
                StartActivity(new Intent(this, typeof(RegisterActivity))); 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        //Event Click open Login Activity
        private void LoginButton_Click(object sender, EventArgs e)
        {
            try
            {
               StartActivity(new Intent(this, typeof(LoginActivity))); 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        #endregion

        #region Permissions 

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            try
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                if (requestCode == 100)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        // Check Created My Folder Or Not 
                        IMethods.IPath.Chack_MyFolder();
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long).Show();
                        FinishAffinity();
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