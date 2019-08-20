using SQLite;

namespace QuickDate.SQLite
{
    public class DataTables
    {
        public class LoginTb
        {
            [PrimaryKey, AutoIncrement]
            public int AutoId { get; set; }

            public string UserId { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string AccessToken { get; set; }
            public string Cookie { get; set; }
            public string Email { get; set; }
            public string Status { get; set; }

            public string Lang { get; set; }
            public string DeviceId { get; set; }
        }

        public class SettingsTb  
        {
            [PrimaryKey, AutoIncrement]
            public int AutoId { get; set; }

            public string LoadConfigInSession { get; set; }
            public string MetaDescription { get; set; }
            public string MetaKeywords { get; set; }
            public string DefaultTitle { get; set; }
            public string SiteName { get; set; }
            public string DefaultLanguage { get; set; }
            public string SmtpOrMail { get; set; }
            public string SmtpHost { get; set; }
            public string SmtpUsername { get; set; }
            public string SmtpPassword { get; set; }
            public string SmtpEncryption { get; set; }
            public string SmtpPort { get; set; }
            public string SiteEmail { get; set; }
            public string Theme { get; set; }
            public string AllLogin { get; set; }
            public string GoogleLogin { get; set; }
            public string FacebookLogin { get; set; }
            public string TwitterLogin { get; set; }
            public string LinkedinLogin { get; set; }
            public string VkontakteLogin { get; set; }
            public string FacebookAppId { get; set; }
            public string FacebookAppKey { get; set; }
            public string GoogleAppId { get; set; }
            public string GoogleAppKey { get; set; }
            public string TwitterAppId { get; set; }
            public string TwitterAppKey { get; set; }
            public string LinkedinAppId { get; set; }
            public string LinkedinAppKey { get; set; }
            public string VkontakteAppId { get; set; }
            public string VkontakteAppKey { get; set; }
            public string InstagramAppId { get; set; }
            public string InstagramAppkey { get; set; }
            public string InstagramLogin { get; set; }
            public string SmsOrEmail { get; set; }
            public string SmsPhoneNumber { get; set; }
            public string PaypalId { get; set; }
            public string PaypalSecret { get; set; }
            public string PaypalMode { get; set; }
            public string Currency { get; set; }
            public string LastBackup { get; set; }
            public string AmazoneS3 { get; set; }
            public string BucketName { get; set; }
            public string AmazoneS3Key { get; set; }
            public string AmazoneS3SKey { get; set; }
            public string Region { get; set; }
            public string SmsTPhoneNumber { get; set; }
            public string SmsTwilioUsername { get; set; }
            public string SmsTwilioPassword { get; set; }
            public string SmsProvider { get; set; }
            public string ProfilePictureWidthCrop { get; set; }
            public string ProfilePictureHeightCrop { get; set; }
            public string UserDefaultAvatar { get; set; }
            public string ProfilePictureImageQuality { get; set; }
            public string EmailValidation { get; set; }
            public string StripeSecret { get; set; }
            public string StripeId { get; set; }
            public string PushId { get; set; }
            public string PushKey { get; set; }
            public string PushId2 { get; set; }
            public string PushKey2 { get; set; }
            public string Terms { get; set; }
            public string About { get; set; }
            public string PrivacyPolicy { get; set; }
            public string FacebookUrl { get; set; }
            public string TwitterUrl { get; set; }
            public string GoogleUrl { get; set; }
            public string CurrencySymbol { get; set; }
            public string BagOfCreditsPrice { get; set; }
            public string BagOfCreditsAmount { get; set; }
            public string BoxOfCreditsPrice { get; set; }
            public string BoxOfCreditsAmount { get; set; }
            public string ChestOfCreditsPrice { get; set; }
            public string ChestOfCreditsAmount { get; set; }
            public string WeeklyProPlan { get; set; }
            public string MonthlyProPlan { get; set; }
            public string YearlyProPlan { get; set; }
            public string LifetimeProPlan { get; set; }
            public string WorkerUpdateDelay { get; set; }
            public string ProfileRecordViewsMinute { get; set; }
            public string CostPerGift { get; set; }
            public string DeleteAccount { get; set; }
            public string UserRegistration { get; set; }
            public string MaxUpload { get; set; }
            public string MimeTypes { get; set; }
            public string NormalBoostMeCreditsPrice { get; set; }
            public string MoreStickersCreditsPrice { get; set; }
            public string ProBoostMeCreditsPrice { get; set; }
            public string BoostExpireTime { get; set; }
            public string NotProChatLimitDaily { get; set; }
            public string NotProChatCredit { get; set; }
            public string NotProChatStickersCredit { get; set; }
            public string NotProChatStickersLimit { get; set; }
            public string CostPerXvisits { get; set; }
            public string XvisitsExpireTime { get; set; }
            public string CostPerXmatche { get; set; }
            public string XmatcheExpireTime { get; set; }
            public string CostPerXlike { get; set; }
            public string XlikeExpireTime { get; set; }
            public string GooglePlaceApi { get; set; }
            public bool IsRtl { get; set; }
            public string Uri { get; set; }
        }

