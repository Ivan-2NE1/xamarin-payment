using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Com.Luseen.Autolinklibrary;
using QuickDate.Helpers.CacheLoaders;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDateClient;
using QuickDateClient.Classes.Chat;
using Object = Java.Lang.Object;
using Path = System.IO.Path;

namespace QuickDate.Activities.Chat.Adapters
{
    public class UserMessagesAdapter : RecyclerView.Adapter , ListPreloader.IPreloadModelProvider
    {
        #region Variables Basic

        public MessagesBoxActivity ActivityContext;
        public ObservableCollection<GetChatConversationsObject.Messages> MessageList = new ObservableCollection<GetChatConversationsObject.Messages>();
        public event EventHandler<UserMessagesAdapterClickEventArgs> OnItemClick;
        public event EventHandler<UserMessagesAdapterClickEventArgs> OnItemLongClick;

        public SparseBooleanArray SelectedItems;
        public IOnClickListenerSelectedMessages ClickListener;
        public int CurrentSelectedIdx = -1;

        #endregion

        public UserMessagesAdapter(MessagesBoxActivity context)
        {
            try
            {
                ActivityContext = context;
                HasStableIds = true;
                SelectedItems = new SparseBooleanArray();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> 
                var itemView = MessageList[viewType];
                if (itemView != null)
                {
                    if (itemView.From == UserDetails.UserId && itemView.MessageType == "text")
                    {
                        View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Right_MS_view, parent, false);
                        TextViewHolder textViewHolder = new TextViewHolder(row, Click, LongClick, ActivityContext);
                        return textViewHolder;
                    }
                    else if (itemView.To == UserDetails.UserId && itemView.MessageType == "text")
                    {
                        View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Left_MS_view, parent, false);
                        TextViewHolder textViewHolder = new TextViewHolder(row, Click, LongClick, ActivityContext);
                        return textViewHolder;
                    }
                    else if (itemView.From == UserDetails.UserId &&  itemView.MessageType == "media" )
                    {
                        View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Right_MS_image, parent, false);
                        ImageViewHolder imageViewHolder = new ImageViewHolder(row);
                        return imageViewHolder;
                    }
                    else if (itemView.To == UserDetails.UserId && itemView.MessageType == "media")
                    {
                        View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Left_MS_image, parent, false);
                        ImageViewHolder imageViewHolder = new ImageViewHolder(row);
                        return imageViewHolder;
                    }
                    else if (itemView.From == UserDetails.UserId &&  itemView.MessageType == "sticker")
                    {
                        if (itemView.Sticker.Contains(".gif"))
                        {
                            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Right_MS_gif, parent, false);
                            GifViewHolder viewHolder = new GifViewHolder(row);
                            return viewHolder;
                        }
                        else
                        {
                            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Right_MS_sticker, parent, false);
                            StickerViewHolder stickerViewHolder = new StickerViewHolder(row);
                            return stickerViewHolder;
                        }
                    }
                    else if (itemView.To == UserDetails.UserId && itemView.MessageType == "sticker")
                    {
                        if (itemView.Sticker.Contains(".gif"))
                        {
                            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Left_MS_gif, parent, false);
                            GifViewHolder viewHolder = new GifViewHolder(row);
                            return viewHolder;
                        }
                        else
                        {
                            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Left_MS_sticker, parent, false);
                            StickerViewHolder stickerViewHolder = new StickerViewHolder(row);
                            return stickerViewHolder;
                        }
                    }

                    return null;
                }

                return null;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder vh, int position)
        {
            try
            {
                int type = GetItemViewType(position);
                var item = MessageList[type];
                if (item == null) return;
                switch (item.MessageType)
                {
                    case "text":
                    {
                        TextViewHolder holder = vh as TextViewHolder;
                        LoadTextOfChatItem(holder, position, item);
                        break;
                    }
                    case "media":
                    {
                        ImageViewHolder holder = vh as ImageViewHolder;
                        LoadImageOfChatItem(holder, position, item);
                        break;
                    }
                    case "sticker" when item.Sticker.Contains(".gif"):
                    {
                        GifViewHolder holder = vh as GifViewHolder;
                        LoadGifOfChatItem(holder, position, item);
                        break;
                    }
                    case "sticker":
                    {
                        StickerViewHolder holder = vh as StickerViewHolder;
                        LoadStickerOfChatItem(holder, position, item);
                        break;
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        public void SetOnClickListener(IOnClickListenerSelectedMessages onClickListener)
        {
            ClickListener = onClickListener;
        }

        #region Load Holder
         
        public void LoadTextOfChatItem(TextViewHolder holder, int position, GetChatConversationsObject.Messages item)
        {
            try
            {
                if (holder.Time.Text != item.CreatedAt)
                {
                    DateTime time = Convert.ToDateTime(item.CreatedAt); 
                    holder.Time.Text = time.ToShortTimeString();
                    holder.TextSanitizerAutoLink.Load(item.Text, item.Type);
                }

                holder.LytParent.Activated = SelectedItems.Get(position, false);

                holder.LytParent.Click += delegate
                {
                    try
                    {
                        if (ClickListener == null) return;

                        ClickListener.ItemClick(holder.LytParent, item, position);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                };

                holder.LytParent.LongClick += delegate
                {
                    try
                    {
                        if (ClickListener == null) return;

                        ClickListener.ItemLongClick(holder.LytParent, item, position);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                };

                ToggleCheckedBackground(holder, position);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void LoadImageOfChatItem(ImageViewHolder holder, int position, GetChatConversationsObject.Messages message)
        {
            try
            {
                string imageUrl = message.Media;
                string fileSavedPath;

                DateTime time = Convert.ToDateTime(message.CreatedAt);
                holder.Time.Text = time.ToShortTimeString();

                if (imageUrl.Contains("http://") || imageUrl.Contains("https://"))
                {
                    var fileName = imageUrl.Split('/').Last();
                    string imageFile = IMethods.MultiMedia.GetMediaFrom_Gallery(IMethods.IPath.FolderDcimImage, fileName);

                    if (imageFile == "File Dont Exists")
                    { 
                        GlideImageLoader.LoadImage(ActivityContext, "ImagePlacholder", holder.ImageView, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);

                        holder.LoadingProgressView.Indeterminate = false;
                        holder.LoadingProgressView.Visibility = ViewStates.Visible;

                        string filePath = Path.Combine(IMethods.IPath.FolderDcimMyApp);
                        string mediaFile = filePath + "/" + fileName;
                        fileSavedPath = mediaFile;

                        WebClient webClient = new WebClient();

                        webClient.DownloadDataAsync(new Uri(imageUrl));
                        webClient.DownloadProgressChanged += (sender, args) =>
                        {
                            var progress = args.ProgressPercentage;
                            // holder.LoadingProgressView.Progress = progress;
                        };

                        webClient.DownloadDataCompleted += (s, e) =>
                        {
                            try
                            {
                                if (!Directory.Exists(filePath))
                                    Directory.CreateDirectory(filePath);

                                File.WriteAllBytes(mediaFile, e.Result);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }

                            var mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                            mediaScanIntent.SetData(Android.Net.Uri.FromFile(new Java.IO.File(mediaFile)));
                            ActivityContext.SendBroadcast(mediaScanIntent);

                            GlideImageLoader.LoadImage(ActivityContext, imageUrl, holder.ImageView, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);

                            holder.LoadingProgressView.Indeterminate = false;
                            holder.LoadingProgressView.Visibility = ViewStates.Gone;
                        };
                    }
                    else
                    {
                        fileSavedPath = imageFile;

                        GlideImageLoader.LoadImage(ActivityContext, imageUrl, holder.ImageView, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);

                        holder.LoadingProgressView.Indeterminate = false;
                        holder.LoadingProgressView.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    fileSavedPath = imageUrl;

                    var url = Android.Net.Uri.FromFile(new Java.IO.File(imageUrl));

                    GlideImageLoader.LoadImage(ActivityContext, url.Path, holder.ImageView, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);

                    holder.LoadingProgressView.Indeterminate = false;
                    holder.LoadingProgressView.Visibility = ViewStates.Gone;
                }

                if (!holder.ImageView.HasOnClickListeners)
                {
                    holder.ImageView.Click += (sender, args) =>
                    {
                        try
                        {
                            string imageFile = IMethods.MultiMedia.CheckFileIfExits(fileSavedPath);

                            if (imageFile != "File Dont Exists")
                            {
                                Java.IO.File file2 = new Java.IO.File(fileSavedPath);
                                var photoUri = FileProvider.GetUriForFile(ActivityContext, ActivityContext.PackageName + ".fileprovider", file2);

                                Intent intent = new Intent();
                                intent.SetAction(Intent.ActionView);
                                intent.AddFlags(ActivityFlags.GrantReadUriPermission);
                                intent.SetDataAndType(photoUri, "image/*");
                                ActivityContext.StartActivity(intent);
                            }
                            else
                            {
                                Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_something_went_wrong), ToastLength.Long).Show();
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    };
                }
                 
                holder.LytParent.Activated = SelectedItems.Get(position, false);

                holder.LytParent.Click += delegate
                {
                    try
                    {
                        if (ClickListener == null) return;

                        ClickListener.ItemClick(holder.LytParent, message, position);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                };

                holder.LytParent.LongClick += delegate
                {
                    try
                    {
                        if (ClickListener == null) return;

                        ClickListener.ItemLongClick(holder.LytParent, message, position);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                };

                ToggleCheckedBackground(holder, position);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void LoadGifOfChatItem(GifViewHolder holder, int position, GetChatConversationsObject.Messages item)
        {
            try
            {
                // G_fixed_height_small_url, // UrlGif - view  >>  mediaFileName
                // G_fixed_height_small_mp4, //MediaGif - sent >>  media

                if (!string.IsNullOrEmpty(item.Sticker))
                    GlideImageLoader.LoadImage(ActivityContext, item.Sticker, holder.ImageGifView, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);

               // ImageServiceLoader.Load_Image(holder.ImageGifView, "ImagePlacholder.jpg", item.Sticker, 2);

                holder.LytParent.Activated = SelectedItems.Get(position, false);

                holder.LytParent.Click += delegate
                {
                    try
                    {
                        if (ClickListener == null) return;

                        ClickListener.ItemClick(holder.LytParent, item, position);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                };

                holder.LytParent.LongClick += delegate
                {
                    try
                    {
                        if (ClickListener == null) return;

                        ClickListener.ItemLongClick(holder.LytParent, item, position);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                };

                ToggleCheckedBackground(holder, position);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void LoadStickerOfChatItem(StickerViewHolder holder, int position, GetChatConversationsObject.Messages message)
        {
            try
            {
                string imageUrl = message.Sticker;
                string fileSavedPath;
                 
                DateTime time = Convert.ToDateTime(message.CreatedAt);
                holder.Time.Text = time.ToShortTimeString();

                if (imageUrl.Contains("http://") || imageUrl.Contains("https://"))
                {
                    var fileName = imageUrl.Split('_').Last();
                    string imageFile = IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskSticker, fileName);
                    if (imageFile == "File Dont Exists")
                    {
                        holder.LoadingProgressView.Indeterminate = false;
                        holder.LoadingProgressView.Visibility = ViewStates.Visible;

                        var url = imageUrl.Contains("media3.giphy.com/");
                        if (url)
                        {
                            imageUrl = imageUrl.Replace(Client.WebsiteUrl, "");
                        }

                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskSticker, imageUrl);

                        GlideImageLoader.LoadImage(ActivityContext,imageUrl, holder.ImageView, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);

                        holder.LoadingProgressView.Indeterminate = false;
                        holder.LoadingProgressView.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        fileSavedPath = imageFile;

                        GlideImageLoader.LoadImage(ActivityContext,fileSavedPath, holder.ImageView, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);

                        holder.LoadingProgressView.Indeterminate = false;
                        holder.LoadingProgressView.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    fileSavedPath = imageUrl;

                    GlideImageLoader.LoadImage(ActivityContext,fileSavedPath, holder.ImageView, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);

                    holder.LoadingProgressView.Indeterminate = false;
                    holder.LoadingProgressView.Visibility = ViewStates.Gone;
                }


                holder.LytParent.Activated = SelectedItems.Get(position, false);

                holder.LytParent.Click += delegate
                {
                    try
                    {
                        if (ClickListener == null) return;

                        ClickListener.ItemClick(holder.LytParent, message, position);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                };

                holder.LytParent.LongClick += delegate
                {
                    try
                    {
                        if (ClickListener == null) return;

                        ClickListener.ItemLongClick(holder.LytParent, message, position);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                };

                ToggleCheckedBackground(holder, position);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        public override int ItemCount
        {
            get
            {
                if (MessageList != null)
                    return MessageList.Count;
                return 0;
            }
        }
       
        public GetChatConversationsObject.Messages GetItem(int position)
        {
            return MessageList[position];
        }

        public override long GetItemId(int position)
        {
            try
            {
                return MessageList[position].Id;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return 0;
            }
        }

        public override int GetItemViewType(int position)
        {
            try
            {
                return position;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return 0;
            }
        }

        private void Click(UserMessagesAdapterClickEventArgs args)
        {
            OnItemClick?.Invoke(this, args);
        }

        private void LongClick(UserMessagesAdapterClickEventArgs args)
        {
            OnItemLongClick?.Invoke(this, args);
        }

        #region Toolbar & Selected
        
        private void ToggleCheckedBackground(dynamic holder, int position)
        {
            try
            {
                if (SelectedItems.Get(position, false))
                {
                    holder.MainView.SetBackgroundColor(Color.LightBlue);
                    if (CurrentSelectedIdx == position) ResetCurrentItems();
                }
                else
                {
                    holder.MainView.SetBackgroundColor(Color.Transparent);
                    if (CurrentSelectedIdx == position) ResetCurrentItems();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void ResetCurrentItems()
        {
            try
            {
                CurrentSelectedIdx = -1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public int GetSelectedItemCount()
        {
            return SelectedItems.Size();
        }

        public List<int> GetSelectedItems()
        {
            List<int> items = new List<int>(SelectedItems.Size());
            for (int i = 0; i < SelectedItems.Size(); i++)
            {
                items.Add(SelectedItems.KeyAt(i));
            }
            return items;
        }

        public void RemoveData(int position, GetChatConversationsObject.Messages users)
        {
            try
            {
                var index = MessageList.IndexOf(MessageList.FirstOrDefault(a => a.Id == users.Id));
                if (index != -1)
                {
                    MessageList.Remove(users);
                    NotifyItemRemoved(index);
                    NotifyItemRangeRemoved(0, ItemCount);
                }

                ResetCurrentItems();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void ClearSelections()
        {
            try
            {
                SelectedItems.Clear();
                NotifyDataSetChanged();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void ToggleSelection(int pos)
        {
            try
            {
                CurrentSelectedIdx = pos;
                if (SelectedItems.Get(pos, false))
                {
                    SelectedItems.Delete(pos);
                }
                else
                {
                    SelectedItems.Put(pos, true);
                }
                NotifyItemChanged(pos);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        public IList GetPreloadItems(int p0)
        {
            try
            {
                var d = new List<string>();
                var item = MessageList[p0];

                if (item == null )
                    return null;

                if (item.Media != "")
                {
                    d.Add(item.Media);
                    return d;
                } 

                return d; 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public RequestBuilder GetPreloadRequestBuilder(Object p0)
        {
            return Glide.With(ActivityContext).Load(p0.ToString())
                .Apply(new RequestOptions());
        }
    }

    public class UserMessagesAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; set; }

        #endregion

        public UserMessagesAdapterViewHolder(View itemView, Action<UserMessagesAdapterClickEventArgs> clickListener,
            Action<UserMessagesAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                //Create an Event
                itemView.Click += (sender, e) => clickListener(new UserMessagesAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new UserMessagesAdapterClickEventArgs { View = itemView, Position = AdapterPosition });

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }

    public class TextViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public LinearLayout LytParent { get; set; }
        public TextView Time { get; set; }
        public View MainView { get; set; }
        public AutoLinkTextView AutoLinkTextView { get; set; }
        public TextSanitizer TextSanitizerAutoLink { get; set; }

        #endregion

        public TextViewHolder(View itemView, Action<UserMessagesAdapterClickEventArgs> clickListener, Action<UserMessagesAdapterClickEventArgs> longClickListener, Activity activity) : base(itemView)
        {
            try
            {
                MainView = itemView;

                LytParent = itemView.FindViewById<LinearLayout>(Resource.Id.main);
                AutoLinkTextView = itemView.FindViewById<AutoLinkTextView>(Resource.Id.active);
                Time = itemView.FindViewById<TextView>(Resource.Id.time);

                AutoLinkTextView.SetTextIsSelectable(true);

                if (TextSanitizerAutoLink == null)
                {
                    TextSanitizerAutoLink = new TextSanitizer(AutoLinkTextView, activity);
                }

                itemView.Click += (sender, e) => clickListener(new UserMessagesAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new UserMessagesAdapterClickEventArgs { View = itemView, Position = AdapterPosition });

            }
            catch (Exception e)
            {
                Console.WriteLine(e + "Error");
            }
        }
    }

    public class ImageViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get;  set; }
        public LinearLayout LytParent { get; set; }
        public ImageView ImageView { get;  set; }
        public ProgressBar LoadingProgressView { get; set; }
        public TextView Time { get;  set; }

        #endregion

        public ImageViewHolder(View itemView) : base(itemView)
        {
            try
            {
                MainView = itemView;
                LytParent = itemView.FindViewById<LinearLayout>(Resource.Id.main);
                ImageView = itemView.FindViewById<ImageView>(Resource.Id.imgDisplay);
                LoadingProgressView = itemView.FindViewById<ProgressBar>(Resource.Id.loadingProgressview);
                Time = itemView.FindViewById<TextView>(Resource.Id.time);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public class StickerViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get;  set; }
        public LinearLayout LytParent { get; set; }
        public ImageView ImageView { get;  set; }
        public ProgressBar LoadingProgressView { get; set; }
        public TextView Time { get;  set; }

        #endregion

        public StickerViewHolder(View itemView) : base(itemView)
        {
            try
            {
                MainView = itemView;
                LytParent = itemView.FindViewById<LinearLayout>(Resource.Id.main);
                ImageView = itemView.FindViewById<ImageView>(Resource.Id.imgDisplay);
                LoadingProgressView = itemView.FindViewById<ProgressBar>(Resource.Id.loadingProgressview);
                Time = itemView.FindViewById<TextView>(Resource.Id.time);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public class GifViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get;  set; }
        public LinearLayout LytParent { get; set; }
        public ImageView ImageGifView { get;  set; }
        public ProgressBar LoadingProgressView { get; set; }
        public TextView Time { get;  set; }

        #endregion

        public GifViewHolder(View itemView) : base(itemView)
        {
            try
            {
                MainView = itemView;
                LytParent = itemView.FindViewById<LinearLayout>(Resource.Id.main);
                ImageGifView = itemView.FindViewById<ImageView>(Resource.Id.imggifdisplay);
                LoadingProgressView = itemView.FindViewById<ProgressBar>(Resource.Id.loadingProgressview);
                Time = itemView.FindViewById<TextView>(Resource.Id.time);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public class UserMessagesAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}