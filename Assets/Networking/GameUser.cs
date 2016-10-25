namespace Assets.Networking
{
    class GameUser
    {
        public GameUser(string id, string name)
        {
            UserId = id;
            UserName = name;
        }

        public GameUser()
        {
        }

        public string UserName { get; set; }
        public string UserId { get; set; }
    }
}
