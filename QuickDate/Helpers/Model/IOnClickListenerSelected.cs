using Android.Views;
using QuickDateClient.Classes.Chat;

namespace QuickDate.Helpers.Model
{
    public interface IOnClickListenerSelected
    {
        void ItemClick(View view, GetConversationListObject.Data obj, int pos);

        void ItemLongClick(View view, GetConversationListObject.Data obj, int pos);
    }
}