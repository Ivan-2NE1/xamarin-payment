using Android;
using Android.App;
using Android.OS;
using System;

namespace QuickDate.Helpers.Controller
{
    public class PermissionsController
    {
        private Activity context;

        public PermissionsController(Activity activity)
        {
            try
            {
                context = activity;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Handle Permission Request
        /// </summary>
        /// <param name="idPermissions"> 100 >> Storage  101 >> ReadContacts && ReadPhoneNumbers  102 >> RecordAudio  103 >> Camera  104 >> SendSms  105 >> Location  106 >> GetAccounts && UseCredentials >> Social Logins  107 >> AccessWifiState && Internet  108 >> Storage && Camera</param>
        public void RequestPermission(int idPermissions)
        {
            // Check if we're running on Android 5.0 or higher
            if ((int)Build.VERSION.SdkInt >= 23)
            {
                switch (idPermissions)
                {
                    case 100:
                        context.RequestPermissions(new string[]
                        {
                            Manifest.Permission.ReadExternalStorage,
                            Manifest.Permission.WriteExternalStorage,
                        }, 100);
                        break;

                    case 101:
                        context.RequestPermissions(new string[]
                        {
                            Manifest.Permission.ReadContacts,
                            Manifest.Permission.ReadPhoneNumbers,
                        }, 101);
                        break;

                    case 102:
                        context.RequestPermissions(new string[]
                        {
                            Manifest.Permission.RecordAudio,
                            Manifest.Permission.ModifyAudioSettings,
                        }, 102);
                        break;

                    case 103:
                        context.RequestPermissions(new string[]
                        {
                            Manifest.Permission.Camera,
                        }, 103);
                        break;

                    case 104:
                        context.RequestPermissions(new string[]
                        {
                            Manifest.Permission.SendSms,
                            Manifest.Permission.BroadcastSms,
                        }, 104);
                        break;

                    case 105:
                        context.RequestPermissions(new string[]
                        {
                            Manifest.Permission.AccessFineLocation,
                            Manifest.Permission.AccessCoarseLocation
                        }, 105);
                        break;

                    case 106:
                        context.RequestPermissions(new[]
                        {
                            Manifest.Permission.GetAccounts,
                            Manifest.Permission.UseCredentials
                        }, 106);
                        break;

                    case 107:
                        context.RequestPermissions(new[]
                        {
                            Manifest.Permission.AccessWifiState,
                            Manifest.Permission.Internet,
                        }, 107);
                        break;
                    case 108:
                        context.RequestPermissions(new[]
                        {
                            Manifest.Permission.Camera,
                            Manifest.Permission.ReadExternalStorage,
                            Manifest.Permission.WriteExternalStorage,
                        }, 108);
                        break;
                    case 109:
                        context.RequestPermissions(new[]
                        {
                            Manifest.Permission.ReadProfile,
                            Manifest.Permission.ReadPhoneNumbers,
                            Manifest.Permission.ReadPhoneState,
                        }, 109);
                        break;
                    case 110:
                        context.RequestPermissions(new[]
                        {
                            Manifest.Permission.WakeLock,
                        }, 110);
                        break;
                }
            }
            else
            {
                return;
            }
        }
    }
}