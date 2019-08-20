using System;
using System.Collections.Generic;
using System.Linq;
using AFollestad.MaterialDialogs;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Text;
using Android.Views;
using Android.Widget;
using AT.Markushi.UI;
using Com.Theartofdev.Edmodo.Cropper;
using Java.IO;
using Java.Lang;
using QuickDate.Helpers.CacheLoaders;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDate.SQLite;
using QuickDateClient.Classes.Global;
using QuickDateClient.Classes.Users;
using QuickDateClient.Requests;
using Console = System.Console;
using Exception = System.Exception;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Uri = Android.Net.Uri;

namespace QuickDate.Activities.MyProfile
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class EditProfileActivity : AppCompatActivity, MaterialDialog.ISingleButtonCallback, MaterialDialog.IInputCallback
    {
        #region Variables Basic

        public ImageView ImageView1, ImageView2, ImageView3, ImageView4, ImageView5, ImageView6;
        public ImageView IconAboutEdit, IconProfileInfoEdit, IconInterestEdit, IconLooksEdit, IconPersonalityInfoEdit, IconLifestyleEdit, IconFavoriteEdit; 
        public CircleButton BtnBoostImage1, BtnBoostImage2, BtnBoostImage3, BtnBoostImage4, BtnBoostImage5, BtnBoostImage6;
        public TextView CountPercent , TxtAbout , TxtName, TxtGender, TxtBirthday, TxtLocation, TxtLanguage, TxtRelationship, TxtWork, TxtEducation;
        public TextView TxtInterest, TxtEthnicity, TxtBody, TxtHeight, TxtHair, TxtCharacter, TxtChildren, TxtFriends, TxtPets, TxtLiveWith,TxtCar;
        public TextView TxtReligion, TxtSmoke, TxtDrink, TxtTravel, TxtMusic, TxtDish, TxtSong, TxtHobby, TxtCity, TxtSport, TxtBook, TxtMovie, TxtColor, TxtTvShow;
        public ProgressBar ProgressBar;
        public int NumImage, Count;
        public string TypeDialog = "", IdImage1 = "", IdImage2 = "", IdImage3 = "", IdImage4 = "", IdImage5 = "", IdImage6 = "";

        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);

                // Create your application here
                SetContentView(Resource.Layout.EditMyProfileLayout);

                //Get Value And Set Toolbar
                InitComponent();
                InitToolbar();

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
                ImageView1 = FindViewById<ImageView>(Resource.Id.ImageView1);
                ImageView2 = FindViewById<ImageView>(Resource.Id.ImageView2);
                ImageView3 = FindViewById<ImageView>(Resource.Id.ImageView3);
                ImageView4 = FindViewById<ImageView>(Resource.Id.ImageView4);
                ImageView5 = FindViewById<ImageView>(Resource.Id.ImageView5);
                ImageView6 = FindViewById<ImageView>(Resource.Id.ImageView6);

                BtnBoostImage1 = FindViewById<CircleButton>(Resource.Id.BoostButton1);
                BtnBoostImage2 = FindViewById<CircleButton>(Resource.Id.BoostButton2);
                BtnBoostImage3 = FindViewById<CircleButton>(Resource.Id.BoostButton3);
                BtnBoostImage4 = FindViewById<CircleButton>(Resource.Id.BoostButton4);
                BtnBoostImage5 = FindViewById<CircleButton>(Resource.Id.BoostButton5);
                BtnBoostImage6 = FindViewById<CircleButton>(Resource.Id.BoostButton6);

                CountPercent = FindViewById<TextView>(Resource.Id.countPercent);
                ProgressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);

                IconAboutEdit = FindViewById<ImageView>(Resource.Id.iconAboutEdit);
                TxtAbout = FindViewById<TextView>(Resource.Id.AboutTextview);

                IconProfileInfoEdit = FindViewById<ImageView>(Resource.Id.iconProfileInfoEdit);
                TxtName = FindViewById<TextView>(Resource.Id.NameTextView);
                TxtGender = FindViewById<TextView>(Resource.Id.GenderTextView);
                TxtBirthday = FindViewById<TextView>(Resource.Id.BirthdayTextView);
                TxtLocation = FindViewById<TextView>(Resource.Id.LocationTextView);
                TxtLanguage = FindViewById<TextView>(Resource.Id.preferred_language_valueTextView);
                TxtRelationship = FindViewById<TextView>(Resource.Id.relationship_status_valueTextView);
                TxtWork = FindViewById<TextView>(Resource.Id.work_status_valueTextView);
                TxtEducation = FindViewById<TextView>(Resource.Id.education_level_valueTextView);

                IconInterestEdit = FindViewById<ImageView>(Resource.Id.iconInterestEdit);
                TxtInterest = FindViewById<TextView>(Resource.Id.interestTextview);
                 
                IconLooksEdit = FindViewById<ImageView>(Resource.Id.iconLooksEdit);
                TxtEthnicity = FindViewById<TextView>(Resource.Id.EthnicityText);
                TxtBody = FindViewById<TextView>(Resource.Id.Body_Type_Value);
                TxtHeight = FindViewById<TextView>(Resource.Id.height_value);
                TxtHair = FindViewById<TextView>(Resource.Id.hair_color_value);

                IconPersonalityInfoEdit = FindViewById<ImageView>(Resource.Id.iconPersonalityinfoEdit);
                TxtCharacter = FindViewById<TextView>(Resource.Id.CharacterText);
                TxtChildren = FindViewById<TextView>(Resource.Id.ChildrenText);
                TxtFriends = FindViewById<TextView>(Resource.Id.FriendsText);
                TxtPets = FindViewById<TextView>(Resource.Id.PetsText);

                IconLifestyleEdit = FindViewById<ImageView>(Resource.Id.iconLifestyleEdit);
                TxtLiveWith = FindViewById<TextView>(Resource.Id.livewithText);
                TxtCar = FindViewById<TextView>(Resource.Id.CarText);
                TxtReligion = FindViewById<TextView>(Resource.Id.ReligionText);
                TxtSmoke = FindViewById<TextView>(Resource.Id.SmokeText);
                TxtDrink = FindViewById<TextView>(Resource.Id.DrinkText);
                TxtTravel = FindViewById<TextView>(Resource.Id.TravelText);

                IconFavoriteEdit = FindViewById<ImageView>(Resource.Id.iconFavouritesEdit);
                TxtMusic = FindViewById<TextView>(Resource.Id.MusicGenreText);
                TxtDish = FindViewById<TextView>(Resource.Id.DishTextView);
                TxtSong = FindViewById<TextView>(Resource.Id.SongTextView);
                TxtHobby = FindViewById<TextView>(Resource.Id.HobbyTextView);
                TxtCity = FindViewById<TextView>(Resource.Id.CityTextView);
                TxtSport = FindViewById<TextView>(Resource.Id.SportTextView);
                TxtBook = FindViewById<TextView>(Resource.Id.BookTextView);
                TxtMovie = FindViewById<TextView>(Resource.Id.MovieTextView);
                TxtColor = FindViewById<TextView>(Resource.Id.ColorTextView);
                TxtTvShow = FindViewById<TextView>(Resource.Id.TVShowTextView); 
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
                var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (toolbar != null)
                {
                    toolbar.Title = " ";
                    toolbar.SetTitleTextColor(Color.Black);
                    SetSupportActionBar(toolbar);
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

        private void AddOrRemoveEvent(bool addEvent)
        {
            try
            {
                // true +=  // false -=
                if (addEvent)
                {
                    ImageView1.Click += ImageView1OnClick;
                    ImageView2.Click += ImageView2OnClick;
                    ImageView3.Click += ImageView3OnClick;
                    ImageView4.Click += ImageView4OnClick;
                    ImageView5.Click += ImageView5OnClick;
                    ImageView6.Click += ImageView6OnClick;
                    BtnBoostImage1.Click +=BtnBoostImage1OnClick;
                    BtnBoostImage2.Click +=BtnBoostImage2OnClick;
                    BtnBoostImage3.Click +=BtnBoostImage3OnClick;
                    BtnBoostImage4.Click +=BtnBoostImage4OnClick;
                    BtnBoostImage5.Click +=BtnBoostImage5OnClick;
                    BtnBoostImage6.Click +=BtnBoostImage6OnClick;
                    IconAboutEdit.Click += IconAboutEditOnClick;
                    IconProfileInfoEdit.Click += IconProfileInfoEditOnClick;
                    IconInterestEdit.Click += IconInterestEditOnClick;
                    IconLooksEdit.Click += IconLooksEditOnClick;
                    IconPersonalityInfoEdit.Click += IconPersonalityInfoEditOnClick;
                    IconLifestyleEdit.Click += IconLifestyleEditOnClick;
                    IconFavoriteEdit.Click += IconFavoriteEditOnClick;
                }
                else
                {
                    ImageView1.Click -= ImageView1OnClick;
                    ImageView2.Click -= ImageView2OnClick;
                    ImageView3.Click -= ImageView3OnClick;
                    ImageView4.Click -= ImageView4OnClick;
                    ImageView5.Click -= ImageView5OnClick;
                    ImageView6.Click -= ImageView6OnClick;
                    BtnBoostImage1.Click -= BtnBoostImage1OnClick;
                    BtnBoostImage2.Click -= BtnBoostImage2OnClick;
                    BtnBoostImage3.Click -= BtnBoostImage3OnClick;
                    BtnBoostImage4.Click -= BtnBoostImage4OnClick;
                    BtnBoostImage5.Click -= BtnBoostImage5OnClick;
                    BtnBoostImage6.Click -= BtnBoostImage6OnClick;
                    IconAboutEdit.Click -= IconAboutEditOnClick;
                    IconProfileInfoEdit.Click -= IconProfileInfoEditOnClick;
                    IconInterestEdit.Click -= IconInterestEditOnClick;
                    IconLooksEdit.Click -= IconLooksEditOnClick;
                    IconPersonalityInfoEdit.Click -= IconPersonalityInfoEditOnClick;
                    IconLifestyleEdit.Click -= IconLifestyleEditOnClick;
                    IconFavoriteEdit.Click -= IconFavoriteEditOnClick;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Events

        //#Add Or Change Image 
        private void ImageView6OnClick(object sender, EventArgs e)
        {
            try
            {
                NumImage = 6;
                OpenDialogGallery();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
            
        }

        private void ImageView5OnClick(object sender, EventArgs e)
        {
            try
            {
                NumImage = 5;
                OpenDialogGallery();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
         
        }

        private void ImageView4OnClick(object sender, EventArgs e)
        {
            try
            {
                NumImage = 4;
                OpenDialogGallery();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
           
        }

        private void ImageView3OnClick(object sender, EventArgs e)
        {
            try
            {
                NumImage = 3;
                OpenDialogGallery();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
          
        }

        private void ImageView2OnClick(object sender, EventArgs e)
        {
            try
            {
                NumImage = 2;
                OpenDialogGallery();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
           
        }

        private void ImageView1OnClick(object sender, EventArgs e)
        {
            try
            {
                NumImage = 1;
                OpenDialogGallery();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
           
        }

        //#Edit info
        private void IconFavoriteEditOnClick(object sender, EventArgs e)
        {
            try
            {
                Intent Int = new Intent(this, typeof(FavoriteEditActivity));
                StartActivityForResult(Int, 3040);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void IconLifestyleEditOnClick(object sender, EventArgs e)
        {
            try
            {
                Intent Int = new Intent(this, typeof(LifeStyleEditActivity));
                StartActivityForResult(Int, 3030);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void IconPersonalityInfoEditOnClick(object sender, EventArgs e)
        {
            try
            {
                Intent Int = new Intent(this, typeof(PersonalityInfoEditActivity));
                StartActivityForResult(Int, 3020);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void IconLooksEditOnClick(object sender, EventArgs e)
        {
            try
            {
                Intent Int = new Intent(this, typeof(LooksEditActivity));
                StartActivityForResult(Int, 3010);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void IconInterestEditOnClick(object sender, EventArgs e)
        {
            try
            {
                TypeDialog = "Interest";
                var dialog = new MaterialDialog.Builder(this);
                dialog.Title(GetString(Resource.String.Lbl_Interest));
                dialog.Input(Resource.String.Lbl_EnterTextInterest, 0, false, this);
                dialog.InputType(InputTypes.TextFlagImeMultiLine);
                dialog.PositiveText(GetText(Resource.String.Lbl_Submit)).OnPositive(this);
                dialog.NegativeText(GetText(Resource.String.Lbl_Cancel)).OnNegative(this);
                dialog.AlwaysCallSingleChoiceCallback();
                dialog.Build().Show();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Edit profile info
        private void IconProfileInfoEditOnClick(object sender, EventArgs e)
        {
            try
            {
                Intent Int = new Intent(this,typeof(ProfileInfoEditActivity));
                StartActivityForResult(Int,3000);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //About
        private void IconAboutEditOnClick(object sender, EventArgs e)
        {
            try
            {
                TypeDialog = "About";

                var dialog = new MaterialDialog.Builder(this);
                dialog.Title(Resource.String.Lbl_About);
                dialog.Input(Resource.String.Lbl_AddWordsAbout, 0, false, this);
                dialog.InputType(InputTypes.TextFlagImeMultiLine);
                dialog.PositiveText(GetText(Resource.String.Lbl_Submit)).OnPositive(this);
                dialog.NegativeText(GetText(Resource.String.Lbl_Cancel)).OnNegative(this);
                dialog.AlwaysCallSingleChoiceCallback();
                dialog.Build().Show();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        //#Reset image
        private void BtnBoostImage6OnClick(object sender, EventArgs e)
        {
            try
            {
                GlideImageLoader.LoadImage(this,"ImagePlacholder", ImageView6, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                RequestsAsync.Users.DeleteImageUserAsync(IdImage6).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            } 
        }

        private void BtnBoostImage5OnClick(object sender, EventArgs e)
        {
            try
            {
                GlideImageLoader.LoadImage(this, "ImagePlacholder", ImageView5, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                RequestsAsync.Users.DeleteImageUserAsync(IdImage5).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            } 
        }

        private void BtnBoostImage4OnClick(object sender, EventArgs e)
        {
            try
            {
                GlideImageLoader.LoadImage(this, "ImagePlacholder", ImageView4, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                RequestsAsync.Users.DeleteImageUserAsync(IdImage4).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            } 
        }

        private void BtnBoostImage3OnClick(object sender, EventArgs e)
        {
            try
            {
                GlideImageLoader.LoadImage(this, "ImagePlacholder", ImageView3, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                RequestsAsync.Users.DeleteImageUserAsync(IdImage3).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            } 
        }

        private void BtnBoostImage2OnClick(object sender, EventArgs e)
        {
            try
            {
                GlideImageLoader.LoadImage(this, "ImagePlacholder", ImageView2, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                RequestsAsync.Users.DeleteImageUserAsync(IdImage2).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }  
        }

        private void BtnBoostImage1OnClick(object sender, EventArgs e)
        {
            try
            {
                GlideImageLoader.LoadImage(this, "ImagePlacholder", ImageView1, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                RequestsAsync.Users.DeleteImageUserAsync(IdImage1).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            } 
        }
     
        #endregion

        #region Permissions && Result

        //Result
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);
                 
                if (requestCode == 108 || requestCode == CropImage.CropImageActivityRequestCode)
                {
                    if (IMethods.CheckConnectivity())
                    {
                        var result = CropImage.GetActivityResult(data);

                        if (result.IsSuccessful)
                        {
                            var resultPathImage = result.Uri.Path;
                            if (!string.IsNullOrEmpty(resultPathImage))
                            {
                                switch (NumImage)
                                {
                                    case 1:
                                        GlideImageLoader.LoadImage(this, resultPathImage, ImageView1, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                                        break;
                                    case 2:
                                        GlideImageLoader.LoadImage(this, resultPathImage, ImageView2, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                                        break;
                                    case 3:
                                        GlideImageLoader.LoadImage(this, resultPathImage, ImageView3, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                                        break;
                                    case 4:
                                        GlideImageLoader.LoadImage(this, resultPathImage, ImageView4, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                                        break;
                                    case 5:
                                        GlideImageLoader.LoadImage(this, resultPathImage, ImageView5, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                                        break;
                                    case 6:
                                        GlideImageLoader.LoadImage(this, resultPathImage, ImageView6, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                                        break; 
                                }

                                //sent api 
                                RequestsAsync.Users.UploadImageUserAsync(resultPathImage).ConfigureAwait(false);
                            }
                        }
                    }
                }
                else if (requestCode == 3000 || requestCode == 3010|| requestCode == 3020|| requestCode == 3030|| requestCode == 3040 && resultCode == Result.Ok)
                {
                    GetMyInfoData();
                } 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Permissions
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            try
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
                if (requestCode == 108)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        OpenDialogGallery();
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
                    if (dataUser.Mediafiles?.Count > 0)
                    {
                        for (int i = 0; i < dataUser.Mediafiles.Count; i++)
                        {
                            switch (i)
                            {
                                case 0:
                                    GlideImageLoader.LoadImage(this, dataUser.Mediafiles[i]?.Avater, ImageView1, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                                    IdImage1 = dataUser.Mediafiles[i]?.Id.ToString();
                                    break;
                                case 1:
                                    GlideImageLoader.LoadImage(this, dataUser.Mediafiles[i]?.Avater, ImageView2, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                                    IdImage2 = dataUser.Mediafiles[i]?.Id.ToString();
                                    break;
                                case 2:
                                    GlideImageLoader.LoadImage(this, dataUser.Mediafiles[i]?.Avater, ImageView3, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                                    IdImage3 = dataUser.Mediafiles[i]?.Id.ToString();
                                    break;
                                case 3:
                                    GlideImageLoader.LoadImage(this, dataUser.Mediafiles[i]?.Avater, ImageView4, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                                    IdImage4 = dataUser.Mediafiles[i]?.Id.ToString();
                                    break;
                                case 4:
                                    GlideImageLoader.LoadImage(this, dataUser.Mediafiles[i]?.Avater, ImageView5, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                                    IdImage5 = dataUser.Mediafiles[i]?.Id.ToString();
                                    break;
                                case 5:
                                    GlideImageLoader.LoadImage(this, dataUser.Mediafiles[i]?.Avater, ImageView6, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                                    IdImage6 = dataUser.Mediafiles[i]?.Id.ToString();
                                    break;
                            }
                        }
                    }

                    if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                    {
                        ProgressBar.SetProgress(Convert.ToInt32(dataUser.ProfileCompletion), true);
                    }
                    else
                    {
                        try
                        {
                            // For API < 24 
                            ProgressBar.Progress = Convert.ToInt32(dataUser.ProfileCompletion);
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception);
                        }
                    }

                    CountPercent.Text = dataUser.ProfileCompletion + "%";
                     
                    TxtAbout.Text =IMethods.Fun_String.StringNullRemover(IMethods.Fun_String.DecodeString(dataUser.About));

                    if (!string.IsNullOrEmpty(dataUser.FirstName) && !string.IsNullOrEmpty(dataUser.LastName))
                        //TxtName.Text = dataUser.FirstName + " " + dataUser.LastName;
                        TxtName.Text = dataUser.FirstName ;
                    else
                        TxtName.Text = dataUser.Username;

                    TxtGender.Text = QuickDateTools.GetGender(Convert.ToInt32(dataUser.Gender));
                    TxtBirthday.Text = dataUser.Birthday;

                    if (IMethods.Fun_String.StringNullRemover(dataUser.Location) != "-----")
                    {
                        TxtLocation.Text = dataUser.Location;
                    }

                    if (IMethods.Fun_String.StringNullRemover(dataUser.Language) != "-----")
                    {
                        TxtLanguage.Text = dataUser.Language;
                    }

                    string relationship = QuickDateTools.GetRelationship(Convert.ToInt32(dataUser.Relationship));
                    if (IMethods.Fun_String.StringNullRemover(relationship) != "-----")
                    {
                        TxtRelationship.Text = relationship;
                    }

                    string work = QuickDateTools.GetWorkStatus(Convert.ToInt32(dataUser.WorkStatus));
                    if (IMethods.Fun_String.StringNullRemover(work) != "-----")
                    {
                        TxtWork.Text = work;
                    }

                    string education = QuickDateTools.GetEducation(Convert.ToInt32(dataUser.Education));
                    if (IMethods.Fun_String.StringNullRemover(education) != "-----")
                    {
                        TxtEducation.Text = education;
                    }

                    if (IMethods.Fun_String.StringNullRemover(dataUser.Interest) != "-----")
                    {
                        TxtInterest.Text = dataUser.Interest.Remove(dataUser.Interest.Length - 1, 1);
                    }

                    string ethnicity = QuickDateTools.GetEthnicity(Convert.ToInt32(dataUser.Ethnicity));
                    if (IMethods.Fun_String.StringNullRemover(ethnicity) != "-----")
                    {
                        TxtEthnicity.Text = ethnicity;
                    }

                    string body = QuickDateTools.GetBody(Convert.ToInt32(dataUser.Body));
                    if (IMethods.Fun_String.StringNullRemover(body) != "-----")
                    {
                        TxtBody.Text = body;
                    }

                    TxtHeight.Text = dataUser.Height + " cm";

                    string hairColor = QuickDateTools.GetHairColor(Convert.ToInt32(dataUser.HairColor));
                    if (IMethods.Fun_String.StringNullRemover(hairColor) != "-----")
                    {
                        TxtHair.Text = hairColor;
                    }

                    string character = QuickDateTools.GetCharacter(Convert.ToInt32(dataUser.Character));
                    if (IMethods.Fun_String.StringNullRemover(character) != "-----")
                    {
                        TxtCharacter.Text = character;
                    }

                    string children = QuickDateTools.GetChildren(Convert.ToInt32(dataUser.Children));
                    if (IMethods.Fun_String.StringNullRemover(children) != "-----")
                    {
                        TxtChildren.Text = children;
                    }

                    string friends = QuickDateTools.GetFriends(Convert.ToInt32(dataUser.Friends));
                    if (IMethods.Fun_String.StringNullRemover(friends) != "-----")
                    {
                        TxtFriends.Text = friends;
                    }

                    string pets = QuickDateTools.GetPets(Convert.ToInt32(dataUser.Pets));
                    if (IMethods.Fun_String.StringNullRemover(pets) != "-----")
                    {
                        TxtPets.Text = pets;
                    }

                    string liveWith = QuickDateTools.GetLiveWith(Convert.ToInt32(dataUser.LiveWith));
                    if (IMethods.Fun_String.StringNullRemover(liveWith) != "-----")
                    {
                        TxtLiveWith.Text = liveWith;
                    }

                    string car = QuickDateTools.GetCar(Convert.ToInt32(dataUser.Car));
                    if (IMethods.Fun_String.StringNullRemover(car) != "-----")
                    {
                        TxtCar.Text = car;
                    }

                    string religion = QuickDateTools.GetReligion(Convert.ToInt32(dataUser.Religion));
                    if (IMethods.Fun_String.StringNullRemover(religion) != "-----")
                    {
                        TxtReligion.Text = religion;
                    }

                    string smoke = QuickDateTools.GetSmoke(Convert.ToInt32(dataUser.Smoke));
                    if (IMethods.Fun_String.StringNullRemover(smoke) != "-----")
                    {
                        TxtSmoke.Text = smoke;
                    }

                    string drink = QuickDateTools.GetDrink(Convert.ToInt32(dataUser.Drink));
                    if (IMethods.Fun_String.StringNullRemover(drink) != "-----")
                    {
                        TxtDrink.Text = drink;
                    }

                    string travel = QuickDateTools.GetTravel(Convert.ToInt32(dataUser.Travel));
                    if (IMethods.Fun_String.StringNullRemover(travel) != "-----")
                    {
                        TxtTravel.Text = travel;
                    }

                    TxtMusic.Text = IMethods.Fun_String.StringNullRemover(dataUser.Music);
                    TxtDish.Text = IMethods.Fun_String.StringNullRemover(dataUser.Dish);
                    TxtSong.Text = IMethods.Fun_String.StringNullRemover(dataUser.Song);
                    TxtHobby.Text = IMethods.Fun_String.StringNullRemover(dataUser.Hobby);
                    TxtCity.Text = IMethods.Fun_String.StringNullRemover(dataUser.City);
                    TxtSport.Text = IMethods.Fun_String.StringNullRemover(dataUser.Sport);
                    TxtBook.Text = IMethods.Fun_String.StringNullRemover(dataUser.Book);
                    TxtMovie.Text = IMethods.Fun_String.StringNullRemover(dataUser.Movie);
                    TxtColor.Text = IMethods.Fun_String.StringNullRemover(dataUser.Colour);
                    TxtTvShow.Text = IMethods.Fun_String.StringNullRemover(dataUser.Tv);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OpenDialogGallery()
        {
            try
            {
                // Check if we're running on Android 5.0 or higher
                if ((int)Build.VERSION.SdkInt < 23)
                {
                    IMethods.IPath.Chack_MyFolder();

                    //Open Image 
                    var myUri = Uri.FromFile(new File(IMethods.IPath.FolderDcimImage, IMethods.GetTimestamp(DateTime.Now) + ".jpeg"));
                    CropImage.Builder()
                        .SetInitialCropWindowPaddingRatio(0)
                        .SetAutoZoomEnabled(true)
                        .SetMaxZoom(4)
                        .SetGuidelines(CropImageView.Guidelines.On)
                        .SetCropMenuCropButtonTitle(GetText(Resource.String.Lbl_Done))
                        .SetOutputUri(myUri).Start(this);
                }
                else
                {
                    if (!CropImage.IsExplicitCameraPermissionRequired(this) && CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted &&
                        CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Granted && CheckSelfPermission(Manifest.Permission.Camera) == Permission.Granted)
                    {
                        IMethods.IPath.Chack_MyFolder();

                        //Open Image 
                        var myUri = Uri.FromFile(new File(IMethods.IPath.FolderDcimImage, IMethods.GetTimestamp(DateTime.Now) + ".jpeg"));
                        CropImage.Builder()
                            .SetInitialCropWindowPaddingRatio(0)
                            .SetAutoZoomEnabled(true)
                            .SetMaxZoom(4)
                            .SetGuidelines(CropImageView.Guidelines.On)
                            .SetCropMenuCropButtonTitle(GetText(Resource.String.Lbl_Done))
                            .SetOutputUri(myUri).Start(this);
                    }
                    else
                    {
                        new PermissionsController(this).RequestPermission(108);
                    }
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

        public async void OnInput(MaterialDialog p0, ICharSequence p1)
        {
            try
            {
                var strName = p1.ToString();
                if (!string.IsNullOrEmpty(strName))
                {
                    if (p1.Length() <= 0) return;

                    if (TypeDialog == "About")
                    { 
                        if (IMethods.CheckConnectivity())
                        {
                            TxtAbout.Text = strName;

                            var dictionary = new Dictionary<string, string>
                            {
                                {"about", strName},
                            };

                            (int apiStatus, var respond) = await RequestsAsync.Users.UpdateProfileAsync(dictionary);
                            if (apiStatus == 200)
                            {
                                if (respond is UpdateProfileObject result)
                                {
                                    var local = ListUtils.MyUserInfo.FirstOrDefault(a => a.Id == UserDetails.UserId);
                                    if (local != null)
                                    {
                                        local.About = strName;
                                      
                                        SqLiteDatabase database = new SqLiteDatabase();
                                        database.InsertOrUpdate_DataMyInfo(local);
                                        database.Dispose();
                                    }
                                    Toast.MakeText(this, GetText(Resource.String.Lbl_Done), ToastLength.Short).Show();
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
                        else
                        {
                            Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                        }   
                    }
                    else if (TypeDialog == "Interest")
                    { 
                        if (IMethods.CheckConnectivity())
                        {
                            TxtInterest.Text = strName;

                            var dictionary = new Dictionary<string, string>
                            {
                                {"interest", strName},
                            };

                            (int apiStatus, var respond) = await RequestsAsync.Users.UpdateProfileAsync(dictionary);
                            if (apiStatus == 200)
                            {
                                if (respond is UpdateProfileObject result)
                                {
                                    var local = ListUtils.MyUserInfo.FirstOrDefault(a => a.Id == UserDetails.UserId);
                                    if (local != null)
                                    {
                                        local.Interest = strName;

                                        SqLiteDatabase database = new SqLiteDatabase();
                                        database.InsertOrUpdate_DataMyInfo(local);
                                        database.Dispose();
                                    }
                                    Toast.MakeText(this, GetText(Resource.String.Lbl_Done), ToastLength.Short).Show();
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
                        else
                        {
                            Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                        }
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