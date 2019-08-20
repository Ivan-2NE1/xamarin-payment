using System;
using System.Collections.ObjectModel;
using System.Linq;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using QuickDate.Activities.InviteFriends.Adapters;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Utils;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace QuickDate.Activities.InviteFriends
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class InviteContactActivity : AppCompatActivity
    {
        #region Variables Basic

        public Toolbar Toolbar;
        public RecyclerView UserRecyclerView;
        public LinearLayoutManager BlockedLayoutManager;
        public InviteContactAdapter ContactAdapter;
        public string InviteSmsText = "";
        public IMethods.PhoneContactManager.UserContact Contact;
       
        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);

                // Create your application here
                SetContentView(Resource.Layout.BlockedUsersLayout);
                 
                Contact = new IMethods.PhoneContactManager.UserContact();

                //Get Value And Set Toolbar
                InitComponent();
                InitToolbar();
                SetRecyclerViewAdapters();  
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
                UserRecyclerView = FindViewById<RecyclerView>(Resource.Id.UsersRecylerview);
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
                    Toolbar.Title = GetText(Resource.String.Lbl_ContactsPhone);
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
                ContactAdapter = new InviteContactAdapter(this);

                BlockedLayoutManager = new LinearLayoutManager(this);
                UserRecyclerView.SetLayoutManager(BlockedLayoutManager);
                UserRecyclerView.SetItemViewCacheSize(20);
                UserRecyclerView.HasFixedSize = true;
                UserRecyclerView.GetLayoutManager().ItemPrefetchEnabled = true;
                UserRecyclerView.SetAdapter(ContactAdapter);

                GetAllContacts();
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
                    ContactAdapter.OnItemClick += ContactAdapterOnItemClick;
                   
                }
                else
                {
                    ContactAdapter.OnItemClick -= ContactAdapterOnItemClick;
                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Events

        private void ContactAdapterOnItemClick(object sender, InviteContactAdapterClickEventArgs e)
        {
            var position = e.Position;
            if (position >= 0)
            {
                var item = ContactAdapter.GetItem(position);

                Contact = item;
                if (item != null)
                {
                    // Check if we're running on Android 5.0 or higher
                    if ((int)Build.VERSION.SdkInt < 23)
                    {
                        IMethods.IApp.SendSMS(this, item.PhoneNumber, InviteSmsText);
                    }
                    else
                    {
                        //Check to see if any permission in our group is available, if one, then all are
                        if (CheckSelfPermission(Manifest.Permission.SendSms) == Permission.Granted)
                            IMethods.IApp.SendSMS(this, item.PhoneNumber, InviteSmsText);
                        else
                            new PermissionsController(this).RequestPermission(104);
                    }
                }
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

                if (requestCode == 104)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        IMethods.IApp.SendSMS(this, Contact.PhoneNumber, InviteSmsText);
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

        public void GetAllContacts()
        {
            try
            {
                var listContacts =new ObservableCollection<IMethods.PhoneContactManager.UserContact>(IMethods.PhoneContactManager.GetAllContacts());
                var orderByDate = listContacts.OrderBy(a => a.UserDisplayName);

                //Set Adapter
                ContactAdapter.UsersPhoneContacts = new ObservableCollection<IMethods.PhoneContactManager.UserContact>(orderByDate);
                ContactAdapter.NotifyDataSetChanged();
                 
                InviteSmsText = GetText(Resource.String.Lbl_InviteSMSText_1) + " " + AppSettings.ApplicationName + " " + GetText(Resource.String.Lbl_InviteSMSText_2);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}