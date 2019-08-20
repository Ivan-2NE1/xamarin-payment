using System;
using System.Collections.Generic;
using System.Linq;
using Android.Animation;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using AT.Markushi.UI;
using Com.Yuyakaido.Android.CardStackView;
using Java.Lang;
using ME.Alexrs.Wavedrawable;
using QuickDate.Activities.Premium;
using QuickDate.Activities.Tabbes.Adapters;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Utils;
using QuickDate.Helpers.Model;
using QuickDateClient.Classes.Global;
using QuickDateClient.Classes.Users;
using QuickDateClient.Requests;
using Exception = System.Exception;
using Fragment = Android.Support.V4.App.Fragment;

namespace QuickDate.Activities.Tabbes
{
    public class CardMachFragment : Fragment, CardStackView.ICardEventListener
    {
        #region Variables Basic

        public CardStackView CardStack;
        public CardAdapter CardDateAdapter;
        public CircleButton LikeButton, DesLikeButton, UndoButton;
        public HomeActivity GlobalContext;
        public WaveDrawable WaveDrawableAnimation;
        public ImageView ImageView;
        public ImageView PopularityImage;
        public LinearLayout BtnLayout;
        public ViewStub EmptyStateLayout;
        public View Inflated;
        public int CountOffset, totalCount;
        public string TotalIdLiked = "",TotalIdDisLiked = "";
        public SwipeDirection Direction;
        public int Index;
    

        #endregion

        #region General

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
            GlobalContext = (HomeActivity) Activity;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                View view = inflater.Inflate(Resource.Layout.TCardMachLayout, container, false);

                InitComponent(view);
                SetRecyclerViewAdapters();

                LikeButton.Click += LikeButtonOnClick;
                DesLikeButton.Click += DesLikeButtonOnClick;
                UndoButton.Click += UndoButtonOnClick;
                PopularityImage.Click += PopularityImageOnClick;

                GetMatches();
                 
                return view;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
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

