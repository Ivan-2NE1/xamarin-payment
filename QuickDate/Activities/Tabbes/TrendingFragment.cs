using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Locations;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Bumptech.Glide.Integration.RecyclerView;
using Bumptech.Glide.Util;
using Com.Liaoinstan.SpringViewLib.Container;
using Com.Liaoinstan.SpringViewLib.Widgets;
using Newtonsoft.Json;
using Plugin.Geolocator;
using QuickDate.Activities.Tabbes.Adapters;
using QuickDate.Activities.UserProfile;
using QuickDate.ButtomSheets;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDateClient.Classes.Global;
using QuickDateClient.Classes.Users;
using QuickDateClient.Requests;
using Fragment = Android.Support.V4.App.Fragment;

namespace QuickDate.Activities.Tabbes
{
    public class TrendingFragment : Fragment, SpringView.IOnFreshListener 
    {
        #region Variables Basic

        public RecyclerView ProRecyclerView;
        public RecyclerView NearByRecyclerView;
        public NearByAdapter NearByAdapter;
        public ProgressBar ProgressBarLoader;
        public ViewStub EmptyStateLayout;
        public View Inflated;
        public HomeActivity GlobalContext;
        public TextView ToolbarTitle;
        public XamarinRecyclerViewOnScrollListener MainScrollEvent; 
        public ImageView FilterButton;
        public Location CurrentLocation;
        public LocationManager LocationManager;
        public bool ShowAlertDialogGps = true;
        public string LocationProvider;
        public int CountOffset = 0;
        public AppBarLayout AppBarLayout;
        public ProUserAdapter ProUserAdapter;
        public SpringView SwipeRefreshLayout;

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
                View view = inflater.Inflate(Resource.Layout.TTrendingLayout, container, false);

                InitComponent(view);
                SetRecyclerViewAdapters();

                FilterButton.Click += FilterButtonOnClick;

                InitializeLocationManager();
                 
                LoadProUser().ConfigureAwait(false);
                LoadUser();

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
                FilterButton = view.FindViewById<ImageView>(Resource.Id.Filterbutton);
                NearByRecyclerView = (RecyclerView) view.FindViewById(Resource.Id.Recylerusers);
                ProgressBarLoader = (ProgressBar) view.FindViewById(Resource.Id.sectionProgress);
                EmptyStateLayout = view.FindViewById<ViewStub>(Resource.Id.viewStub);
                ToolbarTitle = view.FindViewById<TextView>(Resource.Id.toolbartitle);
                ProRecyclerView = (RecyclerView) view.FindViewById(Resource.Id.proRecyler);

                ToolbarTitle.Text = AppSettings.ApplicationName;
                 
                SwipeRefreshLayout = (SpringView)view.FindViewById(Resource.Id.material_style_ptr_frame); 
                SwipeRefreshLayout.SetType(SpringView.Types.Overlap);
                SwipeRefreshLayout.Header = new Helpers.PullSwipeStyles.DefaultHeader(Activity);
                SwipeRefreshLayout.Footer = new MeituanFooter(Activity);
                SwipeRefreshLayout.SetEnable(true);
                SwipeRefreshLayout.SetListener(this);

                AppBarLayout = (AppBarLayout)view.FindViewById(Resource.Id.appBarLayout);
                AppBarLayout.SetExpanded(false);
                 
                ProgressBarLoader.Visibility = ViewStates.Visible;
                NearByRecyclerView.Visibility = ViewStates.Gone;
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
                NearByAdapter = new NearByAdapter(Activity);  
                NearByAdapter.OnItemClick += NearByAdapterOnItemClick;
                 
                StaggeredGridLayoutManager nearByLayoutManager = new StaggeredGridLayoutManager(3, LinearLayoutManager.Vertical);
                NearByRecyclerView.SetLayoutManager(nearByLayoutManager);
                //NearByRecyclerView.SetItemViewCacheSize(20);
                //NearByRecyclerView.HasFixedSize = true;
                //NearByRecyclerView.GetLayoutManager().ItemPrefetchEnabled = true;
                //var sizeProvider = new FixedPreloadSizeProvider(10, 10);
                //var preLoader = new RecyclerViewPreloader<UserInfoObject>(Activity, NearByAdapter, sizeProvider, 10);
                //NearByRecyclerView.AddOnScrollListener(preLoader);
                NearByRecyclerView.SetAdapter(NearByAdapter);
                 
