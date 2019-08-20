using System;
using System.Linq;
using Android.App;
using QuickDateClient.Classes.Common;
using QuickDateClient.Classes.Global;

namespace QuickDate.Helpers.Utils
{
    public class QuickDateTools
    {
        private static string[] RelationshipLocal = Application.Context.Resources.GetStringArray(Resource.Array.RelationShipArray);
        private static string[] WorkStatusLocal = Application.Context.Resources.GetStringArray(Resource.Array.WorkStatusArray);
        private static string[] EducationLocal = Application.Context.Resources.GetStringArray(Resource.Array.EducationArray);
        private static string[] HairColorLocal = Application.Context.Resources.GetStringArray(Resource.Array.HairColorArray);
        private static string[] BodyLocal = Application.Context.Resources.GetStringArray(Resource.Array.BodyArray);
        private static string[] EthnicityLocal = Application.Context.Resources.GetStringArray(Resource.Array.EthnicityArray);
        private static string[] PetsLocal = Application.Context.Resources.GetStringArray(Resource.Array.PetsArray);
        private static string[] FriendsLocal = Application.Context.Resources.GetStringArray(Resource.Array.FriendsArray);
        private static string[] ChildrenLocal = Application.Context.Resources.GetStringArray(Resource.Array.ChildrenArray);
        private static string[] CharacterLocal = Application.Context.Resources.GetStringArray(Resource.Array.CharacterArray);
        private static string[] TravelLocal = Application.Context.Resources.GetStringArray(Resource.Array.TravelArray);
        private static string[] DrinkLocal = Application.Context.Resources.GetStringArray(Resource.Array.DrinkArray);
        private static string[] SmokeLocal = Application.Context.Resources.GetStringArray(Resource.Array.SmokeArray);
        private static string[] ReligionLocal = Application.Context.Resources.GetStringArray(Resource.Array.ReligionArray);
        private static string[] CarLocal = Application.Context.Resources.GetStringArray(Resource.Array.CarArray);
        private static string[] LiveWithLocal = Application.Context.Resources.GetStringArray(Resource.Array.LiveWithArray);
        private static string[] HeightLocal = Application.Context.Resources.GetStringArray(Resource.Array.HeightArray);
        private static string[] countriesArray = Application.Context.Resources.GetStringArray(Resource.Array.countriesArray);
        private static string[] countriesArrayId = Application.Context.Resources.GetStringArray(Resource.Array.countriesArray_id);

