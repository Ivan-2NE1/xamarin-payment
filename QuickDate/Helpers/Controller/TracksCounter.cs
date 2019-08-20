using System;
using System.Linq;
using Android.App;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.Ads;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;

namespace QuickDate.Helpers.Controller
{
    public class TracksCounter
    {
        public Activity ActivityContext;
        public HomeActivity GlobalContext;

        public static int CountClick = 0;
        public TracksCounterEnum LastCounterEnum;

        public enum TracksCounterEnum
        {
            AdsInterstitial,
            AdsRewardedVideo,
            AddImage,
            UpgradePremium,
            AddCredit,
            AddPhoneNumber,
        }
         
        public TracksCounter(Activity activity)
        {
            try
            {
                ActivityContext = activity;
                GlobalContext = HomeActivity.GetInstance() ?? (HomeActivity) ActivityContext;
            }
            catch (Exception e)
            {
                Console.WriteLine(e); 
            } 
        }

        public void CheckTracksCounter()
        {
            try
            {
                CountClick = CountClick + 1; 
                var lastAvatar = ListUtils.SettingsSiteList.FirstOrDefault()?.UserDefaultAvatar?.Split('/').Last() ?? "d-avatar.jpg"; 
                 
                var dataUser = ListUtils.MyUserInfo.FirstOrDefault(a => a.Id == UserDetails.UserId);
                if (dataUser != null)
                {
                    if (CountClick == 3)
                    {
                        if (UserDetails.Avatar.Contains(lastAvatar))
                        {
                            LastCounterEnum = TracksCounterEnum.AddImage;
                            GlobalContext?.OpenAddPhotoFragment();
                        }
                        else
                        {
                            if (dataUser.IsPro == "0")
                            {
                                LastCounterEnum = TracksCounterEnum.AdsInterstitial;
                                AdsGoogle.Ad_Interstitial(ActivityContext);
                            } 
                        } 
                    }
                    else if (CountClick == 7)
                    {
                        if ((dataUser.Balance == "0.00" || dataUser.Balance == "0.0" || dataUser.Balance == "0") && LastCounterEnum != TracksCounterEnum.AddCredit)
                        {
                            LastCounterEnum = TracksCounterEnum.AddCredit;

                            var window = new PopupController(ActivityContext);
                            window.DisplayCreditWindow("credits");
                        }
                        else
                        {
                            if (dataUser.IsPro == "0")
                            {
                                LastCounterEnum = TracksCounterEnum.AdsRewardedVideo;
                                AdsGoogle.Ad_RewardedVideo(ActivityContext);
                            } 
                        }
                    }
                    else if (CountClick == 10)
                    { 
                        if (dataUser.PhoneVerified == 0 && LastCounterEnum != TracksCounterEnum.AddPhoneNumber)
                        {
                            LastCounterEnum = TracksCounterEnum.AddPhoneNumber;

                            var window = new PopupController(ActivityContext);
                            window.DisplayAddPhoneNumber();
                        }
                        else if (UserDetails.Avatar.Contains(lastAvatar) && LastCounterEnum != TracksCounterEnum.AddImage)
                        { 
                            LastCounterEnum = TracksCounterEnum.AddImage;
                            GlobalContext?.OpenAddPhotoFragment();
                        }
                        else if (!dataUser.VerifiedFinal)
                        {
                            LastCounterEnum = TracksCounterEnum.UpgradePremium;

                            var window = new PopupController(ActivityContext);
                            window.DisplayPremiumWindow();
                        }
                        else
                        {
                            if (dataUser.IsPro == "0")
                            {
                                LastCounterEnum = TracksCounterEnum.AdsInterstitial;
                                AdsGoogle.Ad_Interstitial(ActivityContext);
                            }
                        }

                        CountClick = 0;
                    }
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

    }
}