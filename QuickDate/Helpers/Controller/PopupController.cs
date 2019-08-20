using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Support.V4.View;
using Android.Support.V7.Widget;
using Android.Widget;
using Me.Relex;
using QuickDate.Activities;
using QuickDate.Activities.Premium;
using QuickDate.Activities.Premium.Adapters;
using QuickDate.Activities.SettingsUser;
using QuickDate.Helpers.Fonts;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDate.PaymentGoogle;
using QuickDateClient;

namespace QuickDate.Helpers.Controller
{
    public class PopupController
    {
        public Activity ActivityContext;
        public string CreditType;
        public CreditAdapter CreditAdapter;
        public Dialog PremiumWindow, DialogAddCredits, AddPhoneNumberWindow;
        public PremiumAdapter PremiumAdapter;
        private EditText txtNumber1, txtNumber2;
        public string fullNumber;

        public PopupController(Activity context)
        {
            try
            {
                ActivityContext = context;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        public void DisplayAddPhoneNumber()
        {
            try
            {
                AddPhoneNumberWindow = new Dialog(ActivityContext, Resource.Style.MyDialogTheme);
                AddPhoneNumberWindow.SetContentView(Resource.Layout.DialogAddPhoneNumber);

                txtNumber1 = AddPhoneNumberWindow.FindViewById<EditText>(Resource.Id.numberEdit1); //Gone
                txtNumber2 = AddPhoneNumberWindow.FindViewById<EditText>(Resource.Id.numberEdit2);
                 
                var dataUser = ListUtils.MyUserInfo.FirstOrDefault(a => a.Id == UserDetails.UserId);
                if (!string.IsNullOrEmpty(dataUser?.PhoneNumber))
                {
                    var correctly = IMethods.Fun_String.IsPhoneNumber(dataUser?.PhoneNumber);
                    if (correctly)
                    {
                        txtNumber2.Text = dataUser?.PhoneNumber.TrimStart(new Char[] { '0' , '+' }); 
                    }
                }

                fullNumber = txtNumber2.Text.TrimStart(new Char[] { '0', '+' });

                var btnAddPhoneNumber = AddPhoneNumberWindow.FindViewById<Button>(Resource.Id.sentButton);
                var btnSkipAddPhoneNumber = AddPhoneNumberWindow.FindViewById<TextView>(Resource.Id.skipbutton);

                btnAddPhoneNumber.Click += BtnAddPhoneNumberOnClick;
                btnSkipAddPhoneNumber.Click += BtnSkipAddPhoneNumberOnClick;
                 
                AddPhoneNumberWindow.Show();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        private void BtnSkipAddPhoneNumberOnClick(object sender, EventArgs e)
        {
            try
            {
                AddPhoneNumberWindow.Hide();
                AddPhoneNumberWindow.Dismiss();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void BtnAddPhoneNumberOnClick(object sender, EventArgs e)
        {
            try
            {
                fullNumber =  txtNumber2.Text;

                if (!string.IsNullOrEmpty(fullNumber))
                { 
                    Intent intent = new Intent(ActivityContext, typeof(VerificationCodeActivity));
                    intent.PutExtra("Number", fullNumber);
                    ActivityContext.StartActivity(intent);

                    AddPhoneNumberWindow.Hide();
                    AddPhoneNumberWindow.Dismiss();
                } 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }


        ///////////////////////////////////////////////////////
        
        public void DisplayPremiumWindow()
        {
            try
            {
                PremiumWindow = new Dialog(ActivityContext, Resource.Style.MyDialogTheme);
                PremiumWindow.SetContentView(Resource.Layout.UpgradePremiumLayout);

                var recyclerView = PremiumWindow.FindViewById<RecyclerView>(Resource.Id.recyler);
                 
                var image1 = PremiumWindow.FindViewById<TextView>(Resource.Id.icon1);
                var image2 = PremiumWindow.FindViewById<TextView>(Resource.Id.icon2);
                var image3 = PremiumWindow.FindViewById<TextView>(Resource.Id.icon3);
                var image4 = PremiumWindow.FindViewById<TextView>(Resource.Id.icon4);
                var image5 = PremiumWindow.FindViewById<TextView>(Resource.Id.icon5);
                var image6 = PremiumWindow.FindViewById<TextView>(Resource.Id.icon6);

                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, image1, IonIconsFonts.HappyOutline);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, image2, IonIconsFonts.RibbonB);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, image3, IonIconsFonts.Heart);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, image4, IonIconsFonts.Flash);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, image5, IonIconsFonts.StatsBars);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, image6, IonIconsFonts.Wand);

                PremiumAdapter = new PremiumAdapter(ActivityContext);
                recyclerView.SetLayoutManager(new LinearLayoutManager(ActivityContext, LinearLayoutManager.Horizontal, false));
                PremiumAdapter.ItemClick += PremiumAdapterOnItemClick;
                recyclerView.SetAdapter(PremiumAdapter);

                var btnSkipAddCredits = PremiumWindow.FindViewById<Button>(Resource.Id.skippButton);
                btnSkipAddCredits.Click += BtnSkipAddCreditsOnClick;

                PremiumWindow.Show();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void BtnSkipAddCreditsOnClick(object sender, EventArgs e)
        {
            try
            {
                PremiumWindow.Hide();
                PremiumWindow.Dismiss();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Open walletFragment with Google 
        private void PremiumAdapterOnItemClick(object sender, PremiumAdapterClickEventArgs e)
        {
            try
            {
                int position = e.Position;
                if (position > -1)
                {
                    var item = PremiumAdapter.GetItem(position);
                    if (item != null)
                    {
                        Intent intent = new Intent(ActivityContext, typeof(PaymentCardDetailsActivity));
                        intent.PutExtra("Id", item.Id.ToString());
                        intent.PutExtra("Price", item.Price);
                        intent.PutExtra("payType", "membership");// credits|membership
                        ActivityContext.StartActivity(intent);

                        PremiumWindow.Hide();
                        PremiumWindow.Dismiss();
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }


        //////////////////////////////////////////////////////

        public void DisplayCreditWindow(string type)
        {
            try
            {
                CreditType = type;
                DialogAddCredits = new Dialog(ActivityContext, Resource.Style.MyDialogTheme);
                DialogAddCredits.SetContentView(Resource.Layout.DialogAddCredits);

                var recyclerView = DialogAddCredits.FindViewById<RecyclerView>(Resource.Id.recyler);

                var viewPagerView = DialogAddCredits.FindViewById<ViewPager>(Resource.Id.viewPager);
                var indicator = DialogAddCredits.FindViewById<CircleIndicator>(Resource.Id.indicator);

                var titleText = DialogAddCredits.FindViewById<TextView>(Resource.Id.mainTitelText);
                titleText.Text = ActivityContext.GetText(Resource.String.Lbl_Your) + " " + AppSettings.ApplicationName + " " + ActivityContext.GetText(Resource.String.Lbl_CreditsBalance);

                var mainText = DialogAddCredits.FindViewById<TextView>(Resource.Id.mainText);
                var data = ListUtils.MyUserInfo.FirstOrDefault(a => a.Id == UserDetails.UserId);
                mainText.Text = data?.Balance.Replace(".00","") + " " + ActivityContext.GetText(Resource.String.Lbl_Credits);

                var btnSkip = DialogAddCredits.FindViewById<Button>(Resource.Id.skippButton);
                var btnTerms = DialogAddCredits.FindViewById<TextView>(Resource.Id.TermsText);

                var creditsClass = new List<CreditsFeaturesClass>
                {
                    new CreditsFeaturesClass {Description = ActivityContext.GetText(Resource.String.Lbl_DescriptionCredits1), ColorCircle = "#00bee7",ImageFromResource = Resource.Drawable.viewPager_rocket},
                    new CreditsFeaturesClass {Description = ActivityContext.GetText(Resource.String.Lbl_DescriptionCredits2), ColorCircle = "#0456C4" ,ImageFromResource = Resource.Drawable.viewPager_msg},
                    new CreditsFeaturesClass {Description = ActivityContext.GetText(Resource.String.Lbl_DescriptionCredits3), ColorCircle = "#ff7102" ,ImageFromResource = Resource.Drawable.viewPager_gift},
                    new CreditsFeaturesClass {Description = ActivityContext.GetText(Resource.String.Lbl_DescriptionCredits4), ColorCircle = "#4caf50" ,ImageFromResource = Resource.Drawable.viewPager_target},
                    new CreditsFeaturesClass {Description = ActivityContext.GetText(Resource.String.Lbl_DescriptionCredits5), ColorCircle = "#8c4fe6" ,ImageFromResource = Resource.Drawable.viewPager_crown},
                    new CreditsFeaturesClass {Description = ActivityContext.GetText(Resource.String.Lbl_DescriptionCredits6), ColorCircle = "#22e271" ,ImageFromResource = Resource.Drawable.viewPager_sticker},
                    new CreditsFeaturesClass {Description = ActivityContext.GetText(Resource.String.Lbl_DescriptionCredits7), ColorCircle = "#f44336",ImageFromResource = Resource.Drawable.viewPager_heart}
                };
                 
                var imageDescViewPager = new ImageDescViewPager(ActivityContext, creditsClass);
                viewPagerView.Adapter = imageDescViewPager;
                indicator.SetViewPager(viewPagerView);

                CreditAdapter = new CreditAdapter(ActivityContext);
                recyclerView.SetLayoutManager(new LinearLayoutManager(ActivityContext, LinearLayoutManager.Horizontal, false));
                CreditAdapter.OnItemClick += CreditAdapterOnItemClick;
                recyclerView.SetAdapter(CreditAdapter);

                btnSkip.Click += BtnSkipOnClick;
                btnTerms.Click += BtnTermsOnClick;
                DialogAddCredits.Show();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Open walletFragment with Google
        private void CreditAdapterOnItemClick(object sender, CreditAdapterViewHolderClickEventArgs e)
        {
            try
            {
                int position = e.Position;
                if (position > -1)
                {
                    var item = CreditAdapter.GetItem(position);
                    if (item != null)
                    {
                        Intent intent = new Intent(ActivityContext, typeof(PaymentCardDetailsActivity));
                        intent.PutExtra("credits", item.TotalCoins);
                        intent.PutExtra("Price", item.Price);
                        intent.PutExtra("payType", CreditType);// credits|membership
                        ActivityContext.StartActivity(intent);

                        DialogAddCredits.Hide();
                        DialogAddCredits.Dismiss();
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        private void BtnTermsOnClick(object sender, EventArgs e)
        {
            try
            {
                var intent = new Intent(ActivityContext, typeof(LocalWebViewActivity));
                intent.PutExtra("URL", Client.WebsiteUrl + "/terms");
                intent.PutExtra("Type", ActivityContext.GetText(Resource.String.Lbl_TermsOfUse));
                ActivityContext.StartActivity(intent);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void BtnSkipOnClick(object sender, EventArgs e)
        {
            try
            {
                DialogAddCredits.Hide();
                DialogAddCredits.Dismiss();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //////////////////////////////////////////////////////
      
    }
}