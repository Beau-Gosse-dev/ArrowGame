using Parse;

namespace Assets.Networking
{
    class GameUser
    {
        string _userName;

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
            }
            else
            {
                _userName = "Not Logged In";
            }
        }
    }
}
