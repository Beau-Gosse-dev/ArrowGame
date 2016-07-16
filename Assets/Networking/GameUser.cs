using System;
using Parse;
using System.Threading.Tasks;

namespace Assets.Networking
{
    class GameUser
    {
        string _userName;
        string _userId;

        public GameUser()
        {
            InitializeUserName();
        }

        public string UserName
        {
            get
            {
                return _userName;
            }
        }
        public string UserId
        {
            get
            {
                return _userId;
            }
        }

        public bool IsLoggedIn
        {
            get
            {
                return ParseUser.CurrentUser != null;
            }
        }

        private void InitializeUserName()
        {
            if (IsLoggedIn)
            {
                _userName = ParseUser.CurrentUser.Username;
                _userId = ParseUser.CurrentUser.ObjectId;
            }
            else
            {
                _userName = "Not Logged In";
                _userId = "Not Logged In";
            }
        }

        internal void LogOut(NetworkManager.Func handleLogOutResult)
        {
            ParseUser.LogOutAsync().ContinueWith(T =>
            {
                NetworkManager.Call(handleLogOutResult);
            });
        }
    }
}
