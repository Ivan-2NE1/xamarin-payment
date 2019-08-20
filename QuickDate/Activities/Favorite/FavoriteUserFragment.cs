using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using Android.Views;
using Bumptech.Glide.Integration.RecyclerView;
using Bumptech.Glide.Util;
using Newtonsoft.Json;
using QuickDate.Activities.Favorite.Adapters;
using QuickDate.Activities.Tabbes;
using QuickDate.Activities.UserProfile;
using QuickDate.Helpers.Ads;
using QuickDate.Helpers.Utils;
using QuickDate.SQLite;
using QuickDateClient.Classes.Global;
using Fragment = Android.Support.V4.App.Fragment;

namespace QuickDate.Activities.Favorite
{
    public class FavoriteUserFragment : Fragment
    {
        #region Variables Basic

        public FavoriteUsersAdapter FavoriteAdapter;
        public ViewStub EmptyStateLayout;
        public View Inflated;
        public HomeActivity GlobalContext;
        public RecyclerView FavoriteRecyclerView;

        #endregion

        #region General

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            HasOptionsMenu = true;
            // Create your fragment here
            GlobalContext = (HomeActivity)Activity;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                View view = inflater.Inflate(Resource.Layout.FavoriteUsersLayout, container, false);

                InitComponent(view);
                InitToolbar(view);
                SetRecyclerViewAdapters();

                GetFavorite();

                AdsGoogle.Ad_Interstitial(Context);

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
         
        #region Menu

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    GlobalContext.FragmentBottomNavigator.BackStackClickFragment();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        #endregion

        #region Functions

        public void InitComponent(View view)
        {
            try
            {
                FavoriteRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.UsersRecylerview);
                EmptyStateLayout = view.FindViewById<ViewStub>(Resource.Id.viewStub);
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
                Toolbar toolbar = view.FindViewById<Toolbar>(Resource.Id.toolbar);
                GlobalContext.SetToolBar(toolbar,GetString(Resource.String.Lbl_Favorite));
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
                FavoriteAdapter = new FavoriteUsersAdapter(Activity, GlobalContext);
                
                GridLayoutManager layoutManager = new GridLayoutManager(Context, 2);
                FavoriteRecyclerView.SetLayoutManager(layoutManager);
                FavoriteRecyclerView.SetItemViewCacheSize(20);
                FavoriteRecyclerView.HasFixedSize = true;
                FavoriteRecyclerView.GetLayoutManager().ItemPrefetchEnabled = true;
                var sizeProvider = new FixedPreloadSizeProvider(10, 10);
                var preLoader = new RecyclerViewPreloader<UserInfoObject>(Activity, FavoriteAdapter, sizeProvider, 10);
                FavoriteRecyclerView.AddOnScrollListener(preLoader);
                FavoriteRecyclerView.SetAdapter(FavoriteAdapter);

                FavoriteAdapter.OnItemClick += FavoriteAdapterOnItemClick;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        #endregion

        #region Events

        private void FavoriteAdapterOnItemClick(object sender, FavoriteUsersAdapterClickEventArgs e)
        {
            try
            { 
                if (e.Position > -1)
                {
                    var item = FavoriteAdapter.GetItem(e.Position);
                    if (item != null)
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
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        public void GetFavorite()
        {
            try
            {
                if (ListUtils.FavoriteUserList.Count == 0)
                {
                    var sqlEntity = new SqLiteDatabase();
                    ListUtils.FavoriteUserList = sqlEntity.GetDataFavorite();
                    sqlEntity.Dispose();
                }
                else if (ListUtils.FavoriteUserList.Count > 0)
                {
                    FavoriteAdapter.FavoritesList = ListUtils.FavoriteUserList;
                    FavoriteAdapter.NotifyDataSetChanged();
                }
                 
                if (FavoriteAdapter.FavoritesList.Count > 0)
                {
                    EmptyStateLayout.Visibility = ViewStates.Gone; 
                    FavoriteRecyclerView.Visibility = ViewStates.Visible;
                }
                else
                {
                    ShowEmptyState();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void ShowEmptyState()
        {
            try
            {
                FavoriteRecyclerView.Visibility = ViewStates.Gone;

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
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

    }
}