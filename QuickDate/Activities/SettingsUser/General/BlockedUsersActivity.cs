using System;
using System.Collections.ObjectModel;
using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Bumptech.Glide.Integration.RecyclerView;
using Bumptech.Glide.Util;
using QuickDate.Activities.SettingsUser.Adapters;
using QuickDate.Helpers.Ads;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDateClient.Classes.Global;
using QuickDateClient.Classes.Users;
using QuickDateClient.Requests;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace QuickDate.Activities.SettingsUser.General
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class BlockedUsersActivity : AppCompatActivity, MaterialDialog.ISingleButtonCallback
    {
        #region Variables Basic

        public Toolbar Toolbar;
        public RecyclerView UserRecyclerView;
        public ViewStub EmptyStateLayout;
        public LinearLayoutManager BlockedLayoutManager;
        public SwipeRefreshLayout RefreshLayout;
        public BlockedUsersAdapter BlockedAdapter;
        public View Inflated;
        public int Position;
        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                // Create your application here
                SetContentView(Resource.Layout.BlockedUsersLayout);

                //Get Value And Set Toolbar
                InitComponent();
                InitToolbar();
                SetRecyclerViewAdapters();

                AdsGoogle.Ad_RewardedVideo(this);
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
         
        #region Menu

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        #endregion

        #region Functions

        public void InitComponent()
        {
            try
            {
                RefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
                UserRecyclerView = FindViewById<RecyclerView>(Resource.Id.UsersRecylerview);
                EmptyStateLayout = FindViewById<ViewStub>(Resource.Id.viewStub);
               RefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight, Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight, Android.Resource.Color.HoloRedLight);
               RefreshLayout.Refreshing = true;
                RefreshLayout.Enabled = true;
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
                    Toolbar.Title = GetString(Resource.String.Lbl_BlockedUsers);
                    Toolbar.SetTitleTextColor(Color.Black);
                    SetSupportActionBar(Toolbar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                }
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
                BlockedAdapter = new BlockedUsersAdapter(this);           

                BlockedLayoutManager = new LinearLayoutManager(this);
                UserRecyclerView.SetLayoutManager(BlockedLayoutManager);
                UserRecyclerView.SetItemViewCacheSize(20);
                UserRecyclerView.HasFixedSize = true;
                UserRecyclerView.GetLayoutManager().ItemPrefetchEnabled = true;
                var sizeProvider = new FixedPreloadSizeProvider(10, 10);
                var preLoader = new RecyclerViewPreloader<Block>(this, BlockedAdapter, sizeProvider, 10);
                UserRecyclerView.AddOnScrollListener(preLoader);
                UserRecyclerView.SetAdapter(BlockedAdapter);

                UserRecyclerView.Visibility = ViewStates.Visible;
                EmptyStateLayout.Visibility = ViewStates.Gone;

                GetBlockedUsers();
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
                    BlockedAdapter.OnItemClick += BlockedAdapterOnItemClick;
                    RefreshLayout.Refresh += RefreshLayoutOnRefresh;
                }
                else
                {
                    BlockedAdapter.OnItemClick -= BlockedAdapterOnItemClick;
                    RefreshLayout.Refresh -= RefreshLayoutOnRefresh;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Events

        //Refresh
        private void RefreshLayoutOnRefresh(object sender, EventArgs e)
        {
            try
            {
                BlockedAdapter.BlockedUsersList.Clear();
                BlockedAdapter.NotifyDataSetChanged();

                GetBlockedUsers();

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        private void BlockedAdapterOnItemClick(object sender, BlockedUsersAdapterClickEventArgs e)
        {
            try
            {
                Position = e.Position;
                var dialog = new MaterialDialog.Builder(this);
                dialog.Title(Resource.String.Lbl_Warning);
                dialog.Content(GetText(Resource.String.Lbl_DoYouWantUnblock));
                dialog.PositiveText(GetText(Resource.String.Lbl_Yes)).OnPositive(this);
                dialog.NegativeText(GetText(Resource.String.Lbl_No)).OnNegative(this);
                dialog.AlwaysCallSingleChoiceCallback();
                dialog.Build().Show();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        public async void GetBlockedUsers()
        {
            try
            {
                if (!IMethods.CheckConnectivity())
                {
                    RefreshLayout.Refreshing = false;

                    Inflated = EmptyStateLayout.Inflate();
                    EmptyStateInflater x = new EmptyStateInflater();
                    x.InflateLayout(Inflated, EmptyStateInflater.Type.NoConnection);
                    if (!x.EmptyStateButton.HasOnClickListeners)
                    {
                        x.EmptyStateButton.Click += null;
                        x.EmptyStateButton.Click += EmptyStateButtonOnClick;
                    }

                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }
                else
                {
                    var (apiStatus, respond) = await RequestsAsync.Users.ProfileAsync(UserDetails.UserId.ToString(), "blocks");
                    if (apiStatus == 200)
                    {
                        if (respond is ProfileObject result)
                        {
                            if (result.data.Blocks?.Count > 0)
                            {
                                BlockedAdapter.BlockedUsersList = new ObservableCollection<Block>(result.data.Blocks);
                                BlockedAdapter.NotifyDataSetChanged();
                            }
                        }
                    }
                    else if (apiStatus == 400)
                    { 
                        if (respond is ErrorObject error)
                        {
                            var errorText = error.ErrorData.ErrorText;
                              
                            if (errorText.Contains("Permission Denied"))
                                ApiRequest.Logout(this);
                        }
                    }
                    else if (apiStatus == 404)
                    {
                        var error = respond.ToString();
                        //Toast.MakeText(this, error, ToastLength.Short).Show();
                    }
                     
                    if (BlockedAdapter.BlockedUsersList.Count > 0)
                    {
                        EmptyStateLayout.Visibility = ViewStates.Gone;
                        UserRecyclerView.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        UserRecyclerView.Visibility = ViewStates.Gone;

                        if (Inflated == null)
                            Inflated = EmptyStateLayout.Inflate();

                        EmptyStateInflater x = new EmptyStateInflater();
                        x.InflateLayout(Inflated, EmptyStateInflater.Type.NoBlock);
                        if (x.EmptyStateButton.HasOnClickListeners)
                        {
                            x.EmptyStateButton.Click += null;
                        }
                        EmptyStateLayout.Visibility = ViewStates.Visible;
                    }

                    RefreshLayout.Refreshing = false;
                } 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception); 
            }
        }

        private void EmptyStateButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                GetBlockedUsers();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public async void OnClick(MaterialDialog p0, DialogAction p1)
        {
            try
            {
                if (p1 == DialogAction.Positive)
                {
                    if (IMethods.CheckConnectivity())
                    {
                        var itemUser = BlockedAdapter.GetItem(Position);
                        if (itemUser != null)
                        {
                            BlockedAdapter.Remove(itemUser);

                            var (apiStatus, respond) = await RequestsAsync.Users.BlockAsync(itemUser.Id.ToString()).ConfigureAwait(false);
                            Toast.MakeText(this, GetText(Resource.String.Lbl_Unblock_successfully), ToastLength.Short).Show();
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                    }
                     
                    if (BlockedAdapter.BlockedUsersList.Count == 0)
                    { 
                        UserRecyclerView.Visibility = ViewStates.Gone;

                        if (Inflated == null)
                            Inflated = EmptyStateLayout.Inflate();

                        EmptyStateInflater x = new EmptyStateInflater();
                        x.InflateLayout(Inflated, EmptyStateInflater.Type.NoBlock);
                        if (x.EmptyStateButton.HasOnClickListeners)
                        {
                            x.EmptyStateButton.Click += null;
                        }
                        EmptyStateLayout.Visibility = ViewStates.Visible;
                    }

                }
                else if (p1 == DialogAction.Negative)
                {
                    p0.Dismiss();
                } 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}