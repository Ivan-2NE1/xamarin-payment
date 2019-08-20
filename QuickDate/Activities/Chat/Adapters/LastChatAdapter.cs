using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Android.App;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using QuickDate.Helpers.CacheLoaders;
using QuickDate.Helpers.Fonts;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDateClient.Classes.Chat;
using Object = Java.Lang.Object;

namespace QuickDate.Activities.Chat.Adapters
{
    public class LastChatAdapter : RecyclerView.Adapter, ListPreloader.IPreloadModelProvider
    {
        public Activity ActivityContext;

        public ObservableCollection<GetConversationListObject.Data> UserList = new ObservableCollection<GetConversationListObject.Data>();
        public event EventHandler<LastChatAdapterClickEventArgs> OnItemClick;
        public event EventHandler<LastChatAdapterClickEventArgs> OnItemLongClick;

        public IOnClickListenerSelected ClickListener;
        public SparseBooleanArray SelectedItems;
        public int CurrentSelectedIdx = -1;

        public LastChatAdapter(Activity context)
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

        public void SetOnClickListener(IOnClickListenerSelected onClickListener)
        {
            ClickListener = onClickListener;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            try
            {
                if (viewHolder is LastChatAdapterViewHolder holder)
                {
                    var item = UserList[position];
                    if (item != null)
                    { 
                        Initialize(holder, item);

                        holder.lyt_parent.Activated = SelectedItems.Get(position, false);

                        holder.lyt_parent.Click += delegate
                        {
                            try
                            {
                                if (ClickListener == null) return;

                                ClickListener.ItemClick(holder.MainView, item, position);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        };

                        holder.lyt_parent.LongClick += delegate
                        {
                            try
                            {
                                if (ClickListener == null) return;

                                ClickListener.ItemLongClick(holder.MainView, item, position);

                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        };

                        ToggleCheckedIcon(holder, position);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void Initialize(LastChatAdapterViewHolder holder, GetConversationListObject.Data item)
        {
            try
            {
                GlideImageLoader.LoadImage(ActivityContext, item.User.Avater, holder.ImageAvatar, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);

                string name = IMethods.Fun_String.DecodeString(QuickDateTools.GetNameFinal(item.User));
                if (holder.Txt_Username.Text != name)
                {
                    holder.Txt_Username.Text = name;
                }

                //If message contains Media files 
                switch (item.MessageType)
                {
                    case "text":
                    {
                        holder.LastMessagesIcon.Visibility = ViewStates.Gone;
                        holder.Txt_LastMessages.Text = item.Text.Contains("http")
                            ? IMethods.Fun_String.SubStringCutOf(item.Text, 30)
                            : IMethods.Fun_String.DecodeString(IMethods.Fun_String.SubStringCutOf(item.Text, 30))
                            ?? ActivityContext.GetText(Resource.String.Lbl_SendMessage);
                        break;
                    }
                    case "media":
                    {
                        holder.LastMessagesIcon.Visibility = ViewStates.Visible;
                        FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, holder.LastMessagesIcon,IonIconsFonts.Images);
                        holder.Txt_LastMessages.Text = Application.Context.GetText(Resource.String.Lbl_SendImageFile);
                        break;
                    }
                    case "sticker" when item.Sticker.Contains(".gif"):
                    {
                        holder.LastMessagesIcon.Visibility = ViewStates.Visible;
                        FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, holder.LastMessagesIcon,FontAwesomeIcon.Gift);
                        holder.Txt_LastMessages.Text = Application.Context.GetText(Resource.String.Lbl_SendGifFile);
                        break;
                    }
                    case "sticker":
                    {
                        holder.LastMessagesIcon.Visibility = ViewStates.Visible;
                        FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, holder.LastMessagesIcon,IonIconsFonts.Happy);
                        holder.Txt_LastMessages.Text = Application.Context.GetText(Resource.String.Lbl_SendStickerFile);
                        break;
                    }
                }

                //last seen time  
                holder.Txt_timestamp.Text = IMethods.Time.ReplaceTime(item.Time);
                 
                if (item.NewMessages <= 0)
                {
                    holder.ImageColor.Visibility = ViewStates.Invisible;
                }
                else
                {
                    var drawable = TextDrawable.TextDrawable.TextDrawbleBuilder.BeginConfig().FontSize(25).EndConfig().BuildRound(item.NewMessages.ToString(), Color.ParseColor(AppSettings.MainColor));
                    holder.ImageColor.SetImageDrawable(drawable);
                    holder.ImageColor.Visibility = ViewStates.Visible;
                }
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
                //Setup your layout here >> Style_HContact_view
                var itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Style_LastChatView, parent, false);
                var vh = new LastChatAdapterViewHolder(itemView, Click, LongClick);
                return vh;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }
        }

        public override int ItemCount
        {
            get
            {
                if (UserList != null)
                    return UserList.Count;
                return 0;
            }
        }
         
        
        #region Toolbar & Selected

        private void ToggleCheckedIcon(LastChatAdapterViewHolder holder, int position)
        {
            try
            {
                if (SelectedItems.Get(position, false))
                {
                    holder.lyt_image.Visibility = ViewStates.Gone;
                    holder.lyt_checked.Visibility = ViewStates.Visible;
                    if (CurrentSelectedIdx == position) ResetCurrentItems();
                }
                else
                {
                    holder.lyt_checked.Visibility = ViewStates.Gone;
                    holder.lyt_image.Visibility = ViewStates.Visible;
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

        public void RemoveData()
        {
            try
            { 
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

        public GetConversationListObject.Data GetItem(int position)
        {
            return UserList[position];
        }

        public override long GetItemId(int position)
        {
            try
            {
                return UserList[position].Id;
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

        private void Click(LastChatAdapterClickEventArgs args)
        {
            OnItemClick?.Invoke(this, args);
        }

        private void LongClick(LastChatAdapterClickEventArgs args)
        {
            OnItemLongClick?.Invoke(this, args);
        }

        public IList GetPreloadItems(int p0)
        {
            try
            {
                var d = new List<string>();
                var item = UserList[p0];

                if (item == null)
                    return null;

                if (item.User.Avater != "")
                {
                    d.Add(item.User.Avater);
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
                .Apply(new RequestOptions().CircleCrop());
        }
    }

    public class LastChatAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; private set; }

        public RelativeLayout lyt_parent { get; private set; }
        public TextView Txt_Username { get; private set; }
        public TextView LastMessagesIcon { get; private set; }
        public TextView Txt_LastMessages { get; private set; }
        public TextView Txt_timestamp { get; private set; }
        public ImageView ImageAvatar { get; private set; }
        public ImageView ImageColor { get; private set; }

        public RelativeLayout lyt_checked { get; private set; }
        public RelativeLayout lyt_image { get; private set; }

        #endregion

        public LastChatAdapterViewHolder(View itemView, Action<LastChatAdapterClickEventArgs> clickListener, Action<LastChatAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                //Get values
                lyt_parent = (RelativeLayout)MainView.FindViewById(Resource.Id.main);
                Txt_Username = (TextView)MainView.FindViewById(Resource.Id.Txt_Username);
                LastMessagesIcon = (AppCompatTextView)MainView.FindViewById(Resource.Id.LastMessages_icon);
                Txt_LastMessages = (TextView)MainView.FindViewById(Resource.Id.Txt_LastMessages);
                Txt_timestamp = (TextView)MainView.FindViewById(Resource.Id.Txt_timestamp);
                ImageAvatar = (ImageView)MainView.FindViewById(Resource.Id.ImageAvatar);

                ImageColor = (ImageView)MainView.FindViewById(Resource.Id.image_view);

                lyt_checked = (RelativeLayout)MainView.FindViewById(Resource.Id.lyt_checked);
                lyt_image = (RelativeLayout)MainView.FindViewById(Resource.Id.lyt_image);


                //Create an Event
                itemView.Click += (sender, e) => clickListener(new LastChatAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new LastChatAdapterClickEventArgs { View = itemView, Position = AdapterPosition });

                //Dont Remove this code #####
                FontUtils.SetFont(Txt_Username, Fonts.SfRegular);
                FontUtils.SetFont(Txt_LastMessages, Fonts.SfMedium);
                //#####
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }

    public class LastChatAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}