        public class InfoUsersTb  
        {
            [PrimaryKey, AutoIncrement]
            public int AutoId { get; set; }

            public int Id { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Avater { get; set; }
            public string Address { get; set; }
            public string Gender { get; set; }
            public string Facebook { get; set; }
            public string Google { get; set; }
            public string Twitter { get; set; }
            public string Linkedin { get; set; }
            public string Website { get; set; }
            public string Instagrem { get; set; }
            public string WebDeviceId { get; set; }
            public string Language { get; set; }
            public string Src { get; set; }
            public string IpAddress { get; set; }
            public string Type { get; set; }
            public string PhoneNumber { get; set; }
            public string Timezone { get; set; }
            public string Lat { get; set; }
            public string Lng { get; set; }
            public string About { get; set; }
            public string Birthday { get; set; }
            public string Country { get; set; }
            public int Registered { get; set; }
            public int Lastseen { get; set; }
            public string SmsCode { get; set; }
            public int ProTime { get; set; }
            public int LastLocationUpdate { get; set; }
            public string Balance { get; set; }
            public string Verified { get; set; }
            public string Status { get; set; }
            public string Active { get; set; }
            public string Admin { get; set; }
            public string StartUp { get; set; }
            public string IsPro { get; set; }
            public string ProType { get; set; }
            public string SocialLogin { get; set; }
            public string CreatedAt { get; set; }
            public string UpdatedAt { get; set; }
            public string DeletedAt { get; set; }
            public string MobileDeviceId { get; set; }
            public string WebToken { get; set; }
            public string MobileToken { get; set; }
            public string Height { get; set; }
            public string HairColor { get; set; }
            public int WebTokenCreatedAt { get; set; }
            public string MobileTokenCreatedAt { get; set; }
            public string MobileDevice { get; set; }
            public string Interest { get; set; }
            public string Location { get; set; }
            public int Relationship { get; set; }
            public int WorkStatus { get; set; }
            public int Education { get; set; }
            public int Ethnicity { get; set; }
            public int Body { get; set; }
            public int Character { get; set; }
            public int Children { get; set; }
            public int Friends { get; set; }
            public int Pets { get; set; }
            public int LiveWith { get; set; }
            public int Car { get; set; }
            public int Religion { get; set; }
            public int Smoke { get; set; }
            public int Drink { get; set; }
            public int Travel { get; set; }
            public string Music { get; set; }
            public string Dish { get; set; }
            public string Song { get; set; }
            public string Hobby { get; set; }
            public string City { get; set; }
            public string Sport { get; set; }
            public string Book { get; set; }
            public string Movie { get; set; }
            public string Colour { get; set; }
            public string Tv { get; set; }
            public int PrivacyShowProfileOnGoogle { get; set; }
            public int PrivacyShowProfileRandomUsers { get; set; }
            public int PrivacyShowProfileMatchProfiles { get; set; }
            public int EmailOnProfileView { get; set; }
            public int EmailOnNewMessage { get; set; }
            public int EmailOnProfileLike { get; set; }
            public int EmailOnPurchaseNotifications { get; set; }
            public int EmailOnSpecialOffers { get; set; }
            public int EmailOnAnnouncements { get; set; }
            public int PhoneVerified { get; set; }
            public int Online { get; set; }
            public int IsBoosted { get; set; }
            public int BoostedTime { get; set; }
            public int IsBuyStickers { get; set; }
            public int UserBuyXvisits { get; set; }
            public int XvisitsCreatedAt { get; set; }
            public int UserBuyXmatches { get; set; }
            public int XmatchesCreatedAt { get; set; }
            public int UserBuyXlikes { get; set; }
            public int XlikesCreatedAt { get; set; }
            public bool VerifiedFinal { get; set; }
            public string Fullname { get; set; }
            public int Age { get; set; }
            public string LastseenTxt { get; set; }
            public string LastseenDate { get; set; }
            public bool IsOwner { get; set; }
            public bool IsLiked { get; set; }
            public bool IsBlocked { get; set; }
            public int ProfileCompletion { get; set; }
            public string ProfileCompletionMissing { get; set; }
            public string Mediafiles { get; set; }
            public string Likes { get; set; }
            public string  Blocks { get; set; }
            public string Payments  { get; set; }
            public string Reports { get; set; }
            public string Visits { get; set; }
        }
         
        public class GiftsTb
        {
            [PrimaryKey, AutoIncrement]
            public int AutoId { get; set; }
             
            public int IdGifts { get; set; }
            public string File { get; set; }
        }

        public class StickersTb
        {
            [PrimaryKey, AutoIncrement]
            public int AutoIdStickers { get; set; }
             
            public int IdStickers { get; set; }
            public string File { get; set; }
        }

        public class LastChatTb
        {
            [PrimaryKey, AutoIncrement] public int AutoIdLastChat { get; set; }

            public int Id { get; set; }
            public int Owner { get; set; }
            public string UserDataJson { get; set; }
            public int Seen { get; set; }
            public string Text { get; set; }
            public string Media { get; set; }
            public string Sticker { get; set; }
            public string Time { get; set; }
            public string CreatedAt { get; set; }
            public string UserId { get; set; }
            public int NewMessages { get; set; }
            public string MessageType { get; set; }
        }
         
