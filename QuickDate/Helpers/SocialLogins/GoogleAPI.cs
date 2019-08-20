using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using QuickDate.Properties;

namespace QuickDate.Helpers.SocialLogins
{
    public class GoogleAPI : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private GoogleProfile _googleProfile;
        private readonly GoogleServices _googleServices;

        public GoogleProfile GoogleProfile
        {
            get { return _googleProfile; }
            set
            {
                _googleProfile = value;
                OnPropertyChanged();
            }
        }

        public GoogleAPI()
        {
            try
            {
                _googleServices = new GoogleServices();
            }
            catch (Exception e)
            {
               Console.WriteLine(e);
            }
        }

        public async Task<string> GetAccessTokenAsync(string code)
        {
            try
            {
                var dataGoogle = await _googleServices.GetAccessTokenAsync(code);
                return dataGoogle.AccessToken;
            }
            catch (Exception e)
            {
               Console.WriteLine(e);
                return null;
            }
        }

        public async Task SetGoogleUserProfileAsync(string accessToken)
        {
            try
            {
                GoogleProfile = await _googleServices.GetGoogleUserProfileAsync(accessToken);
            }
            catch (Exception e)
            {
               Console.WriteLine(e);
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}