        public void InitComponent(View view)
        {
            try
            {
                CardStack = view.FindViewById<CardStackView>(Resource.Id.activity_main_card_stack_view);
                LikeButton = view.FindViewById<CircleButton>(Resource.Id.likebutton2);
                DesLikeButton = view.FindViewById<CircleButton>(Resource.Id.closebutton1);
                UndoButton = view.FindViewById<CircleButton>(Resource.Id.Undobutton1);
                EmptyStateLayout = view.FindViewById<ViewStub>(Resource.Id.viewStub);
                PopularityImage = view.FindViewById<ImageView>(Resource.Id.coinImage);
              

                CardStack.SetCardEventListener(this);
                List<SwipeDirection> direction = new List<SwipeDirection> {SwipeDirection.Right, SwipeDirection.Left};
                CardStack.SetSwipeDirection(direction);


                //ROEO REQUISITOS PARA SER CLIENTE VERIFICADO
                var dataUser = ListUtils.MyUserInfo.FirstOrDefault(a => a.Id == UserDetails.UserId);
                if (dataUser.PhoneVerified == 0 ) { 
                    Toast.MakeText(Context, Context.GetText(Resource.String.Verifyprofile), ToastLength.Long).Show();
                }
                //FIN

                BtnLayout = view.FindViewById<LinearLayout>(Resource.Id.buttonLayout);
                ImageView = view.FindViewById<ImageView>(Resource.Id.userImageView);

                WaveDrawableAnimation = new WaveDrawable(Color.ParseColor(AppSettings.MainColor), 800);
                if (Build.VERSION.SdkInt < BuildVersionCodes.JellyBean)
                    ImageView.SetBackgroundDrawable(WaveDrawableAnimation);
                else
                    ImageView.Background = WaveDrawableAnimation;
                WaveDrawableAnimation.SetWaveInterpolator(new LinearInterpolator());
                WaveDrawableAnimation.StartAnimation();

                ImageView.Visibility = ViewStates.Visible;
                CardStack.Visibility = ViewStates.Invisible;
                BtnLayout.Visibility = ViewStates.Invisible; 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void SetRecyclerViewAdapters()
        {
            try
            {
                CardDateAdapter = new CardAdapter(Activity);
                CardStack.SetAdapter(CardDateAdapter);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Events

        private void PopularityImageOnClick(object sender, EventArgs e)
        {
            try
            {
                StartActivity(new Intent(Context, typeof(PopularityActivity)));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        private void UndoButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                var data = ListUtils.OldMatchesList.LastOrDefault();
                if (data != null)
                {
                    CardDateAdapter.UsersDateList.Insert(0, data);
                    CardDateAdapter.NotifyDataSetChanged();

                    ListUtils.OldMatchesList.Remove(data);
                }

                CardStack.Reverse();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void DesLikeButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                SetDesLikeDirection();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void LikeButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                SetLikeDirection();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void SetLikeDirection()
        {
            try
            {
                ValueAnimator rotation = ObjectAnimator.OfPropertyValuesHolder(CardStack.TopView, PropertyValuesHolder.OfFloat("rotation", 10f));
                rotation.SetDuration(200);
                ValueAnimator translateX = ObjectAnimator.OfPropertyValuesHolder(CardStack.TopView, PropertyValuesHolder.OfFloat("translationX", 0f, 2000f));
                ValueAnimator translateY = ObjectAnimator.OfPropertyValuesHolder(CardStack.TopView, PropertyValuesHolder.OfFloat("translationY", 0f, -500f));

                translateX.StartDelay = 100;
                translateY.StartDelay = 100;
                translateX.SetDuration(500);
                translateY.SetDuration(500);

                AnimatorSet cardAnimationSet = new AnimatorSet();
                cardAnimationSet.PlayTogether(rotation, translateX, translateY);

                ObjectAnimator overlayAnimator = ObjectAnimator.OfFloat(CardStack.TopView.OverlayContainer, "alpha", 0f, 1f);
                overlayAnimator.SetDuration(200);
                AnimatorSet overlayAnimationSet = new AnimatorSet();
                overlayAnimationSet.PlayTogether(overlayAnimator);

                CardStack.Swipe(SwipeDirection.Right, overlayAnimationSet);

                //int index = CardStack.TopIndex - 1;
                ////CardContainerView view = CardStack.BottomView;

                //if (index > -1)
                //    CardAppeared (index);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void SetDesLikeDirection()
        {
            try
            {
                ValueAnimator rotation = ObjectAnimator.OfPropertyValuesHolder(CardStack.TopView, PropertyValuesHolder.OfFloat("rotation", -10f));
                rotation.SetDuration(200);
                ValueAnimator translateX = ObjectAnimator.OfPropertyValuesHolder(CardStack.TopView, PropertyValuesHolder.OfFloat("translationX", 0f, -2000f));
                ValueAnimator translateY = ObjectAnimator.OfPropertyValuesHolder(CardStack.TopView, PropertyValuesHolder.OfFloat("translationY", 0f, -500f));

                translateX.StartDelay = 100;
                translateY.StartDelay = 100;
                translateX.SetDuration(500);
                translateY.SetDuration(500);

                AnimatorSet cardAnimationSet = new AnimatorSet();
                cardAnimationSet.PlayTogether(rotation, translateX, translateY);

                ObjectAnimator overlayAnimator = ObjectAnimator.OfFloat(CardStack.TopView.OverlayContainer, "alpha", 0f, 1f);
                overlayAnimator.SetDuration(200);
                AnimatorSet overlayAnimationSet = new AnimatorSet();
                overlayAnimationSet.PlayTogether(overlayAnimator);

                CardStack.Swipe(SwipeDirection.Left, overlayAnimationSet);

                //int index = CardStack.TopIndex - 1;
                ////CardContainerView view = CardStack.BottomView;
                //if (index > -1)
                //    CardDisappeared(index);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        #endregion

        #region CardEventListener

        public void OnCardClicked(int index)
        {
           
        }

        public void OnCardDragging(float percentX, float percentY)
        {
           
        }

        public void OnCardMovedToOrigin()
        {
           
        }

        public void OnCardReversed()
        {
            
        }

        public void OnCardSwiped(SwipeDirection direction)
        {
            try
            {
                Index = CardStack.TopIndex - 1;
                Direction = direction;
                new Handler(Looper.MainLooper).Post(new Runnable(Run)); 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void Run()
        {
            try
            {
                if (Direction == SwipeDirection.Right)
                {
                    CardAppeared(Index);
                }
                else if (Direction == SwipeDirection.Left)
                {
                    CardDisappeared(Index);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        public void CardAppeared(int position)
        {
            try
            {
                if (CardDateAdapter.UsersDateList.Count > 0)
                {
                    if (position == -1)
                        position = 0;

                    if (position >= 0)
                    {
                        //var textView = view.FindViewById<TextView>(Resource.Id.item_tourist_spot_card_name);
                        //if (textView != null)
                        //{
                        //    string name = textView.Text;
                        //}

                        if (position == CardDateAdapter.UsersDateList.Count)
                            position = CardDateAdapter.UsersDateList.Count - 1;

                        var data = CardDateAdapter.UsersDateList[position];
                        if (data != null)
                        {
                            if (data.IsLiked)
                            {
                                Activity.RunOnUiThread(() =>
                                {
                                    new DialogController(Activity).OpenDialogMatchFound(data);
                                });
                            }

                            ListUtils.LikedList.Add(data);
                            ListUtils.OldMatchesList.Add(data);

                            Activity.RunOnUiThread(() =>
                            {
                                try
                                {
                                    CardDateAdapter.UsersDateList.Remove(data);
                                    CardDateAdapter.NotifyDataSetChanged();
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);
                                }
                            });

                            totalCount = totalCount + 1;
                        }

                        CheckerCountCard();
                    }
                }
                else
                {
                    CheckerCountCard();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void CardDisappeared(int position)
        {
            try
            {
                if (CardDateAdapter.UsersDateList.Count > 0)
                {
                    if (position == -1)
                        position = 0;

                    if (position >= 0)
                    {
                        if (position == CardDateAdapter.UsersDateList.Count)
                            position = CardDateAdapter.UsersDateList.Count - 1;

                        var data = CardDateAdapter.UsersDateList[position];
                        if (data != null)
                        {
                            ListUtils.DisLikedList.Add(data);
                            ListUtils.OldMatchesList.Add(data);
                            totalCount = totalCount + 1;

                            Activity.RunOnUiThread(() =>
                            {
                                try
                                {
                                    CardDateAdapter.UsersDateList.Remove(data);
                                    CardDateAdapter.NotifyDataSetChanged();
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);
                                }
                            });
                        } 
                        CheckerCountCard();
                    }
                    else
                    {
                        CheckerCountCard();
                    }
                } 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        public void CheckerCountCard()
        {
            try
            { 
                if (totalCount >= 12 || CardDateAdapter.UsersDateList.Count == 0)
                {
                    if (ListUtils.LikedList.Count > 0)
                    {
                        //Get all id 
                        foreach (var item in ListUtils.LikedList)
                        {
                            TotalIdLiked += item.Id + ",";
                        }
                    }

                    if (ListUtils.DisLikedList.Count > 0)
                    {
                        //Get all id 
                        foreach (var item in ListUtils.DisLikedList)
                        {
                            TotalIdDisLiked += item.Id + ",";
                        }
                    }
                     
                    if (!string.IsNullOrEmpty(TotalIdLiked))
                        TotalIdLiked = TotalIdLiked.Remove(TotalIdLiked.Length - 1, 1);
                     
                    if (!string.IsNullOrEmpty(TotalIdDisLiked))
                        TotalIdDisLiked = TotalIdDisLiked.Remove(TotalIdDisLiked.Length - 1, 1);

                    if (!string.IsNullOrEmpty(TotalIdDisLiked) || !string.IsNullOrEmpty(TotalIdDisLiked)) //sent api 
                        RequestsAsync.Users.AddLikesAsync(TotalIdLiked, TotalIdDisLiked).ConfigureAwait(false); 

                    totalCount = 0;
                    ListUtils.LikedList.Clear();
                    ListUtils.DisLikedList.Clear();
                    TotalIdDisLiked = "";
                    TotalIdLiked = "";
                }

                //Load More
                int count = CardDateAdapter.UsersDateList.Count;
                if (count <= 5)
                {
                    CountOffset = CountOffset + 1;
                    GetMatches(CountOffset.ToString());
                } 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        public async void GetMatches(string offset = "0")
        {
            try
            {
                if (IMethods.CheckConnectivity())
                {
                    (int apiStatus, var respond) = await RequestsAsync.Users.MatchesAsync(offset);
                    if (apiStatus == 200)
                    {
                        if (respond is MatchesObject result)
                        {
                            if (result.Data?.Count > 0)
                            {
                                foreach (var item in result.Data)
                                {
                                    var data = ListUtils.AllMatchesList.FirstOrDefault(a => a.Id == item.Id);
                                    if (data == null)
                                    {
                                        CardDateAdapter.UsersDateList.Add(item);
                                        ListUtils.AllMatchesList.Add(item);
                                    } 
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(offset))
                                    Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_NoMoreUsers), ToastLength.Long).Show();
                            }
                        }
                    }
                    else if (apiStatus == 400)
                    {
                        if (respond is ErrorObject error)
                        {
                            var errorText = error.ErrorData.ErrorText;
                            if (errorText.Contains("Permission Denied"))
                                ApiRequest.Logout(Activity);
                        }
                    }
                    else if (apiStatus == 404)
                    {
                        var error = respond.ToString();
                        //Toast.MakeText(this, error, ToastLength.Short).Show();
                    }

                    Activity.RunOnUiThread(() =>
                    {
                        try
                        {
                            if (CardDateAdapter.UsersDateList.Count > 0)
                            {
                                CardDateAdapter.NotifyDataSetChanged();
                                 
                                ImageView.Visibility = ViewStates.Gone;
                                CardStack.Visibility = ViewStates.Visible;
                                BtnLayout.Visibility = ViewStates.Visible;
                                EmptyStateLayout.Visibility = ViewStates.Gone;

                                if (offset == "0")
                                {
                                    if (WaveDrawableAnimation.IsAnimationRunning)
                                        WaveDrawableAnimation?.StopAnimation();
                                }
                            }
                            else
                            {
                                ImageView.Visibility = ViewStates.Gone;
                                CardStack.Visibility = ViewStates.Gone;
                                BtnLayout.Visibility = ViewStates.Gone;

                                if (Inflated == null)
                                    Inflated = EmptyStateLayout.Inflate();

                                EmptyStateInflater x = new EmptyStateInflater();
                                x.InflateLayout(Inflated, EmptyStateInflater.Type.NoMatches);
                                if (x.EmptyStateButton.HasOnClickListeners)
                                {
                                    x.EmptyStateButton.Click += null;
                                }

                                EmptyStateLayout.Visibility = ViewStates.Visible;
                            }

                            // Open Dialog Tutorial
                            OpenDialog();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e); 
                        } 
                    }); 
                }
                else
                {
                    Activity.RunOnUiThread(() =>
                    {
                        try
                        {
                            ImageView.Visibility = ViewStates.Gone;
                            CardStack.Visibility = ViewStates.Gone;
                            BtnLayout.Visibility = ViewStates.Gone;

                            Inflated = EmptyStateLayout.Inflate();
                            EmptyStateInflater x = new EmptyStateInflater();
                            x.InflateLayout(Inflated, EmptyStateInflater.Type.NoConnection);
                            if (!x.EmptyStateButton.HasOnClickListeners)
                            {
                                x.EmptyStateButton.Click += null;
                                x.EmptyStateButton.Click += EmptyStateButtonOnClick;
                            }

                            Toast.MakeText(Context, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long).Show();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e); 
                        } 
                    });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                GetMatches(offset);
            }
        }

        private void EmptyStateButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                if (IMethods.CheckConnectivity())
                {
                    GetMatches();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void OpenDialog()
        {
            try
            {
                var data = ListUtils.DataUserLoginList.FirstOrDefault();
                if (data != null)
                {
                    if (data.Status == "Pending")
                    {
                        new DialogController(Activity).OpenDialogSkipTutorial();  
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