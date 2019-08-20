using System;
using System.Collections.Generic;
using System.Linq;
using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using Java.Lang;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Fonts;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDate.SQLite;
using QuickDateClient.Classes.Global;
using QuickDateClient.Classes.Users;
using QuickDateClient.Requests;
using Exception = System.Exception;

namespace QuickDate.Activities.MyProfile
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class PersonalityInfoEditActivity : AppCompatActivity, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback
    {
        #region Variables Basic

        public TextView BackIcon, CharacterIcon, ChildrenIcon, FriendsIcon, PetsIcon;
        public EditText EdtCharacter, EdtChildren, EdtFriends, EdtPets;
        public Button BtnSave;
        public string TypeDialog;
        public int IdCharacter, IdChildren, IdFriends, IdPets;
        public AdView MAdView;

        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);

                // Create your application here
                SetContentView(Resource.Layout.ButtomSheetPersonalityInfoEdit);

                //Get Value And Set Toolbar
                InitComponent();
                InitAdView();

                GetMyInfoData();
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
                MAdView?.Resume();
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
                MAdView?.Pause();
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

        #region Functions

        public void InitComponent()
        {
            try
            {
                BackIcon = FindViewById<TextView>(Resource.Id.IconBack);

                CharacterIcon = FindViewById<TextView>(Resource.Id.IconCharacter);
                EdtCharacter = FindViewById<EditText>(Resource.Id.CharacterEditText);

                ChildrenIcon = FindViewById<TextView>(Resource.Id.IconChildren);
                EdtChildren = FindViewById<EditText>(Resource.Id.ChildrenEditText);

                FriendsIcon = FindViewById<TextView>(Resource.Id.IconFriends);
                EdtFriends = FindViewById<EditText>(Resource.Id.FriendsEditText);

                PetsIcon = FindViewById<TextView>(Resource.Id.IconPets);
                EdtPets = FindViewById<EditText>(Resource.Id.PetsEditText);
                 
                BtnSave = FindViewById<Button>(Resource.Id.ApplyButton);

                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, BackIcon, IonIconsFonts.ChevronLeft);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, CharacterIcon, FontAwesomeIcon.YinYang);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, ChildrenIcon, FontAwesomeIcon.Baby);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, FriendsIcon, FontAwesomeIcon.UserFriends);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, PetsIcon, FontAwesomeIcon.Cat);

                EdtCharacter.SetFocusable(ViewFocusability.NotFocusable);
                EdtChildren.SetFocusable(ViewFocusability.NotFocusable);
                EdtFriends.SetFocusable(ViewFocusability.NotFocusable);
                EdtPets.SetFocusable(ViewFocusability.NotFocusable);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void InitAdView()
        {
            try
            {
                MAdView = FindViewById<AdView>(Resource.Id.adView);
                var dataUser = ListUtils.MyUserInfo.FirstOrDefault(a => a.Id == UserDetails.UserId);
                if (dataUser?.IsPro == "0")
                {
                    if (AppSettings.ShowAdmobBanner)
                    {
                        MAdView.Visibility = ViewStates.Visible;
                        var adRequest = new AdRequest.Builder().Build();
                        MAdView.LoadAd(adRequest);
                    }
                    else
                    {
                        MAdView.Pause();
                        MAdView.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    MAdView.Pause();
                    MAdView.Visibility = ViewStates.Gone;
                }
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
                    BackIcon.Click += BackIconOnClick;
                    BtnSave.Click += BtnSaveOnClick;
                    EdtCharacter.Click += EdtCharacterOnClick;
                    EdtChildren.Click += EdtChildrenOnClick;
                    EdtFriends.Click += EdtFriendsOnClick;
                    EdtPets.Click += EdtPetsOnClick;
                }
                else
                {
                    BackIcon.Click -= BackIconOnClick;
                    BtnSave.Click -= BtnSaveOnClick;
                    EdtCharacter.Click -= EdtCharacterOnClick;
                    EdtChildren.Click -= EdtChildrenOnClick;
                    EdtFriends.Click -= EdtFriendsOnClick;
                    EdtPets.Click -= EdtPetsOnClick;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Events

        //Pets
        private void EdtPetsOnClick(object sender, EventArgs e)
        {
            try
            {
                TypeDialog = "Pets";
                string[] petsArray = Application.Context.Resources.GetStringArray(Resource.Array.PetsArray);

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(this);

                foreach (var item in petsArray)
                    arrayAdapter.Add(item);

                dialogList.Title(GetText(Resource.String.Lbl_Pets));
                dialogList.Items(arrayAdapter);
                dialogList.PositiveText(GetText(Resource.String.Lbl_Close)).OnPositive(this);
                dialogList.AlwaysCallSingleChoiceCallback();
                dialogList.ItemsCallback(this).Build().Show();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Friends
        private void EdtFriendsOnClick(object sender, EventArgs e)
        {
            try
            {
                TypeDialog = "Friends";
                string[] friendsArray = Application.Context.Resources.GetStringArray(Resource.Array.FriendsArray);

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(this);

                foreach (var item in friendsArray)
                    arrayAdapter.Add(item);

                dialogList.Title(GetText(Resource.String.Lbl_Friends));
                dialogList.Items(arrayAdapter);
                dialogList.PositiveText(GetText(Resource.String.Lbl_Close)).OnPositive(this);
                dialogList.AlwaysCallSingleChoiceCallback();
                dialogList.ItemsCallback(this).Build().Show();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Children
        private void EdtChildrenOnClick(object sender, EventArgs e)
        {
            try
            {
                TypeDialog = "Children";
                string[] childrenArray = Application.Context.Resources.GetStringArray(Resource.Array.ChildrenArray);

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(this);

                foreach (var item in childrenArray)
                    arrayAdapter.Add(item);

                dialogList.Title(GetText(Resource.String.Lbl_Children));
                dialogList.Items(arrayAdapter);
                dialogList.PositiveText(GetText(Resource.String.Lbl_Close)).OnPositive(this);
                dialogList.AlwaysCallSingleChoiceCallback();
                dialogList.ItemsCallback(this).Build().Show();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Character
        private void EdtCharacterOnClick(object sender, EventArgs e)
        {
            try
            {
                TypeDialog = "Character";
                string[] characterArray = Application.Context.Resources.GetStringArray(Resource.Array.CharacterArray);

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(this);

                foreach (var item in characterArray)
                    arrayAdapter.Add(item);

                dialogList.Title(GetText(Resource.String.Lbl_Character));
                dialogList.Items(arrayAdapter);
                dialogList.PositiveText(GetText(Resource.String.Lbl_Close)).OnPositive(this);
                dialogList.AlwaysCallSingleChoiceCallback();
                dialogList.ItemsCallback(this).Build().Show();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Click save data and sent api
        private async void BtnSaveOnClick(object sender, EventArgs e)
        {
            try
            {
                if (IMethods.CheckConnectivity())
                {
                    //Show a progress
                    AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading));

                    var dictionary = new Dictionary<string, string>
                    {
                        {"character", IdCharacter.ToString()},
                        {"children", IdChildren.ToString()},
                        {"friends", IdFriends.ToString()},
                        {"pets",IdPets.ToString()},
                    };

                    (int apiStatus, var respond) = await RequestsAsync.Users.UpdateProfileAsync(dictionary);
                    if (apiStatus == 200)
                    {
                        if (respond is UpdateProfileObject result)
                        {
                            var local = ListUtils.MyUserInfo.FirstOrDefault(a => a.Id == UserDetails.UserId);
                            if (local != null)
                            {
                                local.Character = IdCharacter;
                                local.Children = IdChildren;
                                local.Friends = IdFriends;
                                local.Pets = IdPets;

                                SqLiteDatabase database = new SqLiteDatabase();
                                database.InsertOrUpdate_DataMyInfo(local);
                                database.Dispose(); 
                            }

                            Toast.MakeText(this, GetText(Resource.String.Lbl_Done), ToastLength.Short).Show();
                            AndHUD.Shared.Dismiss(this);
                             
                            Intent resultIntent = new Intent();
                            SetResult(Result.Ok, resultIntent);
                            Finish();
                        }
                    }
                    else if (apiStatus == 400)
                    {
                        if (respond is ErrorObject error)
                        {
                            var errorText = error.ErrorData.ErrorText;

                            AndHUD.Shared.ShowError(this, errorText, MaskType.Clear, TimeSpan.FromSeconds(2));

                            if (errorText.Contains("Permission Denied"))
                                ApiRequest.Logout(this);
                        }
                    }
                    else if (apiStatus == 404)
                    {
                        var error = respond.ToString();
                        //Toast.MakeText(this, error, ToastLength.Short).Show();
                    }
                     
                    AndHUD.Shared.Dismiss(this);
                }
                else
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                AndHUD.Shared.Dismiss(this);
            }
        }
         
        private void BackIconOnClick(object sender, EventArgs e)
        {
            try
            {
                Intent resultIntent = new Intent();
                SetResult(Result.Canceled, resultIntent);
                Finish();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        public void GetMyInfoData()
        {
            try
            {
                if (ListUtils.MyUserInfo.Count == 0)
                {
                    var sqlEntity = new SqLiteDatabase();
                    sqlEntity.GetDataMyInfo();
                    sqlEntity.Dispose();
                }

                var dataUser = ListUtils.MyUserInfo.FirstOrDefault(a => a.Id == UserDetails.UserId);
                if (dataUser != null)
                {
                    string character = QuickDateTools.GetCharacter(Convert.ToInt32(dataUser.Character));
                    if (IMethods.Fun_String.StringNullRemover(character) != "-----")
                    {
                        IdCharacter = Convert.ToInt32(dataUser.Character);
                        EdtCharacter.Text = character;
                    }

                    string children = QuickDateTools.GetChildren(Convert.ToInt32(dataUser.Children));
                    if (IMethods.Fun_String.StringNullRemover(children) != "-----")
                    {
                        IdChildren = Convert.ToInt32(dataUser.Children);
                        EdtChildren.Text = children;
                    }

                    string friends = QuickDateTools.GetFriends(Convert.ToInt32(dataUser.Friends));
                    if (IMethods.Fun_String.StringNullRemover(friends) != "-----")
                    {
                        IdFriends = Convert.ToInt32(dataUser.Friends);
                        EdtFriends.Text = friends;
                    }

                    string pets = QuickDateTools.GetPets(Convert.ToInt32(dataUser.Pets));
                    if (IMethods.Fun_String.StringNullRemover(pets) != "-----")
                    {
                        IdPets = Convert.ToInt32(dataUser.Pets);
                        EdtPets.Text = pets;
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #region MaterialDialog

        public void OnSelection(MaterialDialog p0, View p1, int itemId, ICharSequence itemString)
        {
            try
            {
                if (TypeDialog == "Character")
                {
                    IdCharacter = itemId + 1;
                    EdtCharacter.Text = itemString.ToString();
                }
                else if (TypeDialog == "Children")
                {
                    IdChildren = itemId + 1;
                    EdtChildren.Text = itemString.ToString();
                }
                else if (TypeDialog == "Friends")
                {
                    IdFriends = itemId + 1;
                    EdtFriends.Text = itemString.ToString();
                }
                else if (TypeDialog == "Pets")
                {
                    IdPets = itemId + 1;
                    EdtPets.Text = itemString.ToString();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnClick(MaterialDialog p0, DialogAction p1)
        {
            try
            {
                if (p1 == DialogAction.Positive)
                {
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

        #endregion

    }
}