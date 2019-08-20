using System;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common.Apis;

using QuickDate.Activities.Default;

using Object = Java.Lang.Object;

namespace QuickDate.Helpers.SocialLogins
{
    public class SignInResultCallback : Object, IResultCallback
    {
        public LoginActivity Activity { get; set; }

        public void OnResult(Object result)
        {
            try
            {
                var googleSignInResult = result as GoogleSignInResult;
                Activity.HandleSignInResult(googleSignInResult);
            }
            catch (Exception e)
            {
               Console.WriteLine(e);
            }
        }
    }


    public class SignOutResultCallback : Object, IResultCallback
    {
        public LoginActivity Activity { get; set; }

        public void OnResult(Object result)
        {
            //Activity.UpdateUI(false);
        }
    }

}