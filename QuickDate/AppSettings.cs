//###############################################################
// Author >> Elin Doughouz 
// Copyright (c) PixelPhoto 15/07/2018 All Right Reserved
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// Follow me on facebook >> https://www.facebook.com/Elindoughous
//=========================================================

using System.Collections.Generic;

namespace QuickDate
{
    public class AppSettings
    {
        public AppSettings()
        {
            TripleDesAppServiceProvider = "IbxeixrTQTIbjj2d58d1xcdETtSfdB0y90i4+6g3geQWbGp4GglRVNa82TtRcP3w+sPXd+NKgAyihsd/rQ2JKRSdHHuRk6a8ieYyDvQGwrV9C9X+Pvb7mAz+fqSxTRKfSkzgriXn5D7/fLEDvfU6p2J/mB1ezyAG4faXrgqrF7NR7eqEPkfJHsFf7q5+Ir2Jk6YKdNmOIhJQke4+8bzBYNgr04EBrj/8RlXoT74y6qI=";
        }
          
        //Main Settings >>>>>
        //*********************************************************
        public string TripleDesAppServiceProvider;

        public string Version = "1.1";
        public static string ApplicationName = "Datznat";

        ////ROEO TWILIO >>
        public static string account_sid = "AC484ef5cd5d6dca61fed01553cf660add";
        public static string auth_token = "cb230f1fae0ada6544db79f97cceffd1";
        public static string phone_number = "19259058765";


        //Main Colors >>
        //*********************************************************
        public static string MainColor = "#5573F5";
        public static string StartColor = MainColor;
        public static string EndColor = "#542CDE";

        //Language Settings >> For next update versions 
        //*********************************************************
        public static bool FlowDirectionRightToLeft = false;
        public static string Lang = ""; //Default language

        //Notification Settings >>
        //*********************************************************
        public static bool ShowNotification = true;
        public static int RefreshDataSeconds = 10000; // 10 Seconds

        //*********************************************************

        //Add Animation Image User
        //*********************************************************
        public static bool EnableAddAnimationImageUser = false;
         
        //Set Theme Full Screen App
        //*********************************************************
        public static bool EnableFullScreenApp = false;

        //Social Logins >>
        //If you want login with facebook or google you should change id key in the String.xml file or AndroidManifest.xml
        //Facebook >> ../values/Strings.xml .. line 18 - 19 
        //Google >> ../Properties/AndroidManifest.xml .. line 52
        //*********************************************************
        public static bool ShowFacebookLogin = false; //ROEO NO APARECERA BOTOS FACEBOOK
        public static bool ShowGoogleLogin = false; // Next Version

        //ADMOB >> Please add the code ads in the Here and Strings.xml 
        //*********************************************************
        public static bool ShowAdmobBanner = false;
        public static bool ShowAdmobInterstitial = false;
        public static bool ShowAdmobRewardVideo = false;
         
        public static string AdAppId = "ca-app-pub-5135691635931982~6131426175";
        public static string AdInterstitialKey = "ca-app-pub-5135691635931982/5365139416";
        public static string AdRewardVideoKey = "ca-app-pub-5135691635931982/3896021367";

        //Three times after entering the ad is displayed
        public static int ShowAdmobInterstitialCount = 2;
        public static int ShowAdmobRewardedVideoCount = 2;
         
        //########################### 

        //Last_Messages Page >>
        ///********************************************************* 
        public static bool RunSoundControl = true;
        public static int RefreshChatActivitiesSeconds = 6000; // 6 Seconds
        public static int MessageRequestSpeed = 3000; // 3 Seconds
                  
        //Set Theme Tab
        //*********************************************************
        public static bool SetTabColoredTheme = false;
        public static bool SetTabDarkTheme = false;

        public static string TabColoredColor = MainColor;
        public static bool SetTabIsTitledWithText = false;

        //Bypass Web Errors  
        //*********************************************************
        public static bool TurnTrustFailureOnWebException = true;
        public static bool TurnSecurityProtocolType3072On = true;

        //Show custom error reporting page
        public static bool RenderPriorityFastPostLoad = true;
    }
}