        public class MessageTb
        {
            [PrimaryKey, AutoIncrement] public int AutoIdMessage { get; set; }

            public int Id { get; set; }
            public string FromName { get; set; }
            public string FromAvater { get; set; }
            public string ToName { get; set; }
            public string ToAvater { get; set; }
            public int FromId { get; set; }
            public int ToId { get; set; }
            public string Text { get; set; }
            public string Media { get; set; }
            public int FromDelete { get; set; }
            public int ToDelete { get; set; }
            public string Sticker { get; set; }
            public string CreatedAt { get; set; }
            public int Seen { get; set; }
            public string Type { get; set; }
            public string MessageType { get; set; }
        }

        public class FavoriteUsersTb  
        {
            [PrimaryKey, AutoIncrement]
            public int AutoId { get; set; }

            public int Id { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Avater { get; set; }
            public string Address { get; set; }
            public string Gender { get; set; }
            public string Facebook { get; set; }
            public string Google { get; set; }
            public string Twitter { get; set; }
            public string Linkedin { get; set; }
            public string Website { get; set; }
            public string Instagrem { get; set; }
            public string WebDeviceId { get; set; }
            public string Language { get; set; }
            public string Src { get; set; }
            public string IpAddress { get; set; }
            public string Type { get; set; }
            public string PhoneNumber { get; set; }
            public string Timezone { get; set; }
            public string Lat { get; set; }
            public string Lng { get; set; }
            public string About { get; set; }
            public string Birthday { get; set; }
            public string Country { get; set; }
            public int Registered { get; set; }
            public int Lastseen { get; set; }
            public string SmsCode { get; set; }
            public int ProTime { get; set; }
            public int LastLocationUpdate { get; set; }
            public string Balance { get; set; }
            public string Verified { get; set; }
            public string Status { get; set; }
            public string Active { get; set; }
            public string Admin { get; set; }
            public string StartUp { get; set; }
            public string IsPro { get; set; }
            public string ProType { get; set; }
            public string SocialLogin { get; set; }
            public string CreatedAt { get; set; }
            public string UpdatedAt { get; set; }
            public string DeletedAt { get; set; }
            public string MobileDeviceId { get; set; }
            public string WebToken { get; set; }
            public string MobileToken { get; set; }
            public string Height { get; set; }
            public string HairColor { get; set; }
            public int WebTokenCreatedAt { get; set; }
            public string MobileTokenCreatedAt { get; set; }
            public string MobileDevice { get; set; }
            public string Interest { get; set; }
            public string Location { get; set; }
            public int Relationship { get; set; }
            public int WorkStatus { get; set; }
            public int Education { get; set; }
            public int Ethnicity { get; set; }
            public int Body { get; set; }
            public int Character { get; set; }
            public int Children { get; set; }
            public int Friends { get; set; }
            public int Pets { get; set; }
            public int LiveWith { get; set; }
            public int Car { get; set; }
            public int Religion { get; set; }
            public int Smoke { get; set; }
            public int Drink { get; set; }
            public int Travel { get; set; }
            public string Music { get; set; }
            public string Dish { get; set; }
            public string Song { get; set; }
            public string Hobby { get; set; }
            public string City { get; set; }
            public string Sport { get; set; }
            public string Book { get; set; }
            public string Movie { get; set; }
            public string Colour { get; set; }
            public string Tv { get; set; }
            public int PrivacyShowProfileOnGoogle { get; set; }
            public int PrivacyShowProfileRandomUsers { get; set; }
            public int PrivacyShowProfileMatchProfiles { get; set; }
            public int EmailOnProfileView { get; set; }
            public int EmailOnNewMessage { get; set; }
            public int EmailOnProfileLike { get; set; }
            public int EmailOnPurchaseNotifications { get; set; }
            public int EmailOnSpecialOffers { get; set; }
            public int EmailOnAnnouncements { get; set; }
            public int PhoneVerified { get; set; }
            public int Online { get; set; }
            public int IsBoosted { get; set; }
            public int BoostedTime { get; set; }
            public int IsBuyStickers { get; set; }
            public int UserBuyXvisits { get; set; }
            public int XvisitsCreatedAt { get; set; }
            public int UserBuyXmatches { get; set; }
            public int XmatchesCreatedAt { get; set; }
            public int UserBuyXlikes { get; set; }
            public int XlikesCreatedAt { get; set; }
            public bool VerifiedFinal { get; set; }
            public string Fullname { get; set; }
            public int Age { get; set; }
            public string LastseenTxt { get; set; }
            public string LastseenDate { get; set; }
            public bool IsOwner { get; set; }
            public bool IsLiked { get; set; }
            public bool IsBlocked { get; set; }
            public int ProfileCompletion { get; set; }
            public string ProfileCompletionMissing { get; set; }
            public string Mediafiles { get; set; }
            public string Likes { get; set; }
            public string Blocks { get; set; }
            public string Payments { get; set; }
            public string Reports { get; set; }
            public string Visits { get; set; }
        }


    }
} 