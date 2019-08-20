using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Android;
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
using Newtonsoft.Json;
using QuickDate.Activities.Chat.Adapters;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDate.SQLite;
using QuickDateClient.Classes.Chat;
using QuickDateClient.Classes.Global;
using QuickDateClient.Requests;
using ActionMode = Android.Support.V7.View.ActionMode;
using Object = Java.Lang.Object;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace QuickDate.Activities.Chat
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, LaunchMode = LaunchMode.SingleInstance)]
    public class LastChatActivity : AppCompatActivity, IOnClickListenerSelected
    {
        #region Variables Basic

        public RecyclerView LastMessageRecyclerView;
        public LinearLayoutManager MLayoutManager;
        public static LastChatAdapter MAdapter;
        public SwipeRefreshLayout SwipeRefreshLayout;
        public static Timer Timer;
        public string TimerWork = "Working";

        public string UserId = "";

        public string LastChatId = "";
        public XamarinRecyclerViewOnScrollListener MainScrollEvent;

        public ActionModeCallback ModeCallback;
        public static ActionMode ActionMode;
        public static Toolbar ToolBar;
        public ViewStub EmptyStateLayout;
        public View Inflated;

        #endregion

        #region General
         
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                // Create your application here
                IMethods.IApp.FullScreenApp(this);

                SetContentView(Resource.Layout.LastChatLayout);

                InitComponent();
                InitToolbar();
                SetRecyclerViewAdapters();

                GetLastChatLocal();
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

                if (MAdapter?.UserList?.Count > 0)
                {
                    LastMessageRecyclerView.Visibility = ViewStates.Visible;
                    EmptyStateLayout.Visibility = ViewStates.Gone;
                }

                if (Timer != null)
                {
                    Timer.Enabled = true;
                    Timer.Start();
                }
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

                if (Timer != null)
                {
                    Timer.Enabled = false;
                    Timer.Stop();
                }
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
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public override void OnLowMemory()
        {
            try
            {
                GC.Collect(GC.MaxGeneration);
                base.OnLowMemory();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        protected override void OnDestroy()
        {
            try
            {
                if (Timer != null)
                {
                    Timer.Enabled = false;
                    Timer.Stop();
                    Timer = null;
                }

                MAdapter?.UserList.Clear();
                MAdapter?.NotifyDataSetChanged();
                 
                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region Functions

        private void InitComponent()
        {
            try
            {
                LastMessageRecyclerView = (RecyclerView)FindViewById(Resource.Id.lastmessagesrecyler);
                EmptyStateLayout = FindViewById<ViewStub>(Resource.Id.viewStub);

                SwipeRefreshLayout = (SwipeRefreshLayout)FindViewById(Resource.Id.lastmessages_swipeRefreshLayout);
                SwipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight, Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight, Android.Resource.Color.HoloRedLight);

                LastMessageRecyclerView.Visibility = ViewStates.Visible;
                EmptyStateLayout.Visibility = ViewStates.Gone;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void InitToolbar()
        {
            try
            {
                ToolBar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (ToolBar != null)
                {
                    ToolBar.Title = GetText(Resource.String.Lbl_Chats);
                    ToolBar.SetTitleTextColor(Color.Black);
                    SetSupportActionBar(ToolBar);
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
                //Set Adapter 
                MAdapter = new LastChatAdapter(this);
                MAdapter.UserList = new ObservableCollection<GetConversationListObject.Data>();
                MAdapter.SetOnClickListener(this);

                MLayoutManager = new LinearLayoutManager(this);
                LastMessageRecyclerView.SetLayoutManager(MLayoutManager);
                LastMessageRecyclerView.SetItemViewCacheSize(20);
                LastMessageRecyclerView.HasFixedSize = true;
                LastMessageRecyclerView.GetLayoutManager().ItemPrefetchEnabled = true; 
                var sizeProvider = new FixedPreloadSizeProvider(10, 10);
                var preLoader = new RecyclerViewPreloader<GetConversationListObject.Data>(this, MAdapter, sizeProvider, 10);
                LastMessageRecyclerView.AddOnScrollListener(preLoader); 
                LastMessageRecyclerView.SetAdapter(MAdapter);


                ModeCallback = new ActionModeCallback(this);
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
                    SwipeRefreshLayout.Refresh += SwipeRefreshLayoutOnRefresh;
                }
                else
                {
                    SwipeRefreshLayout.Refresh -= SwipeRefreshLayoutOnRefresh;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Get LastChat

        /// <summary>
        /// Get Last Chat From Database and Run Timer 
        /// </summary>
        public async void GetLastChatLocal()
        {
            try
            {
                SqLiteDatabase dbDatabase = new SqLiteDatabase();
                ListUtils.ChatList =  new ObservableCollection<GetConversationListObject.Data>();
                var list = dbDatabase.GetAllLastChat();
                if (list.Count > 0)
                {
                    ListUtils.ChatList = new ObservableCollection<GetConversationListObject.Data>(list);
                    MAdapter.UserList = ListUtils.ChatList;
                }
                else
                {
                    SwipeRefreshLayout.Refreshing = true;
                    SwipeRefreshLayout.Enabled = true;
                }

                dbDatabase.Dispose();

                await GetLastChat_Api();

                // Run timer 
                Timer = new Timer { Interval = AppSettings.RefreshChatActivitiesSeconds, Enabled = true };
                Timer.Elapsed += TimerOnElapsed;
                Timer.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await GetLastChat_Api().ConfigureAwait(false);
            }
        }

        //Get Last Chat Api
        public async Task GetLastChat_Api(string offset = "")
        {
            try
            {
                if (!IMethods.CheckConnectivity())
                {
                    SwipeRefreshLayout.Refreshing = false;
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short);
                }
                else
                {
                    TimerWork = "Stop";

                    int countList = MAdapter.UserList.Count;
                    var (apiStatus, respond) = await RequestsAsync.Chat.GetConversationListAsync("15", offset);
                    if (apiStatus == 200)
                    {
                        if (respond is GetConversationListObject result)
                        {
                            if (result.data.Count <= 0)
                                SwipeRefreshLayout.Refreshing = false;

                            if (MAdapter.UserList.Count > 0)
                            {
                                foreach (var user in result.data)
                                { 
                                    var checkUser = MAdapter.UserList.FirstOrDefault(a => a.User.Id == user.User.Id);
                                    if (checkUser != null)
                                    {
                                        int index = MAdapter.UserList.IndexOf(checkUser);

                                        //checkUser.Id = user.Id;
                                        if (checkUser.Owner != user.Owner) checkUser.Owner = user.Owner;
                                        if (checkUser.Time != user.Time) checkUser.Time = user.Time;
                                        if (checkUser.Seen != user.Seen) checkUser.Seen = user.Seen;
                                        if (checkUser.Time != user.Time) checkUser.Time = user.Time;
                                        if (checkUser.CreatedAt != user.CreatedAt) checkUser.CreatedAt = user.CreatedAt;
                                        if (checkUser.NewMessages != user.NewMessages) checkUser.NewMessages = user.NewMessages;
                                        if (checkUser.User != user.User) checkUser.User = user.User;

                                        if (checkUser.MessageType != user.MessageType ) continue;
                                        checkUser.MessageType = user.MessageType;
                                          
                                        if (checkUser.Text != user.Text)
                                        {
                                            checkUser.Text = user.Text;

                                            if (index > -1)
                                            {
                                                RunOnUiThread(() =>
                                                {
                                                    MAdapter.UserList.Move(index, 0);
                                                    MAdapter.NotifyItemMoved(index, 0);
                                                });
                                            }
                                        }

                                        if (checkUser.Media != user.Media)
                                        {
                                            checkUser.Media = user.Media;

                                            if (index > -1)
                                            {
                                                RunOnUiThread(() =>
                                                {
                                                    MAdapter.UserList.Move(index, 0);
                                                    MAdapter.NotifyItemMoved(index, 0);
                                                });
                                            }
                                        }

                                        if (checkUser.Sticker != user.Sticker)
                                        {
                                            checkUser.Sticker = user.Sticker;

                                            if (index > -1)
                                            {
                                                RunOnUiThread(() =>
                                                {
                                                    MAdapter.UserList.Move(index, 0);
                                                    MAdapter.NotifyItemMoved(index, 0); 
                                                });
                                            }
                                        }
                                    }
                                    else
                                    {
                                        RunOnUiThread(() =>
                                        {
                                            MAdapter.UserList.Insert(0, user);
                                            MAdapter.NotifyItemInserted(0);

                                            //var dataUser = MAdapter.UserList.IndexOf(MAdapter.UserList.FirstOrDefault(a => a.Id == user.Id));
                                            //if (dataUser > -1)
                                            //    MAdapter.NotifyItemChanged(dataUser);
                                        });
                                    }
                                }
                            }
                            else
                            {
                                MAdapter.UserList = new ObservableCollection<GetConversationListObject.Data>(result.data);
                                MAdapter.NotifyDataSetChanged();
                            }

                            ListUtils.ChatList = MAdapter.UserList;
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
                }

                //Show Empty Page >> 
                //===============================================================
                RunOnUiThread(() =>
                {
                    if (MAdapter.UserList.Count > 0)
                    {
                        LastMessageRecyclerView.Visibility = ViewStates.Visible;
                        EmptyStateLayout.Visibility = ViewStates.Gone;

                        SqLiteDatabase database = new SqLiteDatabase();
                        database.InsertOrReplaceLastChatTable(MAdapter.UserList);
                        database.Dispose();
                    }
                    else
                    {  
                        if (Inflated == null)
                            Inflated = EmptyStateLayout.Inflate();

                        EmptyStateInflater x = new EmptyStateInflater();
                        x.InflateLayout(Inflated, EmptyStateInflater.Type.NoMessage);

                        LastMessageRecyclerView.Visibility = ViewStates.Gone;
                        EmptyStateLayout.Visibility = ViewStates.Visible;
                    }

                    //Set Event Scroll
                    if (MainScrollEvent == null)
                    {
                        var xamarinRecyclerViewOnScrollListener = new XamarinRecyclerViewOnScrollListener(MLayoutManager, SwipeRefreshLayout);
                        MainScrollEvent = xamarinRecyclerViewOnScrollListener;
                        MainScrollEvent.LoadMoreEvent += FragmentOnScroll_OnLoadMoreEvent;
                        LastMessageRecyclerView.AddOnScrollListener(MainScrollEvent);
                        LastMessageRecyclerView.AddOnScrollListener(new ScrollDownDetector());
                    }
                    else
                        MainScrollEvent.IsLoading = false;

                    if (SwipeRefreshLayout.Refreshing)
                        SwipeRefreshLayout.Refreshing = false;
                });

                TimerWork = "Working";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await GetLastChat_Api(offset);
            }
        }

        #endregion

        #region Events

        //Timer 
        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (TimerWork != "Working" || !IMethods.CheckConnectivity()) return;

                GetLastChat_Api().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
        
        //Refresh
        private async void SwipeRefreshLayoutOnRefresh(object sender, EventArgs e)
        {
            try
            {
                if (IMethods.CheckConnectivity())
                {
                    MessagesBoxActivity.MAdapter?.MessageList.Clear();
                    MessagesBoxActivity.MAdapter?.NotifyDataSetChanged();

                    ListUtils.ChatList.Clear();
                    MAdapter?.UserList.Clear();
                    MAdapter?.NotifyDataSetChanged();
                     
                    SqLiteDatabase database = new SqLiteDatabase();
                    database.ClearLastChat();
                    database.ClearAll_Messages();
                    database.Dispose();

                    SwipeRefreshLayout.Refreshing = true;
                    
                    await GetLastChat_Api();
                }
                else
                {
                    Toast.MakeText(this,GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region Permissions

        //Permissions
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            try
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                if (requestCode == 100)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        var item = MAdapter.UserList.FirstOrDefault(a => a.User.Id.ToString() == UserId);
                        if (item != null)
                        {
                            Intent Int = new Intent(this, typeof(MessagesBoxActivity));
                            Int.PutExtra("UserId", item.User.Id.ToString());
                            Int.PutExtra("UserItem", JsonConvert.SerializeObject(item));
                            StartActivity(Int);

                            MAdapter.NotifyItemChanged(MAdapter.UserList.IndexOf(item));

                        }
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long).Show();
                    }
                }
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

        #region Scroll

        //Event Scroll #LastChat
        private void FragmentOnScroll_OnLoadMoreEvent(object sender, EventArgs eventArgs)
        {
            try
            {
                //Code get last id where LoadMore >>
                var item = MAdapter.UserList.LastOrDefault();
                if (item != null) LastChatId = item.Id.ToString();

                if (LastChatId != "")
                    try
                    {
                        //Run Load More Api
                        Task.Run(() => { GetLastChat_Api(LastChatId).ConfigureAwait(false); });
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                    }

                SwipeRefreshLayout.Refreshing = false;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public class XamarinRecyclerViewOnScrollListener : RecyclerView.OnScrollListener
        {
            public delegate void LoadMoreEventHandler(object sender, EventArgs e);

            public bool IsLoading;
            public LinearLayoutManager LayoutManager;
            public SwipeRefreshLayout SwipeRefreshLayout;

            public XamarinRecyclerViewOnScrollListener(LinearLayoutManager layoutManager, SwipeRefreshLayout swipeRefreshLayout)
            {
                try
                {
                    LayoutManager = layoutManager;
                    SwipeRefreshLayout = swipeRefreshLayout;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            public event LoadMoreEventHandler LoadMoreEvent;

            public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
            {
                try
                {
                    base.OnScrolled(recyclerView, dx, dy);

                    var visibleItemCount = recyclerView.ChildCount;
                    var totalItemCount = recyclerView.GetAdapter().ItemCount;

                    var pastVisiblesItems = LayoutManager.FindFirstVisibleItemPosition();
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

        public class ScrollDownDetector : RecyclerView.OnScrollListener
        {
            public Action Action;
            public bool ReadyForAction;
             
            public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
            {
                try
                {
                    base.OnScrolled(recyclerView, dx, dy);

                    if (ReadyForAction && dy > 0)
                    {
                        //The scroll direction is down
                        ReadyForAction = false;
                        Action();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        #endregion

        #region Toolbar & Selected

        public class ActionModeCallback : Object, ActionMode.ICallback
        {
            public LastChatActivity Activity;
            public ActionModeCallback(LastChatActivity activity)
            {
                Activity = activity;
            }

            public bool OnActionItemClicked(ActionMode mode, IMenuItem item)
            {
                int id = item.ItemId;
                if (id == Resource.Id.action_delete)
                {
                    DeleteItems();
                    mode.Finish();
                    return true;
                }
                else if (id == Android.Resource.Id.Home)
                {
                    if (Timer != null)
                    {
                        Timer.Enabled = true;
                        Timer.Start();
                    }

                    MAdapter.ClearSelections();

                    ToolBar.Visibility = ViewStates.Visible;
                    ActionMode.Finish();

                    return true;
                } 
                return false;
            }

            public bool OnCreateActionMode(ActionMode mode, IMenu menu)
            {
                SetSystemBarColor(Activity, AppSettings.MainColor);
                mode.MenuInflater.Inflate(Resource.Menu.menu_delete, menu);
                return true;
            }

            public void SetSystemBarColor(Activity act, string color)
            {
                try
                {
                    if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                    {
                        Window window = act.Window;
                        window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                        window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                        window.SetStatusBarColor(Color.ParseColor(color));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            public void OnDestroyActionMode(ActionMode mode)
            {
                try
                {
                    MAdapter.ClearSelections();
                    ActionMode.Finish();
                    ActionMode = null;

                    SetSystemBarColor(Activity, AppSettings.MainColor);

                    if (Timer != null)
                    {
                        Timer.Enabled = true;
                        Timer.Start();
                    }

                    ToolBar.Visibility = ViewStates.Visible; 
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            public bool OnPrepareActionMode(ActionMode mode, IMenu menu)
            {
                return false;
            }

            //Delete Chat 
            private void DeleteItems()
            {
                try
                {
                    if (Timer != null)
                    {
                        Timer.Enabled = true;
                        Timer.Start();
                    }

                    if (ToolBar.Visibility != ViewStates.Visible)
                        ToolBar.Visibility = ViewStates.Visible;

                    if (IMethods.CheckConnectivity())
                    {
                        List<int> selectedItemPositions = MAdapter.GetSelectedItems();
                        for (int i = selectedItemPositions.Count - 1; i >= 0; i--)
                        {
                            var datItem = MAdapter.GetItem(selectedItemPositions[i]);
                            if (datItem != null)
                            {
                                SqLiteDatabase dbDatabase = new SqLiteDatabase();
                                dbDatabase.DeleteUserLastChat(datItem.User.Id.ToString());
                                dbDatabase.DeleteAllMessagesUser(UserDetails.UserId.ToString(), datItem.User.Id.ToString());
                                dbDatabase.Dispose();

                                var index = MAdapter.UserList.IndexOf(MAdapter.UserList.FirstOrDefault(a => a.User.Id == datItem.User.Id));
                                if (index != -1)
                                {
                                    MAdapter.UserList.Remove(datItem);
                                    MAdapter.NotifyItemRemoved(index);
                                }
                                 
                                MAdapter.RemoveData(); 

                                //Send Api Delete 
                                RequestsAsync.Chat.DeleteMessagesAsync(datItem.User.Id.ToString()).ConfigureAwait(false);
                            }
                        } 
                    }
                    else
                    {
                        Toast.MakeText(Activity, Activity.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        //Event 
        public void ItemClick(View view, GetConversationListObject.Data obj, int pos)
        {
            try
            {
                if (MAdapter.GetSelectedItemCount() > 0) // Add Select  New Item 
                {
                    EnableActionMode(pos);
                }
                else
                {
                    if (Timer != null)
                    {
                        Timer.Enabled = true;
                        Timer.Start();
                    }

                    if (ToolBar.Visibility != ViewStates.Visible)
                        ToolBar.Visibility = ViewStates.Visible;

                    // read the item which removes bold from the row >> event click open ChatBox by user id
                    var item = MAdapter.GetItem(pos);
                    if (item != null)
                    {
                        UserId = item.User.Id.ToString(); 
                        item.NewMessages = 0; 
                        Intent Int = new Intent(Application.Context, typeof(MessagesBoxActivity));
                        Int.PutExtra("UserId", item.User.Id.ToString());
                        Int.PutExtra("TypeChat", "LastChat");
                        Int.PutExtra("UserItem", JsonConvert.SerializeObject(item.User));

                        // Check if we're running on Android 5.0 or higher
                        if ((int)Build.VERSION.SdkInt < 23)
                        {
                            StartActivity(Int);
                            MAdapter.NotifyItemChanged(pos);
                        }
                        else
                        {
                            //Check to see if any permission in our group is available, if one, then all are
                            if (CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted &&
                                CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Granted)
                            {
                                StartActivity(Int);
                                MAdapter.NotifyItemChanged(pos);
                            }
                            else
                                new PermissionsController(this).RequestPermission(100);
                        } 
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void ItemLongClick(View view, GetConversationListObject.Data obj, int pos)
        {
            EnableActionMode(pos);
        }

        private void EnableActionMode(int position)
        {
            if (ActionMode == null)
            {
                ActionMode = StartSupportActionMode(ModeCallback);
            }
            ToggleSelection(position);
        }

        private void ToggleSelection(int position)
        {
            try
            {
                MAdapter.ToggleSelection(position);
                int count = MAdapter.GetSelectedItemCount();

                if (count == 0)
                {
                    if (Timer != null)
                    {
                        Timer.Enabled = true;
                        Timer.Start();
                    }

                    ToolBar.Visibility = ViewStates.Visible;
                    ActionMode.Finish();
                }
                else
                {
                    if (Timer != null)
                    {
                        Timer.Enabled = false;
                        Timer.Stop();
                    }

                    ToolBar.Visibility = ViewStates.Gone;
                    ActionMode.SetTitle(count);
                    ActionMode.Invalidate();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion
         
        public override void OnBackPressed()
        {
            try
            { 
                int count = MAdapter.GetSelectedItemCount(); 
                if (count == 0)
                {
                    base.OnBackPressed();
                }
                else
                {
                    if (Timer != null)
                    {
                        Timer.Enabled = true;
                        Timer.Start();
                    }

                    ToolBar.Visibility = ViewStates.Visible;
                    ActionMode.Finish();
                } 
            }
            catch (Exception e)
            {
                Console.WriteLine(e); 
            } 
        }
    }
}