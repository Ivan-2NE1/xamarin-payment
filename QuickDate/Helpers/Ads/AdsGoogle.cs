using System;
using Android.App;
using Android.Content;
using Android.Gms.Ads;
using Android.Gms.Ads.Reward;
using Android.OS;
using Google.Ads.Mediation.Admob;

namespace QuickDate.Helpers.Ads
{
    public class AdsGoogle
    {
        public static int CountInterstitial = 0;
        public static int CountRewarded = 0;

        #region Interstitial

        public class AdmobInterstitial
        {
            public InterstitialAd _ad;

            public void ShowAd(Context context)
            {
                try
                {
                    _ad = new InterstitialAd(context);
                    _ad.AdUnitId = AppSettings.AdInterstitialKey;

                    var intlistener = new InterstitialAdListener(_ad);
                    intlistener.OnAdLoaded();
                    _ad.AdListener = intlistener;
                    //string android_id = Android.Provider.Settings.Secure.GetString(context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
                    var requestbuilder = new AdRequest.Builder();
                    // requestbuilder.AddTestDevice(android_id);
                    _ad.LoadAd(requestbuilder.Build());
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
        }

        public class InterstitialAdListener : AdListener
        {
            public InterstitialAd _ad;

            public InterstitialAdListener(InterstitialAd ad)
            {
                _ad = ad;
            }

            public override void OnAdLoaded()
            {
                base.OnAdLoaded();

                if (_ad.IsLoaded)
                    _ad.Show();
            }
        }


        public static void Ad_Interstitial(Context context)
        {
            try
            {
                if (AppSettings.ShowAdmobInterstitial)
                {
                    if (CountInterstitial == AppSettings.ShowAdmobInterstitialCount)
                    {
                        CountInterstitial = 0;
                        AdmobInterstitial ads = new AdmobInterstitial();
                        ads.ShowAd(context);
                    }

                    CountInterstitial++;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }


        #endregion

        //Rewarded Video >>
        //===================================================

        #region Rewarded

        public class AdmobRewardedVideo : AdListener, IRewardedVideoAdListener
        {
            public IRewardedVideoAd Rad;

            public void ShowAd(Context context)
            {
                try
                {
                    // Use an activity context to get the rewarded video instance.
                    Rad = MobileAds.GetRewardedVideoAdInstance(context);
                    Rad.RewardedVideoAdListener = this;

                    OnRewardedVideoAdLoaded();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }

            public override void OnAdLoaded()
            {
                try
                {
                    base.OnAdLoaded();

                    OnRewardedVideoAdLoaded();

                    if (Rad.IsLoaded)
                        Rad.Show();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }

            public void OnRewarded(IRewardItem reward)
            {
                //Toast.MakeText(Application.Context, "onRewarded! currency: " + reward.Type + "  amount: " + reward.Amount , ToastLength.Short).Show();

                if (Rad.IsLoaded)
                    Rad.Show();
            }

            public void RewardedVideoAdClosed()
            {
                try
                {
                    OnRewardedVideoAdLoaded();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            public void OnRewardedVideoAdFailedToLoad(int errorCode)
            {
                //Toast.MakeText(Application.Context, "No ads currently available", ToastLength.Short).Show();
            }

            public void OnRewardedVideoAdLeftApplication()
            {

            }

            public void OnRewardedVideoAdLoaded()
            {
                try
                {
                    string android_id = Android.Provider.Settings.Secure.GetString(Application.Context.ContentResolver,
                        Android.Provider.Settings.Secure.AndroidId);

                    Bundle extras = new Bundle();
                    extras.PutBoolean("_noRefresh", true);

                    var requestBuilder = new AdRequest.Builder();
                    requestBuilder.AddTestDevice(android_id);
                    requestBuilder.AddNetworkExtrasBundle(new AdMobAdapter().Class, extras);
                    Rad.UserId = AppSettings.AdAppId;
                    Rad.LoadAd(AppSettings.AdRewardVideoKey, requestBuilder.Build());
                    Rad.Show();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e); 
                } 
            }

            public void OnRewardedVideoAdOpened()
            {

            }

            public void OnRewardedVideoStarted()
            {

            }

            void IRewardedVideoAdListener.OnRewardedVideoAdClosed()
            {
                RewardedVideoAdClosed();
            }
        }

        public static void Ad_RewardedVideo(Context context)
        {
            try
            {
                if (AppSettings.ShowAdmobRewardVideo)
                {
                    if (CountRewarded == AppSettings.ShowAdmobRewardedVideoCount)
                    {
                        CountRewarded = 0;
                        AdmobRewardedVideo ads = new AdmobRewardedVideo();
                        ads.ShowAd(context);
                    }

                    CountRewarded++;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion
         
    }
}