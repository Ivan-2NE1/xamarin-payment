using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Database;
using Android.Graphics;
using Android.Media;
using Android.Net;
using Android.OS;
using Android.Provider;
using Android.Views;
using Android.Widget;
using Java.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Android.Graphics.Drawables;
using Android.Support.CustomTabs;
using Android.Support.V4.App;
using Android.Util;
using Android.Webkit;
using Java.Lang;
using Java.Security;
using Java.Util;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.CacheLoaders;
using Console = System.Console;
using Environment = Android.OS.Environment;
using File = System.IO.File;
using Path = System.IO.Path;
using Random = System.Random;
using Stream = System.IO.Stream;
using String = System.String;
using Thread = System.Threading.Thread;
using TimeZone = System.TimeZone;
using Exception = System.Exception;

namespace QuickDate.Helpers.Utils
{
    public class IMethods
    {

        #region Methods

        //Checks for Internet connection 
        public static bool CheckConnectivity()
        {
            try
            {
                ConnectivityManager connectivityManager = (ConnectivityManager)Application.Context.GetSystemService(Context.ConnectivityService);
                NetworkInfo networkInfo = connectivityManager.ActiveNetworkInfo;
                if (networkInfo != null)
                {
                    bool isOnline = networkInfo.IsConnectedOrConnecting;
                    if (isOnline)
                    {
                        // Now that we know it's connected, determine if we're on WiFi or something else.
                        return true;
                    }
                    else
                    {
                        //NetworkState.Disconnected;
                        return false;
                    }
                }
                else
                {
                    //NetworkState.Disconnected;
                    return false;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return false;
            }
        }

        // Add Short Cut Icon Applications
        public static void AddShortcut()
        {
            try
            {
                Intent shortcutIntent = new Intent(Application.Context, typeof(HomeActivity));
                shortcutIntent.SetAction(Intent.ActionView);

                if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
                {
                    var shortCutManager = Application.Context.GetSystemService(Context.ActivityService) as ShortcutManager;

                    if (Build.VERSION.SdkInt >= BuildVersionCodes.NMr1)
                    {
                        ShortcutInfo shortcut1 = new ShortcutInfo.Builder(Application.Context, "shortcut1")
                            .SetIntent(shortcutIntent)
                            .SetShortLabel("shortcut1")
                            .SetLongLabel("Shortcut 1")
                            .SetShortLabel("This is the shortcut 1")
                            .SetDisabledMessage("Login to open this")
                            .SetIcon(Icon.CreateWithResource(Application.Context, Resource.Drawable.icon))
                            .Build();

                        shortCutManager?.SetDynamicShortcuts((IList<ShortcutInfo>)Arrays.AsList(shortcut1));
                    }
                }
            }
            catch (Exception ex)
            {
                var exception = ex.ToString();
            }
        }

        public static void Set_SoundPlay(string Type_uri)
        {
            try
            {
                //Type_uri >>  mystic_call - Popup_GetMesseges - Popup_SendMesseges 
                var uri = Android.Net.Uri.Parse("android.resource://" + Application.Context.PackageName + "/raw/" +
                                                Type_uri);

                RingtoneManager.GetRingtone(Application.Context, uri).Play();
                //RingtoneManager.GetRingtone(Application.Context, uri).Play();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public static ClipboardManager clipBoardManager;

        public static void CopyToClipboard(string text)
        {
            try
            {
                ClipData clipData = ClipData.NewPlainText("text", text);
                clipBoardManager.PrimaryClip = clipData;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public static void Load_Image_From_Url(Activity context, ImageView Image, string ImageUrl)
        {
            try
            {
                if (!String.IsNullOrEmpty(ImageUrl))
                {
                    GlideImageLoader.LoadImage(context, ImageUrl, Image, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                }
                else
                {
                    GlideImageLoader.LoadImage(context, "no_profile_image.png", Image, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public static void Load_Image_From_Url_Normally(Activity context, ImageView Image, string ImageUrl)
        {
            try
            {
                if (!String.IsNullOrEmpty(ImageUrl))
                {
                    GlideImageLoader.LoadImage(context, ImageUrl, Image, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                }
                else
                {
                    GlideImageLoader.LoadImage(context, "ImagePlacholder.jpg", Image, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public static void Load_Image_From_Url_WithoutDownSample(Activity context, ImageView Image, string ImageUrl)
        {
            try
            {
                if (!String.IsNullOrEmpty(ImageUrl))
                {
                    GlideImageLoader.LoadImage(context, ImageUrl, Image, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                }
                else
                {
                    GlideImageLoader.LoadImage(context, "no_profile_image.png", Image, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public static string GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }

        public static byte[] ConvertFileToByteArray(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                    ms.Write(buffer, 0, read);
                return ms.ToArray();
            }
        }

        #endregion

        #region Audio Record & Play

        public class AudioRecorderAndPlayer
        {
            public MediaPlayer Player { get; set; }
            public static MediaPlayer PlayerStatic { get; set; }

            public static string AudioFileName_Released;
            public static string AudioFileFullPath_Released;
            public Java.IO.File Filedir;
            private string SoundFile;
            private Java.IO.File SoundFileFullPath;
            public MediaRecorder Recorder;
            public int RecorderDuration;

            public AudioRecorderAndPlayer(string ApplicationName)
            {
                try
                {
                    Player = new MediaPlayer();
                    Filedir = Environment.GetExternalStoragePublicDirectory(Environment.DirectoryDcim);
                    SoundFile = GetTimestamp(DateTime.Now) + ".mp3";
                    AudioFileName_Released = SoundFile;
                    SoundFileFullPath = new Java.IO.File(Filedir + "/" + ApplicationName + "/Sounds/" + SoundFile);

                    var Dir = IPath.AndroidDcimFolder + "/" + AppSettings.ApplicationName + "/Sounds/";
                    if (!Directory.Exists(Dir))
                        Directory.CreateDirectory(Dir);

                    if (!Directory.Exists(Filedir + "/" + ApplicationName))
                        Directory.CreateDirectory(Filedir + "/" + ApplicationName);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }


            public static long Get_MediaFileDuration(string path)
            {
                try
                {
                    MediaPlayer mp = new MediaPlayer();
                    FileInputStream stream = new FileInputStream(path);
                    mp.SetDataSource(stream.FD);
                    stream.Close();
                    mp.Prepare();
                    long duration = mp.Duration;
                    mp.Release();
                    return duration;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return 0;
                }
            }

            public void StartRecourding()
            {
                try
                {
                    Recorder = new MediaRecorder();
                    Recorder.Reset();
                    Recorder.SetAudioSource(AudioSource.Mic);
                    Recorder.SetOutputFormat(OutputFormat.Default);
                    Recorder.SetAudioEncoder(AudioEncoder.AmrNb);
                    Recorder.SetOutputFile(SoundFileFullPath.AbsolutePath);
                    Recorder.Prepare();
                    Recorder.Start();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }

            public void StopRecourding()
            {
                try
                {
                    Recorder.Stop();
                    Recorder.Release();
                    AudioFileFullPath_Released = SoundFileFullPath.AbsolutePath;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }

            }


            public string GetRecorded_Sound_Path()
            {
                if (File.Exists(SoundFileFullPath.AbsolutePath))
                {
                    return SoundFileFullPath.AbsolutePath;
                }
                else
                {
                    return null;
                }
            }

            public static string Check_Sound_File_if_Exits(string FolderName, string SoundFile)
            {
                var SoundFileFullPath = Environment.GetExternalStoragePublicDirectory(Environment.DirectoryDcim) + "/" +
                                        FolderName + "/" + SoundFile;
                if (File.Exists(SoundFileFullPath))
                {
                    return SoundFileFullPath;
                }
                else
                {
                    return "File Dont Exists";
                }

            }

            public Stream GetSound_as_Stream(string Path)
            {

                if (File.Exists(Path))
                {
                    byte[] databyte = File.ReadAllBytes(Path);
                    Stream stream = File.OpenRead(Path);

                    return stream;
                }
                else
                {
                    return null;
                }

            }

            public string Delete_Sound_Path(string Path)
            {
                if (File.Exists(Path))
                {
                    File.Delete(Path);

                    return "Deleted";
                }
                else
                {
                    return "Not exits";
                }
            }

            public static void PlayAudioFromAsset(string fileName)
            {
                try
                {
                    PlayerStatic = new MediaPlayer();
                    var fd = Application.Context.Assets.OpenFd(fileName);
                    PlayerStatic.Prepared += (s, e) =>
                    {
                        PlayerStatic.Start();
                    };
                    PlayerStatic.SetDataSource(fd.FileDescriptor, fd.StartOffset, fd.Length);
                    PlayerStatic.Prepare();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }

            public static void StopAudioFromAsset()
            {
                try
                {
                    if (PlayerStatic.IsPlaying)
                    {
                        PlayerStatic.Stop();
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);

                }

            }

            public void PlayAudioFromPath(string AudioPath)
            {
                try
                {
                    Player.Completion += (sender, e) =>
                    {
                        Player.Reset();
                    };

                    Player.SetDataSource(AudioPath);
                    Player.Prepare();
                    Player.Prepared += (s, e) =>
                    {
                        Player.Start();
                    };
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }


            public void StopAudioPlay()
            {
                if (Player.IsPlaying)
                {
                    Player.Stop();
                }
            }

            public void PauseAudioPlay()
            {
                if (Player.IsPlaying)
                {
                    Player.Pause();
                }

            }

            public static string GetTimeString(long millis)
            {

                String finalTimerString = "";
                String secondsString = "";
                String MinutsString = "";

                int hours = (int)(millis / (1000 * 60 * 60));
                int minutes = (int)(millis % (1000 * 60 * 60) / (1000 * 60));
                int seconds = (int)(millis % (1000 * 60 * 60) % (1000 * 60) / 1000);

                // Add hours if there
                if (hours > 0)
                {
                    finalTimerString = hours + ":";
                }

                // Prepending 0 to seconds if it is one digit
                if (seconds < 10)
                {
                    secondsString = "0" + seconds;
                }
                else
                {
                    secondsString = "" + seconds;
                }

                if (minutes < 10)
                {
                    MinutsString = "0" + minutes;
                }
                else
                {
                    MinutsString = "" + minutes;
                }

                finalTimerString = finalTimerString + MinutsString + ":" + secondsString;

                return finalTimerString;
            }

        }

        #endregion

        #region Images And video

        public class MultiMedia
        {
            public static void Save_Images_CostomName(string savedfoldername, string fileUrl, string typeimage, string imageid)
            {
                try
                {
                    string unix = fileUrl.Split('/').Last();

                    string filename = imageid + "_" + typeimage + ".jpg";
                    string filePath = Path.Combine(savedfoldername);
                    string mediaFile = filePath + "/" + filename;

                    if (!File.Exists(mediaFile))
                    {
                        if (!Directory.Exists(filePath))
                            Directory.CreateDirectory(filePath);

                        using (WebClient web = new WebClient())
                        {
                            web.DownloadDataAsync(new System.Uri(fileUrl), mediaFile);

                            web.DownloadDataCompleted += (s, e) =>
                            {
                                try
                                {
                                    File.WriteAllBytes(mediaFile, e.Result);
                                }
                                catch (Exception exception)
                                {
                                    Console.WriteLine(exception);
                                }
                            };
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            public static string Get_Images_CostomName(string savedfoldername, string typeimage, string imageid)
            {
                try
                {
                    string filename = imageid + "_" + typeimage + ".jpg";

                    string fileUrl = GetMediaFrom_Disk(savedfoldername, filename);
                    return fileUrl;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return "File Dont Exists";
                }
            }

            public static string GetMediaFrom_Disk(string foldername, string filename)
            {
                try
                {
                    string file = foldername + "/" + filename;
                    if (File.Exists(file))
                    {
                        FileInfo fi = new FileInfo(file);
                        var size = fi.Length;

                        FileInfo FileVol = new FileInfo(file);
                        string fileLength = FileVol.Length.ToString();

                        Console.WriteLine("Allen +++ " + fileLength);

                        return file;
                    }
                    else
                    {
                        return "File Dont Exists";
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return "File Dont Exists";
                }
            }

            public static string GetMediaFrom_Gallery(string foldername, string filename)
            {
                try
                {
                    string filePath = Path.Combine(foldername);
                    string MediaFile = filePath + "/" + filename;

                    if (File.Exists(MediaFile))
                    {
                        return MediaFile;
                    }
                    else
                    {
                        return "File Dont Exists";
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return "File Dont Exists";
                }
            }

            public static void DeleteMediaFrom_Disk(string path)
            {
                try
                {
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }

            }

            public static string CheckFileIfExits(string filepath)
            {
                try
                {
                    if (File.Exists(filepath))
                    {
                        return filepath;
                    }
                    else
                    {
                        return "File Dont Exists";
                    }

                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return "File Dont Exists";
                }
            }

            public static string CopyMediaFileTo(string pathOfFile, string ToFolderName, bool SaveOnPersonalFolder = true, bool SaveOnGallaryFolder = false)
            {
                //Change the file name to new unique name
                string FileName = pathOfFile.Contains("/")
                    ? pathOfFile.Split('/').Last()
                    : pathOfFile.Split('\\').Last();
                string Extension = FileName.Split('.').Last();
                FileName = FileName.Split('.').First();
                FileName = FileName.Replace(FileName, GetTimestamp(DateTime.Now)) + "." + Extension;

                string NewFolderPath = Path.Combine(ToFolderName);
                string CopyFileFullPath = NewFolderPath + "/" + FileName;

                if (SaveOnPersonalFolder)
                {
                    if (!Directory.Exists(NewFolderPath))
                        Directory.CreateDirectory(NewFolderPath);

                    if (File.Exists(pathOfFile))
                    {
                        File.Copy(pathOfFile, CopyFileFullPath);
                        return CopyFileFullPath;
                    }
                    else
                    {
                        return "Path File Dont exits";
                    }
                }

                if (SaveOnGallaryFolder)
                {
                    NewFolderPath = Path.Combine(ToFolderName);
                    CopyFileFullPath = NewFolderPath + "/" + FileName;

                    if (!Directory.Exists(NewFolderPath))
                        Directory.CreateDirectory(NewFolderPath);

                    if (File.Exists(pathOfFile))
                    {
                        File.Copy(pathOfFile, CopyFileFullPath);
                        var mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                        mediaScanIntent.SetData(Android.Net.Uri.FromFile(new Java.IO.File(CopyFileFullPath)));
                        Application.Context.SendBroadcast(mediaScanIntent);

                        return CopyFileFullPath;
                    }
                    else
                    {
                        //File.Copy(pathOfFile, CopyFileFullPath);
                        return "Path File Dont exits";
                    }
                }
                return "Done";
            }

            public static void DownloadMediaTo_DiskAsync(string savedfoldername, string url)
            {
                try
                {
                    if (url.Contains("http"))
                    {
                        string filename = url.Split('/').Last();
                        string filePath = Path.Combine(savedfoldername);
                        string mediaFile = filePath + "/" + filename;

                        if (!Directory.Exists(filePath))
                            Directory.CreateDirectory(filePath);

                        if (!File.Exists(mediaFile))
                        {
                            WebClient webClient = new WebClient();

                            webClient.DownloadDataAsync(new System.Uri(url), mediaFile);

                            webClient.DownloadDataCompleted += (s, e) =>
                            {
                                try
                                {
                                    File.WriteAllBytes(mediaFile, e.Result);
                                }
                                catch (Exception exception)
                                {
                                    Console.WriteLine(exception);
                                }
                            };
                        }
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }

            public static void DownloadMediaTo_GalleryAsync(string Savedfoldername, string Url)
            {
                try
                {
                    string Filename = Url.Split('/').Last();
                    string filePath = Path.Combine(Savedfoldername);
                    string MediaFile = filePath + "/" + Filename;

                    if (!Directory.Exists(filePath))
                        Directory.CreateDirectory(filePath);

                    if (!File.Exists(MediaFile))
                    {
                        WebClient WebClient = new WebClient();

                        WebClient.DownloadDataAsync(new System.Uri(Url));
                        WebClient.DownloadDataCompleted += (s, e) =>
                        {
                            try
                            {
                                File.WriteAllBytes(MediaFile, e.Result);
                            }
                            catch (Exception exception)
                            {
                                Console.WriteLine(exception);
                            }

                            var mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                            mediaScanIntent.SetData(Android.Net.Uri.FromFile(new Java.IO.File(MediaFile)));
                            Application.Context.SendBroadcast(mediaScanIntent);
                        };
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }

            public static string GetRealVideoPathFromURI(Android.Net.Uri contentUri)
            {
                ICursor cursor = null;
                try
                {
                    String[] proj = { MediaStore.Video.Media.InterfaceConsts.Data };
                    cursor = Application.Context.ContentResolver.Query(contentUri, proj, null, null, null);
                    int column_index = cursor.GetColumnIndexOrThrow(MediaStore.Video.Media.InterfaceConsts.Data);
                    cursor.MoveToFirst();
                    return cursor.GetString(column_index);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return null;
                }
                finally
                {
                    cursor?.Close();
                }
            }

            public static string GetRealImagePathFromURI(Android.Net.Uri contentUri)
            {
                ICursor cursor = null;
                try
                {
                    String[] proj = { MediaStore.Images.Media.InterfaceConsts.Data };
                    cursor = Application.Context.ContentResolver.Query(contentUri, proj, null, null, null);
                    int column_index = cursor.GetColumnIndexOrThrow(MediaStore.Images.Media.InterfaceConsts.Data);
                    cursor.MoveToFirst();
                    return cursor.GetString(column_index);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return null;
                }
                finally
                {
                    cursor?.Close();
                }
            }

            public static bool IsCameraAvailable()
            {
                PackageManager pm = Application.Context.PackageManager;
                if (pm.HasSystemFeature(PackageManager.FeatureCamera))
                    return true;

                return false;
            }

            public static Bitmap Retrieve_VideoFrame_AsBitmap(string MediaFile, ThumbnailKind Thumbnail_Kind = ThumbnailKind.MiniKind)
            {
                try
                {
                    Bitmap bitmap = ThumbnailUtils.CreateVideoThumbnail(MediaFile, Thumbnail_Kind);

                    return bitmap;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return null;
                }
            }

            public static void Export_Bitmap_As_Image(Bitmap bitmap, string Filename, string pathTofolder)
            {
                try
                {
                    if (!Directory.Exists(pathTofolder))
                        Directory.CreateDirectory(pathTofolder);

                    string filePath = Path.Combine(pathTofolder);
                    string mediaFile = filePath + "/" + Filename + ".png";
                    var stream = new FileStream(mediaFile, FileMode.Create);
                    bitmap.Compress(Bitmap.CompressFormat.Png, 100, stream);
                    stream.Close();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }

            public static Stream GetMedia_as_Stream(string Path)
            {
                try
                {
                    byte[] datass = File.ReadAllBytes(Path);
                    Stream dsd = File.OpenRead(Path);
                    return dsd;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return null;
                }
            }

            public void image_compression(string Path)
            {
                try
                {
                    string anyString = File.ReadAllText(Path);
                    CompressStringToFile("new.gz", anyString);
                }
                catch (Exception exception) // Couldn't compress.
                {
                    Console.WriteLine(exception);
                }
            }

            public static void CompressStringToFile(string fileName, string value)
            {
                try
                {
                    string temp = Path.GetTempFileName();
                    File.WriteAllText(temp, value);
                    byte[] b;
                    using (FileStream f = new FileStream(temp, FileMode.Open))
                    {
                        b = new byte[f.Length];
                        f.Read(b, 0, (int)f.Length);
                    }
                    using (FileStream f2 = new FileStream(fileName, FileMode.Create))
                    using (GZipStream gz = new GZipStream(f2, CompressionMode.Compress, false))
                    {
                        gz.Write(b, 0, b.Length);
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
        }

        #endregion

        #region Contacts

        public class PhoneContactManager
        {
            public class UserContact
            {
                public string PhoneNumber { get; set; }
                public string UserDisplayName { get; set; }
                public string FirstName { get; set; }
                public string LastName { get; set; }
            }

            public static IEnumerable<UserContact> GetAllContacts()
            {
                var PhoneContactsList = new ObservableCollection<UserContact>();
                using (var phones = Application.Context.ContentResolver.Query(ContactsContract.CommonDataKinds.Phone.ContentUri, null, null, null, null))
                {
                    if (phones != null)
                    {
                        while (phones.MoveToNext())
                        {
                            try
                            {
                                string name = phones.GetString(phones.GetColumnIndex(ContactsContract.Contacts.InterfaceConsts.DisplayName));
                                string phoneNumber = phones.GetString(phones.GetColumnIndex(ContactsContract.CommonDataKinds.Phone.Number));

                                string[] words = name.Split(' ');
                                var contact = new UserContact();

                                contact.FirstName = words[0];

                                if (words.Length > 1)
                                    contact.LastName = words[1];
                                else
                                    contact.LastName = ""; //no last name

                                contact.UserDisplayName = name;
                                contact.PhoneNumber = phoneNumber.Replace("+", "00").Replace("-", "").Replace(" ", "");

                                var check = PhoneContactsList.FirstOrDefault(a => a.PhoneNumber == contact.PhoneNumber);
                                if (check == null)
                                {
                                    PhoneContactsList.Add(contact);
                                }
                            }
                            catch (Exception exception)
                            {
                                //something wrong with one contact, may be display name is completely empty, decide what to do
                                Console.WriteLine(exception);
                            }
                        }
                        phones.Close();
                    }
                    // if we get here, we can't access the contacts. Consider throwing an exception to display to the user
                }

                return PhoneContactsList;
            }

            public static UserContact Get_ContactInfoBy_Id(string fromUriId)
            {
                ICursor cursor = null;
                try
                {
                    //var uri = ContactsContract.Contacts.ContentUri;
                    var contacts = Application.Context.ContentResolver.Query(ContactsContract.CommonDataKinds.Phone.ContentUri, null, "_id = ?", new string[] { fromUriId }, null);
                    if (contacts != null)
                    {
                        UserContact UserContact = new UserContact();
                        contacts.MoveToFirst();
                        string displayName = contacts.GetString(contacts.GetColumnIndex("display_name"));
                        int indexNumber = contacts.GetColumnIndex(ContactsContract.CommonDataKinds.Phone.Number);


                        string mobileNumber = contacts.GetString(indexNumber);

                        UserContact.PhoneNumber = mobileNumber;
                        UserContact.UserDisplayName = displayName;

                        //var columnNames = contacts.GetColumnNames();
                        //foreach (var columnName in columnNames)
                        //{
                        //    int index = contacts.GetColumnIndex(columnName);
                        //    var value = contacts.GetString(index);
                        //    Console.WriteLine("Allen >> index = {0}, value = {1}", index, value);
                        //}

                        if (!string.IsNullOrEmpty(mobileNumber))
                        {
                            return UserContact;
                        }


                        return null;
                    }
                    else
                    {
                        return null;
                    }

                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return null;
                }
                finally
                {
                    cursor?.Close();
                }
            }

            public void InsertContact(string FisrtName, string LastName, string Number, string Email, string Company)
            {

                try
                {
                    List<ContentProviderOperation> ops = new List<ContentProviderOperation>();
                    ContentProviderOperation.Builder builder =
                        ContentProviderOperation.NewInsert(ContactsContract.RawContacts.ContentUri);
                    builder.WithValue(ContactsContract.RawContacts.InterfaceConsts.AccountType, null);
                    builder.WithValue(ContactsContract.RawContacts.InterfaceConsts.AccountName, null);
                    ops.Add(builder.Build());

                    //Name
                    builder = ContentProviderOperation.NewInsert(ContactsContract.Data.ContentUri);
                    builder.WithValueBackReference(ContactsContract.Data.InterfaceConsts.RawContactId, 0);
                    builder.WithValue(ContactsContract.Data.InterfaceConsts.Mimetype,
                        ContactsContract.CommonDataKinds.StructuredName.ContentItemType);
                    builder.WithValue(ContactsContract.CommonDataKinds.StructuredName.FamilyName, LastName);
                    builder.WithValue(ContactsContract.CommonDataKinds.StructuredName.GivenName, FisrtName);
                    ops.Add(builder.Build());

                    //Number
                    builder = ContentProviderOperation.NewInsert(ContactsContract.Data.ContentUri);
                    builder.WithValueBackReference(ContactsContract.Data.InterfaceConsts.RawContactId, 0);
                    builder.WithValue(ContactsContract.Data.InterfaceConsts.Mimetype,
                        ContactsContract.CommonDataKinds.Phone.ContentItemType);
                    builder.WithValue(ContactsContract.CommonDataKinds.Phone.Number, Number);
                    builder.WithValue(ContactsContract.CommonDataKinds.Phone.InterfaceConsts.Type,
                        ContactsContract.CommonDataKinds.Phone.InterfaceConsts.TypeCustom);
                    builder.WithValue(ContactsContract.CommonDataKinds.Phone.InterfaceConsts.Label, "Work");
                    ops.Add(builder.Build());

                    //Email
                    builder = ContentProviderOperation.NewInsert(ContactsContract.Data.ContentUri);
                    builder.WithValueBackReference(ContactsContract.Data.InterfaceConsts.RawContactId, 0);
                    builder.WithValue(ContactsContract.Data.InterfaceConsts.Mimetype,
                        ContactsContract.CommonDataKinds.Email.ContentItemType);
                    builder.WithValue(ContactsContract.CommonDataKinds.Email.InterfaceConsts.Data, Email);
                    builder.WithValue(ContactsContract.CommonDataKinds.Email.InterfaceConsts.Type,
                        ContactsContract.CommonDataKinds.Email.InterfaceConsts.TypeCustom);
                    builder.WithValue(ContactsContract.CommonDataKinds.Email.InterfaceConsts.Label, "Work");
                    ops.Add(builder.Build());

                    //Company
                    builder = ContentProviderOperation.NewInsert(ContactsContract.Data.ContentUri);
                    builder.WithValueBackReference(ContactsContract.Data.InterfaceConsts.RawContactId, 0);
                    builder.WithValue(ContactsContract.Data.InterfaceConsts.Mimetype,
                        ContactsContract.CommonDataKinds.Organization.ContentItemType);
                    builder.WithValue(ContactsContract.CommonDataKinds.Organization.InterfaceConsts.Data, Company);
                    builder.WithValue(ContactsContract.CommonDataKinds.Organization.InterfaceConsts.Type,
                        ContactsContract.CommonDataKinds.Organization.InterfaceConsts.TypeCustom);
                    builder.WithValue(ContactsContract.CommonDataKinds.Organization.InterfaceConsts.Label, "Work");
                    ops.Add(builder.Build());

                    ContentProviderResult[] res;
                    try
                    {
                        res = Application.Context.ContentResolver.ApplyBatch(ContactsContract.Authority,
                            ops);

                        Toast.MakeText(Application.Context, "Done contacted added", ToastLength.Short)
                            .Show();
                    }
                    catch (Exception exception)
                    {
                        Toast.MakeText(Application.Context, "Error ", ToastLength.Long).Show();
                        Console.WriteLine(exception);
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }

        }

        #endregion

        #region String 

        public class Fun_String
        {
            //========================= Variables =========================
            public static Random random = new Random();

            //========================= Functions =========================

            //creat new Random String Session 
            public static string RandomString(int length)
            {
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXdsdaawerthklmnbvcxer46gfdsYZ0123456789";
                return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
            }

            //creat new Random Color
            public static string RandomColor()
            {
                string color = "";
                int b;
                b = random.Next(1, 11);
                switch (b)
                {
                    case 5:
                        {
                            color = "#314d74"; //dark blue
                        }
                        break;
                    case 1:
                        {

                            color = "#404040"; //dark gray
                        }
                        break;
                    case 2:
                        {
                            color = "#146c7c"; // nele
                        }
                        break;
                    case 4:
                        {
                            color = "#789c74"; //dark green
                        }
                        break;
                    case 6:
                        {
                            color = "#cc8237"; //brown
                        }
                        break;
                    case 7:
                        {
                            color = "#c37887"; //light red
                        }
                        break;
                    case 8:
                        {
                            color = "#AD1457"; //Pink
                        }
                        break;
                    case 9:
                        {
                            color = "#7646ff"; //purple
                        }
                        break;
                    case 3:
                        {
                            color = "#cc635c"; //red
                        }
                        break;
                    case 10:
                        {
                            color = "#20cef5"; //Light blue
                        }
                        break;

                }
                return color;
            }

            public static string GetoLettersfromString(string key)
            {
                try
                {
                    var string1 = key.Split(' ').First();
                    var string2 = key.Split(' ').Last();

                    if (string1 != string2)
                    {
                        String substring1 = string1.Substring(0, 1);
                        String substring2 = string2.Substring(0, 1);
                        var result = substring1 + substring2;
                        return result.ToUpper();
                    }
                    else
                    {
                        String substring1 = string1.Substring(0, 2);

                        var result = substring1;
                        return result.ToUpper();
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return "";
                }
            }

            public static string Format_byte_size(string filepath)
            {
                try
                {
                    /*
                    * var size = new FileInfo(filepath).Length;
                    * double totalSize = size / 1024.0F / 1024.0F;
                    * string sizeFile = totalSize.ToString("0.### KB"); 
                    */

                    string[] sizes = { "B", "KB", "MB", "GB", "TB" };
                    double len = new FileInfo(filepath).Length;
                    int order = 0;
                    while (len >= 1024 && order < sizes.Length - 1)
                    {
                        order++;
                        len = len / 1024;
                    }

                    // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
                    // show a single decimal place, and no space.
                    string result = $"{len:0.##} {sizes[order]}";
                    return result;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return "0B";
                }
            }

            public static string UppercaseFirst(string s)
            {
                // Check for empty string.
                if (string.IsNullOrEmpty(s))
                {
                    return string.Empty;
                }
                // Return char and concat substring.
                return char.ToUpper(s[0]) + s.Substring(1);
            }

            public static string TrimTo(string str, int maxLength)
            {
                try
                {
                    if (str.Length <= maxLength)
                    {
                        return str;
                    }
                    else if (str.Length > 35)
                    {
                        str.Remove(0, 10);
                        return str;
                    }
                    else if (str.Length > 65)
                    {
                        str.Remove(0, 30);
                        return str;
                    }
                    else if (str.Length > 85)
                    {
                        str.Remove(0, 50);
                        return str;
                    }
                    else if (str.Length > 105)
                    {
                        str.Remove(0, 70);
                        return str;
                    }
                    else
                    {
                        return str.Substring(maxLength - 17, maxLength);
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return str.Substring(maxLength - 17, maxLength);
                }
            }

            //SubString Cut Of
            public static string SubStringCutOf(string s, int x)
            {
                try
                {
                    if (s.Length > x)
                    {
                        String substring = s.Substring(0, x);
                        return substring + "...";
                    }
                    else
                    {
                        return s;
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return s;
                }
            }

            //Null Remover >> return Empty
            public static string StringNullRemover(string s)
            {
                try
                {
                    if (string.IsNullOrEmpty(s))
                    {
                        s = "-----";
                    }
                    return s;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return s;
                }
            }

            //De code
            public static string DecodeString(string Content)
            {
                try
                {
                    const string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";
                    const string stripFormatting = @"<[^>]*(>|$)";
                    const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";
                    var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
                    var stripFormattingRegex = new Regex(stripFormatting, RegexOptions.Multiline);
                    var tagWhiteSpaceRegex = new Regex(tagWhiteSpace, RegexOptions.Multiline);

                    var text = Content;

                    //Decode html specific characters
                    text = WebUtility.HtmlDecode(text);

                    //Remove tag whitespace/line breaks
                    text = tagWhiteSpaceRegex.Replace(text, "><");

                    //Replace <br /> with line breaks
                    text = lineBreakRegex.Replace(text, System.Environment.NewLine);

                    //Strip formatting
                    text = stripFormattingRegex.Replace(text, string.Empty);
                     
                    var stringFormater = text.Replace(":)", "\ud83d\ude0a")
                        .Replace(";)", "\ud83d\ude09")
                        .Replace("0)", "\ud83d\ude07")
                        .Replace("(<", "\ud83d\ude02")
                        .Replace(":D", "\ud83d\ude01")
                        .Replace("*_*", "\ud83d\ude0d")
                        .Replace("(<", "\ud83d\ude02")
                        .Replace("<3", "\ud83d\u2764")
                        .Replace("/_)", "\ud83d\ude0f")
                        .Replace("-_-", "\ud83d\ude11")
                        .Replace(":-/", "\ud83d\ude15")
                        .Replace(":*", "\ud83d\ude18")
                        .Replace(":_p", "\ud83d\ude1b")
                        .Replace(":p", "\ud83d\ude1c")
                        .Replace("x(", "\ud83d\ude20")
                        .Replace("X(", "\ud83d\ude21")
                        .Replace(":_(", "\ud83d\ude22")
                        .Replace("<5", "\ud83d\u2B50")
                        .Replace(":0", "\ud83d\ude31")
                        .Replace("B)", "\ud83d\ude0e")
                        .Replace("o(", "\ud83d\ude27")
                        .Replace("</3", "\uD83D\uDC94")
                        .Replace(":o", "\ud83d\ude26")
                        .Replace("o(", "\ud83d\ude27")
                        .Replace(":__(", "\ud83d\ude2d")
                        .Replace("!_", "\uD83D\u2757")
                        .Replace("<br> <br>", "\n")
                        .Replace("<br />", "\n")
                        .Replace("[/a]", "/")
                        .Replace("[a]", "")
                        .Replace("%3A", ":")
                        .Replace("%2F", "/")
                        .Replace("%3F", "?")
                        .Replace("%3D", "=")
                        .Replace("<a href=", "")
                        .Replace("target=", "")
                        .Replace("_blank", "")
                        .Replace(@"""", "")
                        .Replace("</a>", "")
                        .Replace("class=hash", "")
                        .Replace("rel=nofollow>", "")
                        .Replace("<p>", "")
                        .Replace("</p>", "")
                        .Replace("</body>", "")
                        .Replace("<body>", "")
                        .Replace("<div>", "")
                        .Replace("<div ", "")
                        .Replace("</div>", "")
                        .Replace("&#039;", "'")
                        .Replace("&amp;", "&")
                        .Replace("&lt;", "<")
                        .Replace("&gt;", ">")
                        .Replace("&le;", "≤")
                        .Replace("&ge;", "≥")
                        .Replace("<iframe", "")
                        .Replace("</iframe>", "")
                        .Replace("<table", "")
                        .Replace("<ul>", "")
                        .Replace("<li>", "")
                        .Replace("&nbsp;", "")
                        .Replace("&amp;nbsp;&lt;/p&gt;&lt;p&gt;", "\r\n")
                        .Replace("&amp;", "&")
                        .Replace("&quot;", "")
                        .Replace("&apos;", "")
                        .Replace("&cent;", "¢")
                        .Replace("&pound;", "£")
                        .Replace("&yen;", "¥")
                        .Replace("&euro;", "€")
                        .Replace("&copy;", "©")
                        .Replace("&reg;", "®")
                        .Replace("<b>", "")
                        .Replace("<u>", "")
                        .Replace("<i>", "")
                        .Replace("</i>", "")
                        .Replace("</u>", "")
                        .Replace("</b>", "")
                        .Replace("<br>", "\n")
                        .Replace("</li>", "")
                        .Replace("</ul>", "")
                        .Replace("</table>", " ")
                        .Replace("a&#768;", "")
                        .Replace("a&#769;", "")
                        .Replace("a&#770;", "")
                        .Replace("a&#771;", "")
                        .Replace("O&#768;", "")
                        .Replace("O&#769;", "")
                        .Replace("O&#770;", "")
                        .Replace("O&#771;", "")
                        .Replace("</table>", "")
                        .Replace("</table>", " ");

                    return stringFormater;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return "";
                }
            }

            //String format numbers to millions, thousands with rounding
            public static string FormatPriceValue(int num)
            {
                try
                {
                    if (num >= 100000000)
                    {
                        return ((num >= 10050000 ? num - 500000 : num) / 1000000D).ToString("#M");
                    }
                    if (num >= 10000000)
                    {
                        return ((num >= 10500000 ? num - 50000 : num) / 1000000D).ToString("0.#M");
                    }
                    if (num >= 1000000)
                    {
                        return ((num >= 1005000 ? num - 5000 : num) / 1000000D).ToString("0.##M");
                    }
                    if (num >= 100000)
                    {
                        return ((num >= 100500 ? num - 500 : num) / 1000D).ToString("0.k");
                    }
                    if (num >= 10000)
                    {
                        return ((num >= 10550 ? num - 50 : num) / 1000D).ToString("0.#k");
                    }

                    return num >= 1000? ((num >= 1005 ? num - 5 : num) / 1000D).ToString("0.##k") : num.ToString("#,0");
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return num.ToString();
                }
            }

            public static bool IsEmailValid(string emailaddress)
            {
                try
                {
                    MailAddress m = new MailAddress(emailaddress);

                    return true;
                }
                catch (FormatException)
                {
                    return false;
                }
            }

            public static bool IsUrlValid(string url)
            {
                try
                {
                    string pattern =
                        @"^(http|https|ftp|)\://|[a-zA-Z0-9\-\.]+\.[a-zA-Z](:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]$";
                    Regex reg = new Regex(pattern, RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
                    Match m = reg.Match(url);
                    while (m.Success)
                    {
                        //do things with your matching text 
                        m = m.NextMatch();
                        break;
                    }
                    if (reg.IsMatch(url))
                    {
                        var ss = "http://" + m.Value;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return false;
                }
            }

            public static bool IsPhoneNumber(string number)
            {
                return Regex.Match(number, @"^(\+?)([0-9]{9,20}?)$").Success;
            }


            // Functions convert color RGB to HEX
            public static string ConvertColorRGBtoHex(string color)
            {
                //to rgba => string.Format("rgba({0}, {1}, {2}, {3});", color_red, color_green, color_blue, color_alpha);
                try
                {
                    if (color.Contains("rgb"))
                    {
                        var regex = new Regex(@"([0-9]+)");
                        string colorData = color;

                        var matches = regex.Matches(colorData);

                        var color_red = Convert.ToInt32(matches[0].ToString());
                        var color_green = Convert.ToInt32(matches[1].ToString());
                        var color_blue = Convert.ToInt32(matches[2].ToString());
                        var color_alpha = Convert.ToInt16(matches[3].ToString());
                        var hex = $"#{color_red:X2}{color_green:X2}{color_blue:X2}";

                        return hex;
                    }
                    else
                    {
                        return AppSettings.MainColor;
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return AppSettings.MainColor;
                }
            }

            public static bool OnlyHexInString(string color)
            {
                try
                {
                    if (color.Contains("rgba"))
                    {
                        var regex = new Regex(@"([0-9]+)");
                        string colorData = color;

                        var matches = regex.Matches(colorData);

                        var color_red = Convert.ToInt32(matches[0].ToString());
                        var color_green = Convert.ToInt32(matches[1].ToString());
                        var color_blue = Convert.ToInt32(matches[2].ToString());
                        var color_alpha = Convert.ToInt16(matches[3].ToString());
                        var hex = $"#{color_alpha:X2}{color_red:X2}{color_green:X2}{color_blue:X2}";

                        return true;
                    }
                    else if (color.Contains("rgb"))
                    {
                        var regex = new Regex(@"([0-9]+)");
                        string colorData = color;

                        var matches = regex.Matches(colorData);
                        var color_red = Convert.ToInt32(matches[0].ToString());
                        var color_green = Convert.ToInt32(matches[1].ToString());
                        var color_blue = Convert.ToInt32(matches[2].ToString());
                        var color_alpha = Convert.ToInt16(00);
                        var hex = $"#{color_alpha:X2}{color_red:X2}{color_green:X2}{color_blue:X2}";

                        return true;
                    }

                    var Rx_Color = new Regex("^#(?:[0-9a-fA-F]{3}){1,2}$");
                    var Rx_Color2 = new Regex("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3}|[0-9]{3}|[0-9]{6})$");
                    var Rx_Color3 = new Regex(@"\A\b[0-9a-fA-F]+\b\Z");
                    var Rx_Color4 =
                        new Regex(
                            @"\A\b(0[xX])?[0-9a-fA-F]+\b\Z"); // For C-style hex notation (0xFF) you can use @"\A\b(0[xX])?[0-9a-fA-F]+\b\Z"

                    if (Rx_Color.IsMatch(color) || Rx_Color2.IsMatch(color) || Rx_Color3.IsMatch(color) ||
                        Rx_Color4.IsMatch(color))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return false;
                }
            }

            public static string Check_Regex(string text)
            {
                try
                {
                    var Rx_Email = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                    var Rx_Website = new Regex(@"^(http|https|ftp|www)\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*$");
                    var Rx_Hashtag = new Regex(@"(?<=#)\w+");
                    var Rx_Mention = new Regex("@(?<name>[^\\s]+)");
                    var Rx_Number1 = new Regex(@"^\d$");
                    var Rx_Number2 = new Regex("[0-9]");
                    var result_Email = IsEmailValid(text);
                    var result_Web = IsUrlValid(text);

                    if (Rx_Email.IsMatch(text) || result_Email)
                    {
                        return "Email";
                    }
                    else if (Rx_Website.IsMatch(text) || result_Web)
                    {
                        return "Website";
                    }
                    else if (Rx_Hashtag.IsMatch(text))
                    {
                        return "Hashtag";
                    }
                    else if (Rx_Mention.IsMatch(text))
                    {
                        //var results = Rx_Mention.Matches(text).Cast<Match>().Select(m => m.Groups["name"].Value).ToArray();

                        return "Mention";
                    }
                    else if (Rx_Number1.IsMatch(text) || Rx_Number2.IsMatch(text))
                    {
                        return "Number";
                    }
                    else
                    {
                        return text;
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return text;
                }
            }
        }

        #endregion

        #region Time

        public class Time
        {
            public static string LblJustNow = Application.Context.GetText(Resource.String.Lbl_justNow);
            public static string LblHours = Application.Context.GetText(Resource.String.Lbl_hours);
            public static string LblDays = Application.Context.GetText(Resource.String.Lbl_days);
            public static string LblMonth = Application.Context.GetText(Resource.String.Lbl_month);
            public static string LblMinutes = Application.Context.GetText(Resource.String.Lbl_minutes);
            public static string LblSeconds = Application.Context.GetText(Resource.String.Lbl_seconds);
            public static string LblYear = Application.Context.GetText(Resource.String.Lbl_year);
            public static string LblCutHours = Application.Context.GetText(Resource.String.Lbl_CutHours);
            public static string LblCutDays = Application.Context.GetText(Resource.String.Lbl_CutDays);
            public static string LblCutMonth = Application.Context.GetText(Resource.String.Lbl_CutMonth);
            public static string LblCutMinutes = Application.Context.GetText(Resource.String.Lbl_CutMinutes);
            public static string LblCutSeconds = Application.Context.GetText(Resource.String.Lbl_CutSeconds);
            public static string LblCutYear = Application.Context.GetText(Resource.String.Lbl_CutYear);
            public static string LblAboutMinute = Application.Context.GetText(Resource.String.Lbl_about_minute);
            public static string LblAboutHour = Application.Context.GetText(Resource.String.Lbl_about_hour);
            public static string LblYesterday = Application.Context.GetText(Resource.String.Lbl_yesterday);
            public static string LblAboutMonth = Application.Context.GetText(Resource.String.Lbl_about_month);
            public static string LblAboutYear = Application.Context.GetText(Resource.String.Lbl_about_year);

            //Split String Duration (00:00:00)
            public static string SplitStringDuration(string duration)
            {
                try
                {
                    string[] durationsplit = duration.Split(':');
                    if (durationsplit.Length == 3)
                    {

                        if (durationsplit[0] == "00")
                        {
                            string newDuration = durationsplit[1] + ":" + durationsplit[2];
                            return newDuration;
                        }
                        else
                        {
                            return duration;
                        }
                    }
                    else
                    {
                        return duration;
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return duration;
                }
            }

            //Get TimeZone
            public static string GetTimeZone()
            {
                try
                {
                    const string dataFmt = "{0,-30}{1}";
                    const string timeFmt = "{0,-30}{1:MM-dd-yyyy HH:mm}";
                    TimeZone curTimeZone = TimeZone.CurrentTimeZone;
                    // What is TimeZone name?
                    Console.WriteLine(dataFmt, "TimeZone Name:", curTimeZone.StandardName);
                    // Is TimeZone DayLight Saving?|
                    Console.WriteLine(dataFmt, "Daylight saving time?", curTimeZone.IsDaylightSavingTime(DateTime.Now));
                    // What is GMT (also called Coordinated Universal Time (UTC)
                    DateTime curUtc = curTimeZone.ToUniversalTime(DateTime.Now);
                    Console.WriteLine(timeFmt, "Coordinated Universal Time:", curUtc);
                    // What is GMT/UTC offset ?
                    TimeSpan currentOffset = curTimeZone.GetUtcOffset(DateTime.Now);
                    Console.WriteLine(dataFmt, "UTC offset:", currentOffset);
                    // Get DaylightTime object 
                    System.Globalization.DaylightTime dl = curTimeZone.GetDaylightChanges(DateTime.Now.Year);
                    // DateTime when the daylight-saving period begins.
                    Console.WriteLine("Start: {0:MM-dd-yyyy HH:mm} ", dl.Start);
                    // DateTime when the daylight-saving period ends.
                    Console.WriteLine("End: {0:MM-dd-yyyy HH:mm} ", dl.End);
                    // Difference between standard time and the daylight-saving time.
                    Console.WriteLine("delta: {0}", dl.Delta);
                    Console.Read();

                    Android.Icu.Util.Calendar cal = Android.Icu.Util.Calendar.Instance;
                    var tz = cal.TimeZone;
                    Log.Debug("Time zone", "=" + tz.DisplayName);

                    var time = Java.Util.TimeZone.Default.DisplayName;
                    return !string.IsNullOrEmpty(time) ? time : "UTC";
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return "UTC";
                }
            }

            public static string TimeAgo(int time)
            {
                try
                {
                    DateTime dateTime = UnixTimeStampToDateTime(time);
                    string result = string.Empty;
                    var timeSpan = DateTime.Now.Subtract(dateTime);

                    if (timeSpan <= TimeSpan.FromSeconds(60))
                    {
                        //result = $"{timeSpan.Seconds} " + Lbl_seconds;
                        result = LblJustNow;
                    }
                    else if (timeSpan <= TimeSpan.FromMinutes(60))
                    {
                        result = timeSpan.Minutes > 1 ? $"{timeSpan.Minutes} " + LblMinutes : LblAboutMinute;
                    }
                    else if (timeSpan <= TimeSpan.FromHours(24))
                    {
                        result = timeSpan.Hours > 1 ? $"{timeSpan.Hours} " + LblHours : LblAboutHour;
                    }
                    else if (timeSpan <= TimeSpan.FromDays(30))
                    {
                        result = timeSpan.Days > 1 ? $"{timeSpan.Days} " + LblDays : LblYesterday;
                    }
                    else if (timeSpan <= TimeSpan.FromDays(365))
                    {
                        result = timeSpan.Days > 30 ? $"{timeSpan.Days / 30} " + LblMonth : LblAboutMonth;
                    }
                    else
                    {
                        result = timeSpan.Days > 365 ? $"{timeSpan.Days / 365} " + LblYear : LblAboutYear;
                    }

                    return result;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return time.ToString();
                }
            }

            public static string TimeAgo(DateTime dateTime)
            {
                try
                {
                    string result = string.Empty;
                    var timeSpan = DateTime.Now.Subtract(dateTime);

                    if (timeSpan <= TimeSpan.FromSeconds(60))
                    {
                        //result = $"{timeSpan.Seconds} " + Lbl_seconds;
                        result = LblJustNow;
                    }
                    else if (timeSpan <= TimeSpan.FromMinutes(60))
                    {
                        result = timeSpan.Minutes > 1 ? $"{timeSpan.Minutes} " + LblMinutes : LblAboutMinute;
                    }
                    else if (timeSpan <= TimeSpan.FromHours(24))
                    {
                        result = timeSpan.Hours > 1 ? $"{timeSpan.Hours} " + LblHours : LblAboutHour;
                    }
                    else if (timeSpan <= TimeSpan.FromDays(30))
                    {
                        result = timeSpan.Days > 1 ? $"{timeSpan.Days} " + LblDays : LblYesterday;
                    }
                    else if (timeSpan <= TimeSpan.FromDays(365))
                    {
                        result = timeSpan.Days > 30 ? $"{timeSpan.Days / 30} " + LblMonth : LblAboutMonth;
                    }
                    else
                    {
                        result = timeSpan.Days > 365 ? $"{timeSpan.Days / 365} " + LblYear : LblAboutYear;
                    }

                    return result;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return dateTime.ToShortTimeString();
                }
            }

            //Functions Replace Time
            public static string ReplaceTime(string time)
            {
                if (time.Contains("hours ago") || time.Contains("hour ago"))
                {
                    time = time.Replace("hours ago", LblCutHours);
                    time = time.Replace("hour ago", LblCutHours);
                }
                else if (time.Contains("days ago") || time.Contains("day ago"))
                {
                    time = time.Replace("days ago", LblCutDays);
                    time = time.Replace("day ago", LblCutDays);
                }
                else if (time.Contains("month ago"))
                {
                    time = time.Replace("month ago", LblCutMonth);
                }
                else if (time.Contains("minutes ago") || time.Contains("minutes ago"))
                {
                    time = time.Replace("minutes ago", LblCutMinutes);
                }
                else if (time.Contains("seconds ago") || time.Contains("second ago"))
                {
                    time = time.Replace("seconds ago", LblCutSeconds);
                    time = time.Replace("second ago", LblCutSeconds);
                }
                else if (time.Contains("year ago") || time.Contains("years ago"))
                {
                    time = time.Replace("year ago", LblCutYear);
                    time = time.Replace("years ago", LblCutYear);
                }
                else if (time.Contains("yesterday"))
                {
                    time = time.Replace("yesterday", LblYesterday);
                }

                return time;
            }

            //convert a Unix timestamp to DateTime 
            public static DateTime UnixTimeStampToDateTime(int unixTimeStamp)
            {
                // Unix timestamp is seconds past epoch
                DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
                return dtDateTime;
            } 

            #region To days

            public static double ConvertMillisecondsToDays(double milliseconds)
            {
                return TimeSpan.FromMilliseconds(milliseconds).TotalDays;
            }

            public static double ConvertSecondsToDays(double seconds)
            {
                return TimeSpan.FromSeconds(seconds).TotalDays;
            }

            public static double ConvertMinutesToDays(double minutes)
            {
                return TimeSpan.FromMinutes(minutes).TotalDays;
            }

            public static double ConvertHoursToDays(double hours)
            {
                return TimeSpan.FromHours(hours).TotalDays;
            }

            #endregion

            #region To hours

            public static double ConvertMillisecondsToHours(double milliseconds)
            {
                return TimeSpan.FromMilliseconds(milliseconds).TotalHours;
            }

            public static double ConvertSecondsToHours(double seconds)
            {
                return TimeSpan.FromSeconds(seconds).TotalHours;
            }

            public static double ConvertMinutesToHours(double minutes)
            {
                return TimeSpan.FromMinutes(minutes).TotalHours;
            }

            public static double ConvertDaysToHours(double days)
            {
                return TimeSpan.FromHours(days).TotalHours;
            }

            #endregion

            #region To minutes

            public static double ConvertMillisecondsToMinutes(double milliseconds)
            {
                return TimeSpan.FromMilliseconds(milliseconds).Minutes;
            }

            public static double ConvertSecondsToMinutes(double seconds)
            {
                return TimeSpan.FromSeconds(seconds).TotalMinutes;
            }

            public static double ConvertHoursToMinutes(double hours)
            {
                return TimeSpan.FromHours(hours).TotalMinutes;
            }

            public static double ConvertDaysToMinutes(double days)
            {
                return TimeSpan.FromDays(days).TotalMinutes;
            }

            #endregion

            #region To seconds

            public static double ConvertMillisecondsToSeconds(double milliseconds)
            {
                return TimeSpan.FromMilliseconds(milliseconds).Seconds;
            }

            public static double ConvertMinutesToSeconds(double minutes)
            {
                return TimeSpan.FromMinutes(minutes).TotalSeconds;
            }

            public static double ConvertHoursToSeconds(double hours)
            {
                return TimeSpan.FromHours(hours).TotalSeconds;
            }

            public static double ConvertDaysToSeconds(double days)
            {
                return TimeSpan.FromDays(days).TotalSeconds;
            }

            #endregion

            #region To milliseconds

            public static double ConvertSecondsToMilliseconds(double seconds)
            {
                return TimeSpan.FromSeconds(seconds).TotalMilliseconds;
            }

            public static double ConvertMinutesToMilliseconds(double minutes)
            {
                return TimeSpan.FromMinutes(minutes).TotalMilliseconds;
            }

            public static double ConvertHoursToMilliseconds(double hours)
            {
                return TimeSpan.FromHours(hours).TotalMilliseconds;
            }

            public static double ConvertDaysToMilliseconds(double days)
            {
                return TimeSpan.FromDays(days).TotalMilliseconds;
            }

            #endregion

        }

        #endregion

        #region Dialog Popup

        public class DialogPopup
        {
            public enum MessageResult
            {
                NONE = 0,
                OK = 1,
                CANCEL = 2,
                ABORT = 3,
                RETRY = 4,
                IGNORE = 5,
                YES = 6,
                NO = 7
            }

            Activity mcontext;

            public DialogPopup(Activity activity) : base()
            {
                mcontext = activity;
            }

            public Task<MessageResult> ShowDialog(string Title, string Message, bool SetCancelable = false,
                bool SetInverseBackgroundForced = false, MessageResult PositiveButton = MessageResult.OK,
                MessageResult NegativeButton = MessageResult.NONE, MessageResult NeutralButton = MessageResult.NONE,
                int IconAttribute = Android.Resource.Attribute.AlertDialogIcon)
            {
                var tcs = new TaskCompletionSource<MessageResult>();

                var builder = new AlertDialog.Builder(mcontext, Resource.Style.AlertDialogCustom);
                //builder.SetIconAttribute(IconAttribute);
                builder.SetTitle(Title);
                builder.SetMessage(Message);
                builder.SetInverseBackgroundForced(SetInverseBackgroundForced);
                builder.SetCancelable(SetCancelable);

                builder.SetPositiveButton(
                    PositiveButton != MessageResult.NONE ? PositiveButton.ToString() : string.Empty,
                    (senderAlert, args) =>
                    {
                        tcs.SetResult(PositiveButton);
                    });
                builder.SetNegativeButton(
                    NegativeButton != MessageResult.NONE ? NegativeButton.ToString() : string.Empty,
                    delegate
                    {
                        tcs.SetResult(NegativeButton);
                    });
                builder.SetNeutralButton(
                    NeutralButton != MessageResult.NONE ? NeutralButton.ToString() : string.Empty,
                    delegate
                    {
                        tcs.SetResult(NeutralButton);
                    });

                builder.Show();

                return tcs.Task;
            }

            public static void InvokeAndShowDialog(Activity activity, string Title, string Message,string PositiveButtonstring)
            {
                try
                {
                    activity.RunOnUiThread(() =>
                    {
                        try
                        {
                            Android.Support.V7.App.AlertDialog.Builder builder;
                            builder = new Android.Support.V7.App.AlertDialog.Builder(activity, Resource.Style.AlertDialogCustom);
                            builder.SetTitle(Title);
                            builder.SetMessage(Message);
                            builder.SetCancelable(false);
                            builder.SetPositiveButton(PositiveButtonstring, delegate { builder.Dispose(); });
                            builder.Show();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }

                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e); 
                } 
            }
        }


        #endregion

        #region IApp

        public class IApp
        {
            public static void FullScreenApp(Activity context)
            {
                try
                {
                    if (AppSettings.EnableFullScreenApp)
                    {
                        View mContentView = context.Window.DecorView;
                        var uiOptions = (int)mContentView.SystemUiVisibility;
                        var newUiOptions = (int)uiOptions;

                        newUiOptions |= (int)SystemUiFlags.Fullscreen;
                        newUiOptions |= (int)SystemUiFlags.HideNavigation;
                        mContentView.SystemUiVisibility = (StatusBarVisibility)newUiOptions;

                        context.Window.AddFlags(WindowManagerFlags.Fullscreen);
                        //context.Window.RequestFeature(WindowFeatures.NoTitle);

                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }


            public static void OpenWebsiteUrl(Context context, string Website)
            {
                try
                {
                    var uri = Android.Net.Uri.Parse(Website);
                    var intent = new Intent(Intent.ActionView, uri);
                    intent.AddFlags(ActivityFlags.NewTask);
                    context.StartActivity(intent);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }

            public static void OpenbrowserUrl(Context context, String url)
            {
                try
                {
                    CustomTabsIntent.Builder builder = new CustomTabsIntent.Builder();
                    CustomTabsIntent customTabsIntent = builder.Build();
                    customTabsIntent.Intent.AddFlags(ActivityFlags.NewTask);
                    customTabsIntent.LaunchUrl(context, Android.Net.Uri.Parse(url));
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }

            public static void OpenAppByPackageName(Context context, string packageName, string userId = "")
            {
                try
                {
                    Intent intent;
                    Intent Chkintent = context.PackageManager.GetLaunchIntentForPackage(packageName);
                    if (Chkintent != null)
                    {
                        if (userId != "")
                        {
                            Intent launchIntent = context.PackageManager.GetLaunchIntentForPackage(packageName);
                            if (launchIntent != null)
                            {
                                launchIntent.AddFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                                launchIntent.PutExtra("UserID", userId);

                                launchIntent.AddFlags(ActivityFlags.SingleTop);
                                context.StartActivity(launchIntent);
                            }
                        }
                        else
                        {
                            intent = context.PackageManager.GetLaunchIntentForPackage(packageName);
                            intent.AddFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                            intent.AddFlags(ActivityFlags.SingleTop);
                            context.StartActivity(intent);
                        }
                    }
                    else
                    {
                        intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("market://details?id=" + packageName));
                        intent.AddFlags(ActivityFlags.NewTask);
                        context.StartActivity(intent);
                    }
                }
                catch (ActivityNotFoundException es)
                {
                    Console.WriteLine(es);
                    var intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("http://play.google.com/store/apps/details?id=" + packageName));
                    intent.AddFlags(ActivityFlags.NewTask);
                    context.StartActivity(intent);
                }
            }

            public static void ClearWebViewCache(Context context)
            {
                try
                {
                    WebView wv = new WebView(context);
                    // wv.ClearCache(true);

                    if (AppSettings.RenderPriorityFastPostLoad)
                    {
                        wv.Settings.SetRenderPriority(WebSettings.RenderPriority.High);
                        wv.Settings.SetAppCacheEnabled(true);
                        wv.Settings.EnableSmoothTransition();
                        wv.Settings.SetLayoutAlgorithm(WebSettings.LayoutAlgorithm.NarrowColumns);

                        if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
                            wv.SetLayerType(LayerType.Hardware, null);
                        else
                            wv.SetLayerType(LayerType.Software, null);
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }

            public static void SendSMS(Context context, string PhoneNumber, string textmessges)
            {
                try
                {
                    var smsUri = Android.Net.Uri.Parse("smsto:" + PhoneNumber);
                    var smsIntent = new Intent(Intent.ActionSendto, smsUri);
                    smsIntent.PutExtra("sms_body", textmessges);
                    smsIntent.AddFlags(ActivityFlags.NewTask);
                    context.StartActivity(smsIntent);

                    //Or use this code
                    // Android.Telephony.SmsManager.Default.SendTextMessage(item.PhoneNumber, null, "Hello Xamarin This is My Test SMS", null, null);

                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }

            public static void SaveContacts(Context context, string phonenumber, string name, string type)
            {
                try
                {
                    if (type == "1")
                    {
                        Intent intent = new Intent(ContactsContract.Intents.Insert.Action);
                        intent.SetType(ContactsContract.RawContacts.ContentType);
                        intent.PutExtra(ContactsContract.Intents.Insert.Phone, phonenumber);
                        intent.PutExtra(ContactsContract.Intents.Insert.Name, name);
                        //intent.PutExtra(ContactsContract.Intents.Insert.Email, "wael@test.com");  /* REVISAR ROEO*/
                        intent.PutExtra(ContactsContract.Intents.Insert.Email, "info@datznat.com");  /* REVISAR ROEO*/
                        context.StartActivity(intent);
                    }
                    else
                    {
                        var contactUri = Android.Net.Uri.Parse("tel:" + phonenumber);
                        Intent intent = new Intent(ContactsContract.Intents.ShowOrCreateContact, contactUri);
                        intent.PutExtra(ContactsContract.Intents.ExtraRecipientContactName, true);
                        context.StartActivity(intent);
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }


            //ROEO ENVIAR EMAIL METODO
            public static void SendEmail(Context context, string Email)
            {
                try
                {
                    var email = new Intent(Intent.ActionSend);
                    email.PutExtra(Intent.ExtraEmail, Email);
                    email.PutExtra(Intent.ExtraCc, Email);
                    email.PutExtra(Intent.ExtraSubject, "Hello Email");
                    email.PutExtra(Intent.ExtraText, " ");
                    email.SetType("message/rfc822");
                    context.StartActivity(email);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }

            public static void SendPhoneCall(Context context, string phoneNumber)
            {
                try
                {
                    var urlNumber = Android.Net.Uri.Parse("tel:" + phoneNumber);
                    var callIntent = new Intent(Intent.ActionCall);
                    callIntent.SetData(urlNumber);
                    callIntent.AddFlags(ActivityFlags.NewTask);
                    context.StartActivity(callIntent);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }

            public static void Restart_App(Context context, string packageName)
            {
                try
                {
                    Intent intent =
                        Application.Context.PackageManager.GetLaunchIntentForPackage(packageName);
                    // If not NULL run the app, if not, take the user to the app store
                    if (intent != null)
                    {
                        intent.AddFlags(ActivityFlags.NewTask);
                        context.StartActivity(intent);
                    }
                    else
                    {

                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }

            public static void Close_App()
            {
                try
                {
                    Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
                }
            }


            public static string GetKeyHashesConfigured(Context applicationContext)
            {
                try
                {
                    PackageInfo info = applicationContext.PackageManager.GetPackageInfo(applicationContext.PackageName, PackageInfoFlags.Signatures);
                    foreach (var signature in info.Signatures)
                    {
                        MessageDigest md = MessageDigest.GetInstance("SHA");
                        md.Update(signature.ToByteArray());
                        var dd = signature.ToString();
                        var a = md.Algorithm;
                        var ass = md.DigestLength;
                        var asss = md.ToString();
                        string returnValue = Convert.ToBase64String(md.Digest());

                        Log.Debug("KeyHash:", returnValue);
                        return returnValue;
                    }
                }
                catch (PackageManager.NameNotFoundException e)
                {
                    Console.WriteLine(e);
                    return "";
                }
                catch (NoSuchAlgorithmException e)
                {
                    Console.WriteLine(e);
                    return "";
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return "";
                }
                return "";
            }


            public static string GetValueFromManifest(Context applicationContext, string nameValue)
            {
                try
                {
                    ApplicationInfo ai = applicationContext.PackageManager.GetApplicationInfo(applicationContext.PackageName, PackageInfoFlags.MetaData);
                    Bundle bundle = ai.MetaData;
                    string myApiKey = bundle.GetString(nameValue);
                    return myApiKey;
                }
                catch (PackageManager.NameNotFoundException e)
                {
                    string error = "Failed to load meta-data, NameNotFound: " + e.Message;
                    Console.WriteLine(error);
                    Console.WriteLine(e.InnerException);
                }
                catch (NullPointerException e)
                {
                    Console.WriteLine(e.InnerException);
                    Console.WriteLine("Failed to load meta-data, NullPointer: " + e.Message);
                }

                return "";
            }
        }

        #endregion

        #region MimeType

        public class MimeType
        {
            private static readonly byte[] BMP = { 66, 77 };
            private static readonly byte[] DOC = { 208, 207, 17, 224, 161, 177, 26, 225 };
            private static readonly byte[] EXE_DLL = { 77, 90 };
            private static readonly byte[] GIF = { 71, 73, 70, 56 };
            private static readonly byte[] ICO = { 0, 0, 1, 0 };
            private static readonly byte[] JPG = { 255, 216, 255 };
            private static readonly byte[] MP3 = { 255, 251, 48 };
            private static readonly byte[] OGG = { 79, 103, 103, 83, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0 };
            private static readonly byte[] PDF = { 37, 80, 68, 70, 45, 49, 46 };
            private static readonly byte[] PNG = { 137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 13, 73, 72, 68, 82 };
            private static readonly byte[] RAR = { 82, 97, 114, 33, 26, 7, 0 };
            private static readonly byte[] SWF = { 70, 87, 83 };
            private static readonly byte[] TIFF = { 73, 73, 42, 0 };
            private static readonly byte[] TORRENT = { 100, 56, 58, 97, 110, 110, 111, 117, 110, 99, 101 };
            private static readonly byte[] TTF = { 0, 1, 0, 0, 0 };
            private static readonly byte[] WAV_AVI = { 82, 73, 70, 70 };

            private static readonly byte[] WMV_WMA =
                {48, 38, 178, 117, 142, 102, 207, 17, 166, 217, 0, 170, 0, 98, 206, 108};

            private static readonly byte[] ZIP_DOCX = { 80, 75, 3, 4 };

            public static string GetMimeType(byte[] file, string fileName)
            {

                string mime = "text/plain"; //DEFAULT UNKNOWN MIME TYPE

                //Ensure that the filename isn't empty or null
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    return mime;
                }

                //Get the file extension
                string extension = Path.GetExtension(fileName) == null
                    ? string.Empty
                    : Path.GetExtension(fileName).ToUpper();

                //Get the MIME Type
                if (file.Take(2).SequenceEqual(BMP))
                {
                    mime = "image/bmp";
                }
                else if (file.Take(8).SequenceEqual(DOC))
                {
                    mime = "application/msword";
                }
                else if (file.Take(2).SequenceEqual(EXE_DLL))
                {
                    mime = "application/x-msdownload"; //both use same mime type
                }
                else if (file.Take(4).SequenceEqual(GIF))
                {
                    mime = "image/gif";
                }
                else if (file.Take(4).SequenceEqual(ICO))
                {
                    mime = "image/x-icon";
                }
                else if (file.Take(3).SequenceEqual(JPG))
                {
                    mime = "image/jpeg";
                }
                else if (file.Take(3).SequenceEqual(MP3))
                {
                    mime = "audio/mpeg";
                }
                else if (file.Take(14).SequenceEqual(OGG))
                {
                    if (extension == ".OGX")
                    {
                        mime = "application/ogg";
                    }
                    else if (extension == ".OGA")
                    {
                        mime = "audio/ogg";
                    }
                    else
                    {
                        mime = "video/ogg";
                    }
                }
                else if (file.Take(7).SequenceEqual(PDF))
                {
                    mime = "application/pdf";
                }
                else if (file.Take(16).SequenceEqual(PNG))
                {
                    mime = "image/png";
                }
                else if (file.Take(7).SequenceEqual(RAR))
                {
                    mime = "application/x-rar-compressed";
                }
                else if (file.Take(3).SequenceEqual(SWF))
                {
                    mime = "application/x-shockwave-flash";
                }
                else if (file.Take(4).SequenceEqual(TIFF))
                {
                    mime = "image/tiff";
                }
                else if (file.Take(11).SequenceEqual(TORRENT))
                {
                    mime = "application/x-bittorrent";
                }
                else if (file.Take(5).SequenceEqual(TTF))
                {
                    mime = "application/x-font-ttf";
                }
                else if (file.Take(4).SequenceEqual(WAV_AVI))
                {
                    mime = extension == ".AVI" ? "video/x-msvideo" : "audio/x-wav";
                }
                else if (file.Take(16).SequenceEqual(WMV_WMA))
                {
                    mime = extension == ".WMA" ? "audio/x-ms-wma" : "video/x-ms-wmv";
                }
                else if (file.Take(4).SequenceEqual(ZIP_DOCX))
                {
                    mime = extension == ".DOCX"
                        ? "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
                        : "application/x-zip-compressed";
                }

                return mime;
            }
        }

        #endregion

        #region LocalNotification

        public class LocalNotification
        {
            public static string NOTIFICATION_ID = "NOTIFICATION_ID";
            public static string CHANNEL_ID = "Channel_2018";
            public static WebClient WebClient = new WebClient();

            public static void CreateLocalNotification(string notification_Id, string Title, string ContentText)
            {
                try
                {
                    if (AppSettings.ShowNotification)
                    {
                        // Instantiate the builder and set notification elements:
                        Notification.Builder builder = new Notification.Builder(Application.Context)
                            .SetContentTitle(Title) //Sample Notification
                            .SetContentText(ContentText) //Hello World! This is my first notification!
                            .SetStyle(new Notification.BigTextStyle().BigText(ContentText))
                            .SetSmallIcon(Resource.Drawable.icon);

                        builder.SetDefaults(NotificationDefaults.Sound | NotificationDefaults.Vibrate);

                        // Build the notification:
                        Notification notification = builder.Build();

                        // Get the notification manager:
                        NotificationManager notificationManager =
                            Application.Context.GetSystemService(Context.NotificationService) as NotificationManager;

                        // Publish the notification:
                        var notificationId = Convert.ToInt32(notification_Id);

                        notificationManager.Notify(notificationId, notification);
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }

            public static void Create_Progress_Notification(string Notification_Id, string Notification_Title)
            {
                try
                {
                    if (AppSettings.ShowNotification)
                    {
                        NotificationManagerCompat notificationManager = NotificationManagerCompat.From(Application.Context);
                        var notificationId = Convert.ToInt32(Notification_Id);

                        if (Build.VERSION.SdkInt >= Build.VERSION_CODES.O)
                        {
                            // Create the NotificationChannel, but only on API 26+ because
                            // the NotificationChannel class is new and not in the support library
                            var channel = new NotificationChannel(CHANNEL_ID, "Video_Notifciation_Channel_1", NotificationImportance.High);
                            channel.Description = "";
                            channel.EnableVibration(true);
                            channel.LockscreenVisibility = NotificationVisibility.Public;
                            if (notificationManager != null)
                            {
                                //notificationManager.CreateNotificationChannel(channel);
                            }
                        }

                        var NotificationBroadcasterAction = new Intent(Application.Context, typeof(NotificationBroadcasterCloser));
                        NotificationBroadcasterAction.PutExtra(NOTIFICATION_ID, notificationId);
                        NotificationBroadcasterAction.PutExtra("type", "dismiss");
                        PendingIntent cancelIntent = PendingIntent.GetBroadcast(Application.Context, notificationId, NotificationBroadcasterAction, 0);

                        NotificationCompat.Builder builder = new NotificationCompat.Builder(Application.Context)
                            .SetContentTitle(Notification_Title).SetOngoing(true).SetProgress(100, 0, false)
                            .SetSmallIcon(Resource.Drawable.icon);
                        builder.SetPriority(NotificationCompat.PriorityMax);
                        //.AddAction(Resource.Drawable.icon, "Dismiss", cancelIntent)

                        Notification notification = builder.Build();

                        try
                        {
                            string Url = "http://clips.vorwaerts-gmbh.de/big_buck_bunny.mp4";
                            string Filename = Url.Split('/').Last();
                            string filePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "mmSavedVideos");
                            string MediaFile = filePath + "/" + Filename;

                            if (!File.Exists(MediaFile))
                            {
                                WebClient = new WebClient();

                                if (!Directory.Exists(filePath))
                                    Directory.CreateDirectory(filePath);

                                WebClient.DownloadFileAsync(new System.Uri(Url), MediaFile);

                                WebClient.DownloadProgressChanged += (sender, ep) => {

                                    double bytesIn = double.Parse(ep.BytesReceived.ToString());
                                    double totalBytes = double.Parse(ep.TotalBytesToReceive.ToString());
                                    double percentage = bytesIn / totalBytes * 100;
                                    var Presint = Convert.ToInt32(percentage);

                                    new Thread(new ThreadStart(() =>
                                    {
                                        builder.SetProgress(100, Presint, false);
                                        notificationManager.Notify(Convert.ToInt32(Notification_Id), builder.Build());

                                    })).Start();
                                };
                                WebClient.DownloadDataCompleted += (s, e) =>
                                {
                                    try
                                    {
                                        builder.SetContentText("Download complete")
                                            .SetProgress(0, 0, false);
                                        notificationManager.Notify(notificationId, builder.Build());
                                        notificationManager.Cancel(notificationId);
                                        File.WriteAllBytes(MediaFile, e.Result);
                                    }
                                    catch (Exception exception)
                                    {
                                        Console.WriteLine(exception);
                                    }
                                };
                            }
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception);
                        }

                        notificationManager.Notify(notificationId, notification);
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }

            [BroadcastReceiver()]
            [IntentFilter(new[] { "select.notif.id" })]
            public class NotificationBroadcasterCloser : BroadcastReceiver
            {
                public override void OnReceive(Context context, Intent intent)
                {
                    try
                    {
                        int notificationId = intent.GetIntExtra("NOTIFICATION_ID", 0);

                        NotificationManager notifyMgr = (NotificationManager)Application.Context.GetSystemService(Context.NotificationService);
                        notifyMgr.Cancel(notificationId);

                        if (intent.GetStringExtra("action") == "dismiss")
                        {
                            WebClient.CancelAsync();
                            notifyMgr.CancelAll();
                        }
                        else
                        {

                        }
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                    }
                }
            }

            [Service]
            public class BackgroundRunner : IntentService
            {
                protected override void OnHandleIntent(Intent intent)
                {

                }
            }
        }

        #endregion

        #region AttachmentFiles

        public class AttachmentFiles
        {
            public static string GetActualPathFromFile(Context context, Android.Net.Uri uri)
            {
                bool isKitKat = Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat;

                if (isKitKat && DocumentsContract.IsDocumentUri(context, uri))
                {
                    // ExternalStorageProvider
                    if (isExternalStorageDocument(uri))
                    {
                        string docId = DocumentsContract.GetDocumentId(uri);

                        char[] chars = { ':' };
                        string[] split = docId.Split(chars);
                        string type = split[0];

                        if ("primary".Equals(type, StringComparison.OrdinalIgnoreCase))
                        {
                            return Environment.ExternalStorageDirectory + "/" + split[1];
                        }
                    }
                    // DownloadsProvider
                    else if (isDownloadsDocument(uri))
                    {
                        string id = DocumentsContract.GetDocumentId(uri);

                        Android.Net.Uri contentUri = ContentUris.WithAppendedId(Android.Net.Uri.Parse("content://downloads/public_downloads"), long.Parse(id));

                        //System.Diagnostics.Debug.WriteLine(contentUri.ToString());

                        return getDataColumn(context, contentUri, null, null);
                    }
                    // MediaProvider
                    else if (isMediaDocument(uri))
                    {
                        String docId = DocumentsContract.GetDocumentId(uri);

                        char[] chars = { ':' };
                        String[] split = docId.Split(chars);

                        String type = split[0];

                        Android.Net.Uri contentUri = null;
                        if ("image".Equals(type))
                        {
                            contentUri = MediaStore.Images.Media.ExternalContentUri;
                        }
                        else if ("video".Equals(type))
                        {
                            contentUri = MediaStore.Video.Media.ExternalContentUri;
                        }
                        else if ("audio".Equals(type))
                        {
                            contentUri = MediaStore.Audio.Media.ExternalContentUri;
                        }

                        String selection = "_id=?";
                        String[] selectionArgs = new String[]
                        {
                            split[1]
                        };

                        return getDataColumn(context, contentUri, selection, selectionArgs);
                    }
                }
                // MediaStore (and general)
                else if ("content".Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase))
                {

                    // Return the remote address
                    if (isGooglePhotosUri(uri))
                        return uri.LastPathSegment;

                    return getDataColumn(context, uri, null, null);
                }
                // File
                else if ("file".Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase))
                {
                    return uri.Path;
                }

                return null;
            }

            public static String getDataColumn(Context context, Android.Net.Uri uri, String selection, String[] selectionArgs)
            {
                ICursor cursor = null;
                String column = "_data";
                String[] projection =
                {
                    column
                };

                try
                {
                    cursor = context.ContentResolver.Query(uri, projection, selection, selectionArgs, null);
                    if (cursor != null && cursor.MoveToFirst())
                    {
                        int index = cursor.GetColumnIndexOrThrow(column);
                        return cursor.GetString(index);
                    }
                }
                finally
                {
                    cursor?.Close();
                }
                return null;
            }

            //Whether the Uri authority is ExternalStorageProvider.
            public static bool isExternalStorageDocument(Android.Net.Uri uri)
            {
                return "com.android.externalstorage.documents".Equals(uri.Authority);
            }

            //Whether the Uri authority is DownloadsProvider.
            public static bool isDownloadsDocument(Android.Net.Uri uri)
            {
                return "com.android.providers.downloads.documents".Equals(uri.Authority);
            }

            //Whether the Uri authority is MediaProvider.
            public static bool isMediaDocument(Android.Net.Uri uri)
            {
                return "com.android.providers.media.documents".Equals(uri.Authority);
            }

            //Whether the Uri authority is Google Photos.
            public static bool isGooglePhotosUri(Android.Net.Uri uri)
            {
                return "com.google.android.apps.photos.content".Equals(uri.Authority);
            }

            // Functions Check File Extension */Audio, Image, Video\*
            public static string Check_FileExtension(string filename)
            {
                if (filename.EndsWith("mp3") || filename.EndsWith("MP3"))
                {
                    return "Audio";
                }
                else if (filename.EndsWith("aac") || filename.EndsWith("AAC"))
                {
                    return "Audio";
                }
                else if (filename.EndsWith("aiff") || filename.EndsWith("AIFF"))
                {
                    return "Audio";
                }
                else if (filename.EndsWith("amr") || filename.EndsWith("AMR"))
                {
                    return "Audio";
                }
                else if (filename.EndsWith("ape") || filename.EndsWith("APE"))
                {
                    return "Audio";
                }
                else if (filename.EndsWith("arf") || filename.EndsWith("ARF"))
                {
                    return "Audio";
                }
                else if (filename.EndsWith("asf") || filename.EndsWith("ASF"))
                {
                    return "Audio";
                }
                else if (filename.EndsWith("m4a") || filename.EndsWith("M4A"))
                {
                    return "Audio";
                }
                else if (filename.EndsWith("m4b") || filename.EndsWith("M4B"))
                {
                    return "Audio";
                }
                else if (filename.EndsWith("m4p") || filename.EndsWith("M4A"))
                {
                    return "Audio";
                }
                else if (filename.EndsWith("3ga") || filename.EndsWith("3GA"))
                {
                    return "Audio";
                }
                else if (filename.EndsWith("ogg") || filename.EndsWith("OGG"))
                {
                    return "Audio";
                }
                else if (filename.EndsWith("wav") || filename.EndsWith("WAV"))
                {
                    return "Audio";
                }
                else if (filename.EndsWith("wma") || filename.EndsWith("WMA"))
                {
                    return "Audio";
                }
                else if (filename.EndsWith("wpl") || filename.EndsWith("WPL"))
                {
                    return "Audio";
                }
                else if (filename.EndsWith("ani") || filename.EndsWith("ANI"))
                {
                    return "Image";
                }
                else if (filename.EndsWith("bmp") || filename.EndsWith("BMP"))
                {
                    return "Image";
                }
                else if (filename.EndsWith("cal") || filename.EndsWith("CAL"))
                {
                    return "Image";
                }
                else if (filename.EndsWith("fax") || filename.EndsWith("FAX"))
                {
                    return "Image";
                }
                else if (filename.EndsWith("gif") || filename.EndsWith("GIF"))
                {
                    return "Image";
                }
                else if (filename.EndsWith("img") || filename.EndsWith("IMG"))
                {
                    return "Image";
                }
                else if (filename.EndsWith("jbg") || filename.EndsWith("JBG"))
                {
                    return "Image";
                }
                else if (filename.EndsWith("jpeg") || filename.EndsWith("JPEG"))
                {
                    return "Image";
                }
                else if (filename.EndsWith("jpe") || filename.EndsWith("JPE"))
                {
                    return "Image";
                }
                else if (filename.EndsWith("jpg") || filename.EndsWith("JPG"))
                {
                    return "Image";
                }
                else if (filename.EndsWith("mac") || filename.EndsWith("MAC"))
                {
                    return "Image";
                }
                else if (filename.EndsWith("pbm") || filename.EndsWith("PBM"))
                {
                    return "Image";
                }
                else if (filename.EndsWith("pcd") || filename.EndsWith("PCD"))
                {
                    return "Image";
                }
                else if (filename.EndsWith("pcx") || filename.EndsWith("PCX"))
                {
                    return "Image";
                }
                else if (filename.EndsWith("pct") || filename.EndsWith("PCT"))
                {
                    return "Image";
                }
                else if (filename.EndsWith("pgm") || filename.EndsWith("PGM"))
                {
                    return "Image";
                }
                else if (filename.EndsWith("png") || filename.EndsWith("PNG"))
                {
                    return "Image";
                }
                else if (filename.EndsWith("ppm") || filename.EndsWith("PPM"))
                {
                    return "Image";
                }
                else if (filename.EndsWith("psd") || filename.EndsWith("PSD"))
                {
                    return "Image";
                }
                else if (filename.EndsWith("ras") || filename.EndsWith("RAS"))
                {
                    return "Image";
                }
                else if (filename.EndsWith("tga") || filename.EndsWith("TGA"))
                {
                    return "Image";
                }
                else if (filename.EndsWith("tiff") || filename.EndsWith("TIFF"))
                {
                    return "Image";
                }
                else if (filename.EndsWith("wmf") || filename.EndsWith("WMF"))
                {
                    return "Image";
                }
                else if (filename.EndsWith("avi") || filename.EndsWith("AVI"))
                {
                    return "Video";
                }
                else if (filename.EndsWith("asf") || filename.EndsWith("ASF"))
                {
                    return "Video";
                }
                else if (filename.EndsWith("mov") || filename.EndsWith("MOV"))
                {
                    return "Video";
                }
                else if (filename.EndsWith("qt") || filename.EndsWith("QT"))
                {
                    return "Video";
                }
                else if (filename.EndsWith("avchd") || filename.EndsWith("AVCHD"))
                {
                    return "Video";
                }
                else if (filename.EndsWith("flv") || filename.EndsWith("FLV"))
                {
                    return "Video";
                }
                else if (filename.EndsWith("swf") || filename.EndsWith("SWF"))
                {
                    return "Video";
                }
                else if (filename.EndsWith("mpg") || filename.EndsWith("MPG"))
                {
                    return "Video";
                }
                else if (filename.EndsWith("mpeg") || filename.EndsWith("MPEG"))
                {
                    return "Video";
                }
                else if (filename.EndsWith("wmv") || filename.EndsWith("WMV"))
                {
                    return "Video";
                }
                else if (filename.EndsWith("mpg-4") || filename.EndsWith("MPEG-4"))
                {
                    return "Video";
                }
                else if (filename.EndsWith("mp4") || filename.EndsWith("MP4"))
                {
                    return "Video";
                }
                else if (filename.EndsWith("h.264") || filename.EndsWith("H.264"))
                {
                    return "Video";
                }
                else if (filename.EndsWith("divx") || filename.EndsWith("DivX"))
                {
                    return "Video";
                }
                else if (filename.EndsWith("mkv") || filename.EndsWith("MKV"))
                {
                    return "Video";
                }
                else if (filename.EndsWith("rar") || filename.EndsWith("RAR"))
                {
                    return "File";
                }
                else if (filename.EndsWith("zip") || filename.EndsWith("ZIP"))
                {
                    return "File";
                }
                else if (filename.EndsWith("txt") || filename.EndsWith("TXT"))
                {
                    return "File";
                }
                else if (filename.EndsWith("docx") || filename.EndsWith("DOCX"))
                {
                    return "File";
                }
                else if (filename.EndsWith("doc") || filename.EndsWith("DOC"))
                {
                    return "File";
                }
                else if (filename.EndsWith("pdf") || filename.EndsWith("PDF"))
                {
                    return "File";
                }
                else
                {
                    return "Forbidden";
                }
            }
        }


        #endregion

        #region IPath URL

        public class IPath
        {
            public static string PersonalFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            public static string AndroidDcimFolder = Environment.GetExternalStoragePublicDirectory(Environment.DirectoryDcim).AbsolutePath;

            //DcimFolder 
            public static string FolderDcimMyApp = AndroidDcimFolder + "/" + AppSettings.ApplicationName + "/";
            public static string FolderDcimImage = FolderDcimMyApp + "/Images/";
            public static string FolderDcimVideo = FolderDcimMyApp + "/Video/";
            public static string FolderDcimStory = FolderDcimMyApp + "/Story/";
            public static string FolderDcimSound = FolderDcimMyApp + "/Sound/";
            public static string FolderDcimFile = FolderDcimMyApp + "/File/";
            public static string FolderDcimGif = FolderDcimImage + "/Gif/";
            public static string FolderDcimSticker = FolderDcimImage + "/Sticker/";
            public static string FolderDcimPage = FolderDcimImage + "/Page/";
            public static string FolderDcimGroup = FolderDcimImage + "/Group/";
            public static string FolderDcimEvent = FolderDcimImage + "/Event/";
            public static string FolderDcimMovie = FolderDcimVideo + "/Movie/";
            public static string FolderDcimArticles = FolderDcimImage + "/Articles/";
            public static string FolderDcimMarket = FolderDcimImage + "/Market/";
            public static string FolderDcimPost = FolderDcimMyApp + "/Post/";
            public static string FolderDcimNiceArt = FolderDcimMyApp + "/Editor/";

            //Disk
            public static string FolderDiskMyApp = PersonalFolder + "/" + AppSettings.ApplicationName + "/";
            public static string FolderDiskImage = FolderDiskMyApp + "/Images/";
            public static string FolderDiskVideo = FolderDiskMyApp + "/Video/";
            public static string FolderDiskStory = FolderDiskMyApp + "/Story/";
            public static string FolderDiskSound = FolderDiskMyApp + "/Sound/";
            public static string FolderDiskFile = FolderDiskMyApp + "/File/";
            public static string FolderDiskGif = FolderDiskMyApp + "/Gif/";
            public static string FolderDiskSticker = FolderDiskMyApp + "/Sticker/";
            public static string FolderDiskPage = FolderDiskImage + "/Page/";
            public static string FolderDiskGroup = FolderDiskImage + "/Group/";
            public static string FolderDiskEvent = FolderDiskImage + "/Event/";
            public static string FolderDiskMovie = FolderDiskVideo + "/Movie/";
            public static string FolderDiskArticles = FolderDiskImage + "/Articles/";
            public static string FolderDiskMarket = FolderDiskImage + "/Market/";
            public static string FolderDiskPost = FolderDiskMyApp + "/Post/";
            public static string FolderDiskNiceArt = FolderDiskMyApp + "/Editor/";

            public static void Chack_MyFolder()
            {
                try
                {
                    if (!Directory.Exists(FolderDcimMyApp))
                        Directory.CreateDirectory(FolderDcimMyApp);

                    if (!Directory.Exists(FolderDiskMyApp))
                        Directory.CreateDirectory(FolderDiskMyApp);

                    if (!Directory.Exists(FolderDcimImage))
                        Directory.CreateDirectory(FolderDcimImage);

                    if (!Directory.Exists(FolderDcimVideo))
                        Directory.CreateDirectory(FolderDcimVideo);

                    //if (!Directory.Exists(FolderDcimStory))
                    //    Directory.CreateDirectory(FolderDcimStory);

                    //if (!Directory.Exists(FolderDcimSound))
                    //    Directory.CreateDirectory(FolderDcimSound);

                    //if (!Directory.Exists(FolderDcimFile))
                    //    Directory.CreateDirectory(FolderDcimFile);

                    if (!Directory.Exists(FolderDcimGif))
                        Directory.CreateDirectory(FolderDcimGif);

                    if (!Directory.Exists(FolderDcimSticker))
                        Directory.CreateDirectory(FolderDcimSticker);

                    //if (!Directory.Exists(FolderDcimPage))
                    //    Directory.CreateDirectory(FolderDcimPage);

                    //if (!Directory.Exists(FolderDcimGroup))
                    //    Directory.CreateDirectory(FolderDcimGroup);

                    //if (!Directory.Exists(FolderDcimEvent))
                    //    Directory.CreateDirectory(FolderDcimEvent);

                    //if (!Directory.Exists(FolderDcimMovie))
                    //    Directory.CreateDirectory(FolderDcimMovie);

                    //if (!Directory.Exists(FolderDcimArticles))
                    //    Directory.CreateDirectory(FolderDcimArticles);

                    //if (!Directory.Exists(FolderDcimMarket))
                    //    Directory.CreateDirectory(FolderDcimMarket);

                    //if (!Directory.Exists(FolderDcimPost))
                    //    Directory.CreateDirectory(FolderDcimPost);

                    //if (!Directory.Exists(FolderDcimNiceArt))
                    //    Directory.CreateDirectory(FolderDcimNiceArt);

                    //================================================

                    if (!Directory.Exists(FolderDiskImage))
                        Directory.CreateDirectory(FolderDiskImage);

                    if (!Directory.Exists(FolderDiskVideo))
                        Directory.CreateDirectory(FolderDiskVideo);

                    //if (!Directory.Exists(FolderDiskStory))
                    //    Directory.CreateDirectory(FolderDiskStory);

                    //if (!Directory.Exists(FolderDiskFile))
                    //    Directory.CreateDirectory(FolderDiskFile);

                    //if (!Directory.Exists(FolderDiskSound))
                    //    Directory.CreateDirectory(FolderDiskSound);

                    if (!Directory.Exists(FolderDiskGif))
                        Directory.CreateDirectory(FolderDiskGif);

                    if (!Directory.Exists(FolderDiskSticker))
                        Directory.CreateDirectory(FolderDiskSticker);

                    //if (!Directory.Exists(FolderDiskPage))
                    //    Directory.CreateDirectory(FolderDiskPage);

                    //if (!Directory.Exists(FolderDiskGroup))
                    //    Directory.CreateDirectory(FolderDiskGroup);

                    //if (!Directory.Exists(FolderDiskEvent))
                    //    Directory.CreateDirectory(FolderDiskEvent);

                    //if (!Directory.Exists(FolderDiskMovie))
                    //    Directory.CreateDirectory(FolderDiskMovie);

                    //if (!Directory.Exists(FolderDiskArticles))
                    //    Directory.CreateDirectory(FolderDiskArticles);

                    //if (!Directory.Exists(FolderDiskMarket))
                    //    Directory.CreateDirectory(FolderDiskMarket);

                    //if (!Directory.Exists(FolderDiskPost))
                    //    Directory.CreateDirectory(FolderDiskPost);

                    //if (!Directory.Exists(FolderDiskNiceArt))
                    //    Directory.CreateDirectory(FolderDiskNiceArt);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            public static void DeleteAll_MyFolder()
            {
                try
                {

                    //if (Directory.Exists(FolderDiskMyApp))
                    //    Directory.Delete(FolderDiskMyApp,true);

                    if (Directory.Exists(FolderDcimImage))
                        Directory.Delete(FolderDcimImage, true);

                    if (Directory.Exists(FolderDcimVideo))
                        Directory.Delete(FolderDcimVideo, true);

                    if (Directory.Exists(FolderDcimStory))
                        Directory.Delete(FolderDcimStory, true);

                    if (Directory.Exists(FolderDcimSound))
                        Directory.Delete(FolderDcimSound, true);

                    if (Directory.Exists(FolderDcimGif))
                        Directory.Delete(FolderDcimGif, true);

                    if (Directory.Exists(FolderDcimSticker))
                        Directory.Delete(FolderDcimSticker, true);

                    if (Directory.Exists(FolderDcimPage))
                        Directory.Delete(FolderDcimPage, true);

                    if (Directory.Exists(FolderDcimGroup))
                        Directory.Delete(FolderDcimGroup, true);

                    if (Directory.Exists(FolderDcimEvent))
                        Directory.Delete(FolderDcimEvent, true);

                    if (Directory.Exists(FolderDcimMovie))
                        Directory.Delete(FolderDcimMovie, true);

                    if (Directory.Exists(FolderDcimArticles))
                        Directory.Delete(FolderDcimArticles, true);

                    if (Directory.Exists(FolderDcimMarket))
                        Directory.Delete(FolderDcimMarket, true);

                    if (!Directory.Exists(FolderDcimNiceArt))
                        Directory.CreateDirectory(FolderDcimNiceArt);

                    //================================================

                    //if (Directory.Exists(FolderDcimMyApp))
                    //    Directory.Delete(FolderDcimMyApp,true);

                    if (Directory.Exists(FolderDiskImage))
                        Directory.Delete(FolderDiskImage, true);

                    if (Directory.Exists(FolderDiskVideo))
                        Directory.Delete(FolderDiskVideo, true);

                    if (Directory.Exists(FolderDiskStory))
                        Directory.Delete(FolderDiskStory, true);

                    if (Directory.Exists(FolderDiskSound))
                        Directory.Delete(FolderDiskSound, true);

                    if (Directory.Exists(FolderDiskGif))
                        Directory.Delete(FolderDiskGif, true);

                    if (Directory.Exists(FolderDiskSticker))
                        Directory.Delete(FolderDiskSticker, true);

                    if (Directory.Exists(FolderDiskPage))
                        Directory.Delete(FolderDiskPage, true);

                    if (Directory.Exists(FolderDiskGroup))
                        Directory.Delete(FolderDiskGroup, true);

                    if (Directory.Exists(FolderDiskEvent))
                        Directory.Delete(FolderDiskEvent, true);

                    if (Directory.Exists(FolderDiskMovie))
                        Directory.Delete(FolderDiskMovie, true);

                    if (Directory.Exists(FolderDiskArticles))
                        Directory.Delete(FolderDiskArticles, true);

                    if (Directory.Exists(FolderDiskMarket))
                        Directory.Delete(FolderDiskMarket, true);

                    if (!Directory.Exists(FolderDiskNiceArt))
                        Directory.Delete(FolderDiskNiceArt, true);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            public static void DeleteAll_MyFolderDisk()
            {
                try
                {

                    if (Directory.Exists(FolderDiskImage))
                        Directory.Delete(FolderDiskImage, true);

                    if (Directory.Exists(FolderDiskVideo))
                        Directory.Delete(FolderDiskVideo, true);

                    if (Directory.Exists(FolderDiskStory))
                        Directory.Delete(FolderDiskStory, true);

                    if (Directory.Exists(FolderDiskSound))
                        Directory.Delete(FolderDiskSound, true);

                    if (Directory.Exists(FolderDiskGif))
                        Directory.Delete(FolderDiskGif, true);

                    if (Directory.Exists(FolderDiskSticker))
                        Directory.Delete(FolderDiskSticker, true);

                    if (Directory.Exists(FolderDiskPage))
                        Directory.Delete(FolderDiskPage, true);

                    if (Directory.Exists(FolderDiskGroup))
                        Directory.Delete(FolderDiskGroup, true);

                    if (Directory.Exists(FolderDiskEvent))
                        Directory.Delete(FolderDiskEvent, true);

                    if (Directory.Exists(FolderDiskMovie))
                        Directory.Delete(FolderDiskMovie, true);

                    if (Directory.Exists(FolderDiskArticles))
                        Directory.Delete(FolderDiskArticles, true);

                    if (Directory.Exists(FolderDiskMarket))
                        Directory.Delete(FolderDiskMarket, true);

                    if (!Directory.Exists(FolderDiskNiceArt))
                        Directory.Delete(FolderDiskNiceArt, true);

                    if (Directory.Exists(FolderDiskMyApp))
                        Directory.Delete(FolderDiskMyApp, true);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }


        #endregion

        //########################## End Class IMethods Application Version 3.0 ##########################
    }
}