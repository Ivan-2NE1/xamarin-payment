using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Bumptech.Glide.Integration.RecyclerView;
using Bumptech.Glide.Util;
using Newtonsoft.Json;
using QuickDate.Activities.Tabbes.Adapters;
using QuickDate.Activities.UserProfile;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Utils;
using QuickDateClient.Classes.Common;
using QuickDateClient.Classes.Global;
using QuickDateClient.Requests;
using Fragment = Android.Support.V4.App.Fragment;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace QuickDate.Activities.Tabbes
{
    public class NotificationsFragment : Fragment
    {
        #region Variables Basic

        public RecyclerView NotificationRecycler;
        public ViewStub EmptyStateLayout;
        public View Inflated;
        public RecyclerView.LayoutManager NotifyLayoutManager;
        public Toolbar ToolbarView;
        public NotificationsAdapter NotifyAdapter;
        public SwipeRefreshLayout SwipeRefreshLayout;
        public RelativeLayout MatchesButton, LikesButton, VisitsButton;
        public LinearLayout MatchesLayout, LikesLayout, VisitsLayout;
        public ImageView MatchesImage, LikesImage, VisitsImage;
        public TextView MatchesTextView, LikesTextView, VisitsTextView;
        public LinearLayout TabButtons;
        public HomeActivity GlobalContext;

        #endregion
         
        #region General

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
            GlobalContext = (HomeActivity)Activity;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                View view = inflater.Inflate(Resource.Layout.TNotificationsLayout, container, false);

                InitComponent(view);
                InitToolbar(view);
                SetRecyclerViewAdapters();

                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                {
                    Activity.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                    Activity.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                    Activity.Window.SetStatusBarColor(Color.ParseColor("#542CDE"));
                }

                MatchesButton.Click += MatchesButtonOnClick;
                LikesButton.Click += LikesButtonOnClick;
                VisitsButton.Click += VisitsButtonOnClick;
                SwipeRefreshLayout.Refresh += SwipeRefreshLayoutOnRefresh;
                LoadNotificationFeed();

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
                TabButtons = view.FindViewById<LinearLayout>(Resource.Id.TabButtons);
                NotificationRecycler = (RecyclerView)view.FindViewById(Resource.Id.NotifcationRecyler);
                EmptyStateLayout = view.FindViewById<ViewStub>(Resource.Id.viewStub);
                MatchesButton = view.FindViewById<RelativeLayout>(Resource.Id.MatchesButton);
                LikesButton = view.FindViewById<RelativeLayout>(Resource.Id.LikesButton);
                VisitsButton = view.FindViewById<RelativeLayout>(Resource.Id.VisitsButton);
                MatchesLayout = view.FindViewById<LinearLayout>(Resource.Id.bt1);
                VisitsLayout = view.FindViewById<LinearLayout>(Resource.Id.bt2);
                LikesLayout = view.FindViewById<LinearLayout>(Resource.Id.bt3);

                MatchesImage = view.FindViewById<ImageView>(Resource.Id.ImageView1);
                VisitsImage = view.FindViewById<ImageView>(Resource.Id.ImageView2);
                LikesImage = view.FindViewById<ImageView>(Resource.Id.ImageView3);

                MatchesTextView = view.FindViewById<TextView>(Resource.Id.text1);
                LikesTextView = view.FindViewById<TextView>(Resource.Id.text3);
                VisitsTextView = view.FindViewById<TextView>(Resource.Id.text2);

                SwipeRefreshLayout = (SwipeRefreshLayout)view.FindViewById(Resource.Id.swipeRefreshLayout);
                SwipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight, Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight, Android.Resource.Color.HoloRedLight);
                SwipeRefreshLayout.Refreshing = true;
                SwipeRefreshLayout.Enabled = true;
                 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void InitToolbar(View view)
        {
            try
            {
                ToolbarView = view.FindViewById<Toolbar>(Resource.Id.toolbar);
                ToolbarView.Title = GetString(Resource.String.Lbl_Notifications);
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
                NotifyAdapter = new NotificationsAdapter(Activity);
                NotifyAdapter.NotificationsList = ListUtils.MatchList;
                  
                NotifyLayoutManager = new LinearLayoutManager(Activity);
                NotificationRecycler.SetLayoutManager(NotifyLayoutManager);
                NotificationRecycler.SetItemViewCacheSize(20);
                NotificationRecycler.HasFixedSize = true;
                NotificationRecycler.SetItemViewCacheSize(10);
                NotificationRecycler.GetLayoutManager().ItemPrefetchEnabled = true;
                var sizeProvider = new FixedPreloadSizeProvider(10, 10);
                var preLoader = new RecyclerViewPreloader<GetNotificationsObject.Datum>(Activity, NotifyAdapter, sizeProvider, 10);
                NotificationRecycler.AddOnScrollListener(preLoader);
                NotificationRecycler.SetAdapter(NotifyAdapter);
                 
                NotifyAdapter.OnItemClick += NotifyAdapterOnItemClick;

                TranslateAnimation animation1 = new TranslateAnimation(1500.0f, 0.0f, 0.0f, 0.0f); // new TranslateAnimation(xFrom,xTo, yFrom,yTo)
                animation1.Duration = 500; // animation duration
                animation1.FillAfter = true;
                TabButtons.StartAnimation(animation1);
                animation1 = new TranslateAnimation(0.0f, 0.0f, 1500.0f, 0.0f);
                animation1.Duration = 700; // animation duration
                NotificationRecycler.StartAnimation(animation1);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        #endregion

        #region Event

        //Open user profile
        private void NotifyAdapterOnItemClick(object sender, NotificationsAdapterClickEventArgs e)
        {
            try
            {
                if (e.Position > -1)
                {
                    var item = NotifyAdapter.GetItem(e.Position);
                    if (item != null)
                    {
                        var intent = new Intent(Context, typeof(UserProfileActivity));

                        if (item.Type == "got_new_match")
                        {
                            intent.PutExtra("EventPage", "HideButton");
                        }
                        else if (item.Type == "like")
                        {
                            intent.PutExtra("EventPage", "likeAndClose");
                        }
                        else  
                        {
                            intent.PutExtra("EventPage", "Close");
                        }
                        intent.PutExtra("ItemUser", JsonConvert.SerializeObject(item.Notifier));
                        if (AppSettings.EnableAddAnimationImageUser)
                        {
                            ActivityOptionsCompat options = ActivityOptionsCompat.MakeSceneTransitionAnimation((Activity)Context, e.Image, "profileimage");
                            Context.StartActivity(intent, options.ToBundle());
                        }
                        else
                        {
                            Context.StartActivity(intent);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        private void VisitsButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                NotifyAdapter.NotificationsList = ListUtils.VisitsList;
                NotifyAdapter.NotifyDataSetChanged();
                ToolbarView.SetBackgroundResource(Resource.Drawable.Shape_Gradient_Normal2);
                VisitsImage.SetColorFilter(Color.ParseColor("#ffffff"));
                VisitsTextView.SetTextColor(Color.ParseColor("#DB2251"));
                VisitsLayout.SetBackgroundResource(Resource.Drawable.Shape_Radius_Gradient_Btn3);
                VisitsButton.Tag = "Clicked";

                ResetTabsButtonOnVistsClick();

                if (Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
                    return;

                Activity.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                Activity.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                Activity.Window.SetStatusBarColor(Color.ParseColor(AppSettings.EndColor));

                ShowEmptyPage();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void LikesButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                NotifyAdapter.NotificationsList = ListUtils.LikesList;
                NotifyAdapter.NotifyDataSetChanged();

                ToolbarView.SetBackgroundResource(Resource.Drawable.Shape_Gradient_Normal2);
                LikesImage.SetColorFilter(Color.ParseColor("#ffffff"));
                LikesTextView.SetTextColor(Color.ParseColor("#DB2251"));
                //LikesLayout.SetBackgroundResource(Resource.Drawable.Shape_Radius_Gradient_Btn);
                LikesLayout.SetBackgroundResource(Resource.Drawable.Shape_Radius_Gradient_Btn3);
                LikesButton.Tag = "Clicked";

                ResetTabsButtonOnLikesClick();

                if (Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
                    return;

                Activity.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                Activity.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                Activity.Window.SetStatusBarColor(Color.ParseColor(AppSettings.EndColor));

                ShowEmptyPage();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void MatchesButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                NotifyAdapter.NotificationsList = ListUtils.MatchList;
                NotifyAdapter.NotifyDataSetChanged();

                ToolbarView.SetBackgroundResource(Resource.Drawable.Shape_Gradient_Normal3);
                MatchesImage.SetColorFilter(Color.ParseColor("#ffffff"));
                MatchesTextView.SetTextColor(Color.ParseColor("#DB2251"));
                MatchesLayout.SetBackgroundResource(Resource.Drawable.Shape_Radius_Gradient_Btn3);
                MatchesButton.Tag = "Clicked";

                ResetTabsButtonOnMatchClick();

                if (Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
                    return;

                Activity.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                Activity.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                Activity.Window.SetStatusBarColor(Color.ParseColor(AppSettings.EndColor));

                ShowEmptyPage();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void SwipeRefreshLayoutOnRefresh(object sender, EventArgs e)
        {
            try
            {
                ListUtils.MatchList.Clear();
                ListUtils.LikesList.Clear();
                ListUtils.VisitsList.Clear();
                NotifyAdapter.NotificationsList.Clear();
                NotifyAdapter.NotifyDataSetChanged();

                LoadNotificationFeed();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception); 
            } 
        }
         
        #endregion

        #region Set Tab

        public void ResetTabsButtonOnMatchClick()
        {
            try
            {
                LikesImage.SetColorFilter(Color.ParseColor("#A1A1A1"));
                LikesTextView.SetTextColor(Color.ParseColor("#A1A1A1"));
                LikesLayout.SetBackgroundResource(Resource.Drawable.Shape_Radius_Line_Grey);
                LikesButton.Tag = "UnClicked";
                VisitsImage.SetColorFilter(Color.ParseColor("#A1A1A1"));
                VisitsTextView.SetTextColor(Color.ParseColor("#A1A1A1"));
                VisitsLayout.SetBackgroundResource(Resource.Drawable.Shape_Radius_Line_Grey);
                VisitsButton.Tag = "UnClicked";
            }
            catch (Exception e)
            {
                Console.WriteLine(e); 
            } 
        }

        public void ResetTabsButtonOnVistsClick()
        {
            try
            {
                MatchesImage.SetColorFilter(Color.ParseColor("#A1A1A1"));
                MatchesTextView.SetTextColor(Color.ParseColor("#A1A1A1"));
                MatchesLayout.SetBackgroundResource(Resource.Drawable.Shape_Radius_Line_Grey);
                MatchesButton.Tag = "UnClicked";

                LikesImage.SetColorFilter(Color.ParseColor("#A1A1A1"));
                LikesTextView.SetTextColor(Color.ParseColor("#A1A1A1"));
                LikesLayout.SetBackgroundResource(Resource.Drawable.Shape_Radius_Line_Grey);
                LikesButton.Tag = "UnClicked";
            }
            catch (Exception e)
            {
                Console.WriteLine(e); 
            } 
        }

        public void ResetTabsButtonOnLikesClick()
        {
            try
            {
                MatchesImage.SetColorFilter(Color.ParseColor("#A1A1A1"));
                MatchesTextView.SetTextColor(Color.ParseColor("#A1A1A1"));
                MatchesLayout.SetBackgroundResource(Resource.Drawable.Shape_Radius_Line_Grey);
                MatchesButton.Tag = "UnClicked";

                VisitsImage.SetColorFilter(Color.ParseColor("#A1A1A1"));
                VisitsTextView.SetTextColor(Color.ParseColor("#A1A1A1"));
                VisitsLayout.SetBackgroundResource(Resource.Drawable.Shape_Radius_Line_Grey);
                VisitsButton.Tag = "UnClicked";
            }
            catch (Exception e)
            {
                Console.WriteLine(e); 
            }  
        }

        #endregion
         
        public async void LoadNotificationFeed(string offset = "0")
        {
            try
            {
                if (IMethods.CheckConnectivity())
                {
                    (int apiStatus, var respond) = await RequestsAsync.Common.GetNotificationsAsync("25",offset);
                    if (apiStatus == 200)
                    {
                        if (respond is GetNotificationsObject result)
                        {
                            if (result.Data?.Count > 0)
                            {
                                foreach (var item in result.Data)
                                {
                                    item.Text = QuickDateTools.GetNotification(item);
                                }
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

                    SwipeRefreshLayout.Refreshing = false;
                    ShowEmptyPage();
                }
                else
                {
                    NotificationRecycler.Visibility = ViewStates.Gone;
                    
                    Inflated = EmptyStateLayout.Inflate();
                    EmptyStateInflater x = new EmptyStateInflater();
                    x.InflateLayout(Inflated, EmptyStateInflater.Type.NoConnection);
                    if (!x.EmptyStateButton.HasOnClickListeners)
                    {
                        x.EmptyStateButton.Click += null;
                        x.EmptyStateButton.Click += EmptyStateButtonOnClick;
                    }

                    SwipeRefreshLayout.Refreshing = false;

                    Toast.MakeText(Context, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long).Show();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                LoadNotificationFeed(offset);
            }
        }

        public void ShowEmptyPage()
        {
            try
            { 
                if (NotifyAdapter.NotificationsList.Count > 0)
                {
                    NotifyAdapter.NotifyDataSetChanged();
                    NotificationRecycler.Visibility = ViewStates.Visible;
                    SwipeRefreshLayout.Refreshing = false;
                    EmptyStateLayout.Visibility = ViewStates.Gone;
                }
                else
                {
                    NotificationRecycler.Visibility = ViewStates.Gone;

                    if (Inflated == null)
                        Inflated = EmptyStateLayout.Inflate();

                    EmptyStateInflater x = new EmptyStateInflater();
                    x.InflateLayout(Inflated, EmptyStateInflater.Type.NoNotifications);
                    if (x.EmptyStateButton.HasOnClickListeners)
                    {
                        x.EmptyStateButton.Click += null;
                    }
                    EmptyStateLayout.Visibility = ViewStates.Visible;
                } 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void EmptyStateButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                if (IMethods.CheckConnectivity())
                {
                    LoadNotificationFeed();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}