                //Pro Recycler View 
                ProUserAdapter = new ProUserAdapter(Activity);
                ProUserAdapter.OnItemClick += ProUserAdapterOnItemClick;
                 
                ProRecyclerView.SetLayoutManager(new LinearLayoutManager(Activity, LinearLayoutManager.Horizontal, false));
                ProRecyclerView.SetItemViewCacheSize(20);
                ProRecyclerView.HasFixedSize = true;
                ProRecyclerView.GetLayoutManager().ItemPrefetchEnabled = true;
                var sizeProviderPro = new FixedPreloadSizeProvider(10, 10);
                var preLoaderPro = new RecyclerViewPreloader<UserInfoObject>(Activity, ProUserAdapter, sizeProviderPro, 10);
                ProRecyclerView.AddOnScrollListener(preLoaderPro);
                ProRecyclerView.SetAdapter(ProUserAdapter);

                if (MainScrollEvent != null) return;
                MainScrollEvent = new XamarinRecyclerViewOnScrollListener(nearByLayoutManager);
                NearByRecyclerView.AddOnScrollListener(MainScrollEvent);
                MainScrollEvent.LoadMoreEvent += SeconderScrollEventOnLoadMoreEvent;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        #endregion

        #region Events

        //Open search filter
        private void FilterButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                var searchFilter = new SearchFilterBottomDialogFragment();
                searchFilter.Show(Activity.SupportFragmentManager, "searchFilter");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Open profile user >> Near By
        private void NearByAdapterOnItemClick(object sender, SuggestionsAdapterClickEventArgs e)
        {
            try
            {
                if (e.Position >-1)
                {
                    var item = NearByAdapter.GetItem(e.Position);
                    if (item != null)
                    {
                        if (item.Id != UserDetails.UserId)
                        {
                            var intent = new Intent(Context, typeof(UserProfileActivity));
                            intent.PutExtra("EventPage", "Close");
                            intent.PutExtra("ItemUser", JsonConvert.SerializeObject(item));
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
                        else
                        {
                            GlobalContext.NavigationTabBar.SetModelIndex(4, true);
                        } 
                    } 
                } 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Open profile user  >> Pro User
        private void ProUserAdapterOnItemClick(object sender, ProUserAdapterClickEventArgs e)
        {
            try
            {
                if (e.Position > -1)
                {
                    var item = ProUserAdapter.GetItem(e.Position);
                    if (item != null)
                    {
                        if (item.Type == "Your")
                        {
                            var window = new PopupController(Activity);
                            window.DisplayPremiumWindow();
                        }
                        else
                        {
                            var intent = new Intent(Context, typeof(UserProfileActivity));
                            intent.PutExtra("EventPage", "Close");
                            intent.PutExtra("ItemUser", JsonConvert.SerializeObject(item));
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
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }


        #endregion

        #region Scroll

        private void SeconderScrollEventOnLoadMoreEvent(object sender, EventArgs eventArgs)
        {
            try
            {
                //Code get last id where LoadMore >>
                if (MainScrollEvent.IsLoading == false)
                {
                    MainScrollEvent.IsLoading = true;
                    CountOffset = CountOffset + 1;
                    LoadUsersJsonAsync(CountOffset.ToString());
                    MainScrollEvent.IsLoading = false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public class XamarinRecyclerViewOnScrollListener : RecyclerView.OnScrollListener
        {
            public delegate void LoadMoreEventHandler(object sender, EventArgs e);
            public event LoadMoreEventHandler LoadMoreEvent;
            public bool IsLoading = false;
            public StaggeredGridLayoutManager LayoutManager;
            public XamarinRecyclerViewOnScrollListener(StaggeredGridLayoutManager layoutManager)
            {
                try
                {
                    LayoutManager = layoutManager;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
            {
                try
                {
                    base.OnScrolled(recyclerView, dx, dy);

                    var visibleItemCount = recyclerView.ChildCount;
                    var totalItemCount = recyclerView.GetAdapter().ItemCount;

                    int[] firstVisibleItemPositions = new int[3];
                    var firstVisibleItem = LayoutManager.FindFirstVisibleItemPositions(firstVisibleItemPositions)[0];

                    var pastVisiblesItems = firstVisibleItem;
                    if (visibleItemCount + pastVisiblesItems + 8 >= totalItemCount)
                        if (IsLoading == false)
                        {
                            LoadMoreEvent?.Invoke(this, null);
                            IsLoading = true;
                        } 
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
        }
          
        #endregion

        #region Location

        private void InitializeLocationManager()
        {
            try
            {
                LocationManager = (LocationManager)Activity.GetSystemService(Context.LocationService);
                var criteriaForLocationService = new Criteria
                {
                    Accuracy = Accuracy.Fine
                };
                var acceptableLocationProviders = LocationManager.GetProviders(criteriaForLocationService, true);
                if (acceptableLocationProviders.Any())
                    LocationProvider = acceptableLocationProviders.First();
                else
                    LocationProvider = string.Empty;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Get Position GPS Current Location
        private async Task<bool> GetPosition()
        {
            try
            {                 
                if (CrossGeolocator.Current.IsGeolocationAvailable)
                {
                    // Check if we're running on Android 5.0 or higher
                    if ((int)Build.VERSION.SdkInt < 23)
                    {
                        CheckAndGetLocation();
                    }
                    else
                    {
                        if (Context.CheckSelfPermission(Manifest.Permission.AccessFineLocation) == Permission.Granted && Context.CheckSelfPermission(Manifest.Permission.AccessCoarseLocation) == Permission.Granted)
                        {
                            CheckAndGetLocation();
                        }
                        else
                        {
                            new PermissionsController(Activity).RequestPermission(105);
                        }
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public async void CheckAndGetLocation()
        {
            try
            {
                if (!LocationManager.IsProviderEnabled(LocationManager.GpsProvider))
                {
                    if (ShowAlertDialogGps)
                    {
                        ShowAlertDialogGps = false;

                        Activity.RunOnUiThread(() =>
                        {
                            try
                            {
                                // Call your Alert message
                                AlertDialog.Builder alert = new AlertDialog.Builder(Context);
                                alert.SetTitle(GetString(Resource.String.Lbl_Use_Location) + "?");
                                alert.SetMessage(GetString(Resource.String.Lbl_GPS_is_disabled) + "?");

                                alert.SetPositiveButton(GetString(Resource.String.Lbl_Ok), (senderAlert, args) =>
                                {
                                    //Open intent Gps
                                    new IntentController(Activity).OpenIntentGps(LocationManager);
                                });

                                alert.SetNegativeButton(GetString(Resource.String.Lbl_Cancel), (senderAlert, args) => { });

                                Dialog gpsDialog = alert.Create();
                                gpsDialog.Show();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e); 
                            } 
                        });
                    }
                }
                else
                {
                    var locator = CrossGeolocator.Current;
                    locator.DesiredAccuracy = 50;
                    var position = await locator.GetPositionAsync(TimeSpan.FromMilliseconds(10000));
                    Console.WriteLine("Position Status: {0}", position.Timestamp);
                    Console.WriteLine("Position Latitude: {0}", position.Latitude);
                    Console.WriteLine("Position Longitude: {0}", position.Longitude);

                    UserDetails.Lat = position.Latitude.ToString(CultureInfo.InvariantCulture);
                    UserDetails.Lng = position.Longitude.ToString(CultureInfo.InvariantCulture);
                }

                LoadUsersJsonAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region LoadUser
          
        public async void LoadUser()
        {
            try
            { 
                // Check if we're running on Android 5.0 or higher
                if ((int) Build.VERSION.SdkInt < 23)
                    await GetPosition();
                else
                {
                    if (Context.CheckSelfPermission(Manifest.Permission.AccessFineLocation) == Permission.Granted &&Context.CheckSelfPermission(Manifest.Permission.AccessCoarseLocation) == Permission.Granted)
                        await GetPosition();
                    else
                        new PermissionsController(Activity).RequestPermission(105);
                } 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                LoadUser();
            }
        }

        public async void LoadUsersJsonAsync(string offset = "0")
        {
            try
            {
                if (!IMethods.CheckConnectivity())
                {
                    if (ProgressBarLoader.Visibility != ViewStates.Gone)
                        ProgressBarLoader.Visibility = ViewStates.Gone;

                    Inflated = EmptyStateLayout.Inflate();
                    EmptyStateInflater x = new EmptyStateInflater();
                    x.InflateLayout(Inflated, EmptyStateInflater.Type.NoConnection);
                    if (!x.EmptyStateButton.HasOnClickListeners)
                    {
                        x.EmptyStateButton.Click += null;
                        x.EmptyStateButton.Click += EmptyStateButtonOnClick;
                    }

                    Toast.MakeText(Context, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }
                else
                {                     
                    var dictionary = new Dictionary<string, string>
                    {
                        {"limit", "35"},
                        {"offset", offset},
                        {"_gender", UserDetails.Gender},
                        //{"_interest", ""}, // keyword
                        {"_located", UserDetails.Located},
                        {"_location", UserDetails.Location},
                        {"_age_from", UserDetails.AgeMin.ToString()},
                        {"_age_to",  UserDetails.AgeMax.ToString()},
                        {"_lat", UserDetails.Lat},
                        {"_lng", UserDetails.Lng}
                    };

                    int countList = NearByAdapter.NearByList.Count;
                    (int apiStatus, var respond) = await RequestsAsync.Users.SearchAsync(dictionary);
                    if (apiStatus == 200)
                    {
                        if (respond is SearchObject result)
                        {
                            var respondList = result.Data?.Count;
                            if (respondList > 0)
                            {
                                foreach (var item in result.Data)
                                {
                                    var check = NearByAdapter.NearByList.FirstOrDefault(a => a.Id == item.Id);
                                    if (check == null)
                                    {
                                        if (UserDetails.SwitchState)
                                        {
                                            var online = QuickDateTools.GetStatusOnline(item.Lastseen, item.Online);
                                            if (online)
                                            {
                                                NearByAdapter.NearByList.Add(item);
                                            }
                                        }
                                        else
                                        {
                                            NearByAdapter.NearByList.Add(item);
                                        }
                                    }
                                }

                                if (countList > 0)
                                    NearByAdapter.NotifyItemRangeInserted(countList - 1, NearByAdapter.NearByList.Count - countList);
                                else
                                    NearByAdapter.NotifyDataSetChanged();
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

                    if (NearByAdapter.NearByList.Count > 0)
                    {
                        EmptyStateLayout.Visibility = ViewStates.Gone;
                        NearByRecyclerView.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        NearByRecyclerView.Visibility = ViewStates.Gone;
                        
                        if (Inflated == null)
                            Inflated = EmptyStateLayout.Inflate();

                        EmptyStateInflater x = new EmptyStateInflater();
                        x.InflateLayout(Inflated, EmptyStateInflater.Type.NoUsers);
                        if (x.EmptyStateButton.HasOnClickListeners)
                        {
                            x.EmptyStateButton.Click += null;
                        }
                        EmptyStateLayout.Visibility = ViewStates.Visible;
                    }

                    MainScrollEvent.IsLoading = false;
                    ProgressBarLoader.Visibility = ViewStates.Gone;
                    SwipeRefreshLayout?.OnFinishFreshAndLoad();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                LoadUsersJsonAsync(offset);
            }
        }

        private void EmptyStateButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                if (IMethods.CheckConnectivity())
                {
                    EmptyStateLayout.Visibility = ViewStates.Invisible;
                    ProgressBarLoader.Visibility = ViewStates.Visible;
                    LoadUsersJsonAsync();
                }
                else
                {
                    if (EmptyStateLayout.Visibility != ViewStates.Visible)
                        EmptyStateLayout.Visibility = ViewStates.Visible;

                    if (ProgressBarLoader.Visibility != ViewStates.Gone)
                        ProgressBarLoader.Visibility = ViewStates.Gone;

                    Toast.MakeText(Activity, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region Load Pro User Api 

        public async Task LoadProUser()
        {
            try
            {
                if (IMethods.CheckConnectivity())
                {

                    int countList = ProUserAdapter.ProUserList.Count;
                    var (respondCode, respondString) = await RequestsAsync.Users.GetProAsync("15").ConfigureAwait(false);
                    if (respondCode == 200)
                    {
                        if (respondString is SearchObject result)
                        {
                            var respondList = result.Data?.Count;
                            if (respondList > 0)
                            {
                                if (countList > 0)
                                {
                                    foreach (var item in result.Data)
                                    {
                                        var check = ProUserAdapter.ProUserList.FirstOrDefault(a => a.Id == item.Id);
                                        if (check == null)
                                        {
                                            ProUserAdapter.ProUserList.Add(item);
                                        }
                                    }

                                    Activity.RunOnUiThread(() => { ProUserAdapter.NotifyItemRangeInserted(countList - 1, ProUserAdapter.ProUserList.Count - countList);  });
                                }
                                else
                                {
                                    ProUserAdapter.ProUserList = new ObservableCollection<UserInfoObject>(result?.Data);
                                    Activity.RunOnUiThread(() => { ProUserAdapter.NotifyDataSetChanged(); });
                                }
                            }
                            else
                            {
                                Toast.MakeText(Context, "No more user found", ToastLength.Short).Show();
                            }
                        }
                    } 
                    else if (respondCode == 400)
                    {
                        if (respondString is ErrorObject error)
                        {
                            var errorText = error.ErrorData.ErrorText;
                            if (errorText.Contains("Permission Denied"))
                                ApiRequest.Logout(Activity);
                        }
                    }
                    else if (respondCode == 404)
                    {
                        var error = respondString.ToString();
                        //Toast.MakeText(this, error, ToastLength.Short).Show();
                    }

                    var data = ListUtils.MyUserInfo?.FirstOrDefault(a => a.Id == UserDetails.UserId);
                    if (data?.IsPro != "1")
                    {
                        var dataOwner = ProUserAdapter.ProUserList.FirstOrDefault(a => a.Type == "Your");
                        if (dataOwner == null)
                        {
                            ProUserAdapter.ProUserList.Insert(0, new UserInfoObject()
                            {
                                Avater = UserDetails.Avatar,
                                Type = "Your",
                                Username = Context.GetText(Resource.String.Lbl_AddMe),
                                IsOwner = true,
                            });

                            Activity.RunOnUiThread(() => { ProUserAdapter.NotifyItemInserted(0); });
                        }
                    } 
                    
                    Activity.RunOnUiThread(() =>
                    {
                        try
                        { 
                            ProRecyclerView.Visibility = ViewStates.Visible;
                            AppBarLayout.SetExpanded(true);
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
            }
        }

        #endregion

        #region Refresh

        public void OnRefresh()
        {
            try
            {
                ProUserAdapter.ProUserList.Clear();
                ProUserAdapter.NotifyDataSetChanged();

                NearByAdapter.NearByList.Clear();
                NearByAdapter.NotifyDataSetChanged();

                ProgressBarLoader.Visibility = ViewStates.Gone;
                EmptyStateLayout.Visibility = ViewStates.Gone;
                NearByRecyclerView.Visibility = ViewStates.Visible;
                 
                LoadUser();
                LoadProUser().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnLoadMore()
        {
            try
            {
                //Code get last id where LoadMore >>
                if (MainScrollEvent.IsLoading == false)
                {
                    MainScrollEvent.IsLoading = true;
                    CountOffset = CountOffset + 1;
                    LoadUsersJsonAsync(CountOffset.ToString());

                    SwipeRefreshLayout.OnFinishFreshAndLoad();
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