        public static bool GetStatusOnline(int lastSeen , int isShowOnline)
        {
            try
            {
                string time = IMethods.Time.TimeAgo(lastSeen);
                bool status  = isShowOnline == 1 && time == IMethods.Time.LblJustNow ? true : false;
                return status;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public static string GetNameFinal(UserInfoObject dataUser)
        {
            try
            { 
                if (!string.IsNullOrEmpty(dataUser.Fullname))
                    return dataUser.Fullname;
                //ROEO  TEXTO NOMBRE Y APELLIDOS
                //string name = !string.IsNullOrEmpty(dataUser.FirstName) && !string.IsNullOrEmpty(dataUser.LastName)
                //    ? dataUser.FirstName + " " + dataUser.LastName
                //    : dataUser.Username;
                string name = !string.IsNullOrEmpty(dataUser.FirstName) && !string.IsNullOrEmpty(dataUser.LastName)
                ? dataUser.FirstName 
                : dataUser.Username;
                return name;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return dataUser?.Username;
            }
        }
         
        public static string GetWorkStatus(int id)
        {
            try
            { 
                int index = id - 1;
                if (index > -1)
                {
                    string name = WorkStatusLocal[index];
                    return name;
                }
                return ""; 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            } 
        }

        public static string GetRelationship(int id)
        {
            try
            {
                int index = id - 1;
                if (index > -1)
                {
                    string name = RelationshipLocal[index];
                    return name;
                }
                return "";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }

        public static string GetEducation(int id)
        {
            try
            {
                int index = id - 1;
                if (index > -1)
                {
                    string name = EducationLocal[index];
                    return name;
                }
                return "";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }
         
        public static string GetEthnicity(int id)
        {
            try
            {
                int index = id - 1;
                if (index > -1)
                {
                    string name = EthnicityLocal[index];
                    return name;
                }
                 
                return "";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }
         
        public static string GetBody(int id)
        {
            try
            {
                int index = id - 1;
                if (index > -1)
                {
                    string name = BodyLocal[index];
                    return name;
                }

                return ""; 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }

        public static string GetHairColor(int id)
        {
            try
            {
                int index = id - 1;
                if (index > -1)
                {
                    string name = HairColorLocal[index];
                    return name;
                } 
                return ""; 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }

        public static string GetCharacter(int id)
        {
            try
            {
                int index = id - 1;
                if (index > -1)
                {
                    string name = CharacterLocal[index];
                    return name;
                }
                return ""; 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }

        public static string GetChildren(int id)
        {
            try
            {
                int index = id - 1;
                if (index > -1)
                {
                    string name = ChildrenLocal[index];
                    return name;
                }
                return ""; 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }

        public static string GetFriends(int id)
        {
            try
            {
                int index = id - 1;
                if (index > -1)
                {
                    string name = FriendsLocal[index];
                    return name;
                }
                return ""; 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }

        public static string GetLiveWith(int id)
        {
            try
            {
                int index = id - 1;
                if (index > -1)
                {
                    string name =  LiveWithLocal[index];
                    return name;
                }
                return ""; 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }
         
        public static string GetCar(int id)
        {
            try
            {
                int index = id - 1;
                if (index > -1)
                {
                    string name = CarLocal[index];
                    return name;
                }
                return ""; 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }
         
        public static string GetReligion(int id)
        {
            try
            {
                int index = id - 1;
                if (index > -1)
                {
                    string name = ReligionLocal[index];
                    return name;
                }
                return ""; 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }

        public static string GetSmoke(int id)
        {
            try
            {
                int index = id - 1;
                if (index > -1)
                {
                    string name = SmokeLocal[index];
                    return name;
                }
                return ""; 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }

        public static string GetTravel(int id)
        {
            try
            {
                int index = id - 1;
                if (index > -1)
                {
                    string name = TravelLocal[index];
                    return name;
                }
                return ""; 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }

        public static string GetNotification(GetNotificationsObject.Datum item)
        {
            try
            {
                string text = "";
                switch (item.Type)
                {
                    case "visit":
                        text = Application.Context.GetText(Resource.String.Lbl_VisitYou);
                        ListUtils.VisitsList.Add(item);
                        break;
                    case "like":
                        text = Application.Context.GetText(Resource.String.Lbl_LikeYou);
                        ListUtils.LikesList.Add(item);
                        break;
                    case "dislike":
                        text = Application.Context.GetText(Resource.String.Lbl_DislikeYou);
                        ListUtils.LikesList.Add(item);
                        break;
                    case "send_gift":
                        text = Application.Context.GetText(Resource.String.Lbl_SendGiftToYou);
                        break;
                    case "got_new_match":
                        text = Application.Context.GetText(Resource.String.Lbl_YouGotMatch);
                        ListUtils.MatchList.Add(item);
                        break;
                    default:
                        text = "";
                        break;
                }

                return text;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }

        public static string GetGender(int id)
        {
            try
            {
                string text = "";
                switch (id)
                {
                    case 0:
                        text = Application.Context.GetText(Resource.String.Lbl_Male);
                        break;
                    case 1:
                        text = Application.Context.GetText(Resource.String.Lbl_Female);
                        break;
                    default:
                        text = "";
                        break;
                }
                return text;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }

        public static string GetPets(int id)
        {
            try
            {
                int index = id - 1;
                if (index > -1)
                {
                    string name = PetsLocal[index];
                    return name;
                }
                return ""; 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }

        public static string GetDrink(int id)
        {
            try
            {
                int index = id - 1;
                if (index > -1)
                {
                    string name = DrinkLocal[index];
                    return name;
                }
                return ""; 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }


        public static string GetCountry(string codeCountry)
        {
            try
            { 
                var list = countriesArrayId.ToList();
                int index = 0;

                var data = list.FirstOrDefault(a => a.Contains(codeCountry));
               if (data != null)
               {
                   index = list.IndexOf(data);
               }
                
                if (index > -1)
                {
                    string name = countriesArray[index];
                    return name;
                }
                return "";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }
         
        public static string GetNotificationsText(string type)
        {
            try
            {
                string text = "";
                if (type == "visit")
                {
                    text = Application.Context.GetText(Resource.String.Lbl_VisitYou);
                }
                else if (type == "like")
                {
                    text = Application.Context.GetText(Resource.String.Lbl_LikeYou);
                }
                else if (type == "dislike")
                {
                    text = Application.Context.GetText(Resource.String.Lbl_DislikeYou);
                }
                else if (type == "send_gift")
                {
                    text = Application.Context.GetText(Resource.String.Lbl_SendGiftToYou);
                }
                else if (type == "got_new_match")
                {
                    text = Application.Context.GetText(Resource.String.Lbl_YouGotMatch);
                }
                else if (type == "message")
                {
                    text = Application.Context.GetText(Resource.String.Lbl_Message);
                }
                else if (type == "approve_receipt")
                {
                    text = Application.Context.GetText(Resource.String.Lbl_ApproveReceipt);
                }
                else if (type == "disapprove_receipt")
                {
                    text = Application.Context.GetText(Resource.String.Lbl_DisapproveReceipt);
                }
                else if (type == "accept_chat_request")
                {
                    text = Application.Context.GetText(Resource.String.Lbl_AcceptChatRequest);
                }
                else if (type == "accept_chdecline_chat_requestat_request")
                {
                    text = Application.Context.GetText(Resource.String.Lbl_DeclineChatRequest);
                }
                
                return text;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }
    }
}