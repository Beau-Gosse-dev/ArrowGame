using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Parse;
using UnityEngine.SceneManagement;
using Assets.Networking;
using System.Threading.Tasks;

class NetworkManager : MonoBehaviour
{
    #region One-time Creation
    public static NetworkManager Instance { get; private set; }
    public GameUser CurrentUser { get; set; }

    void Awake()
    {
        // For running on the main thread
        calls = new List<CallInfo>();
        functions = new List<Func>();
        StartCoroutine(Executer());

        Instance = this;
        // This NetworkManager object will persist until the game is closed
        DontDestroyOnLoad(gameObject);

        // TODO: Apparently you can't call any parse stuff until this scene has finished. Maybe we need to wrap the Parse stuff in a outer scene that loads this one.
        //CurrentUser = new GameUser();

        // Now that the network manager is ready, head to the menu
        SceneManager.LoadScene("StartMenu");
    }
    #endregion

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            LevelManager.quitPlaying();
        }
    }


    public Task<List<Match>> GetMatchesAsync()
    {
        // TODO: Can this function be cleaned up?
        ParseQuery<ParseObject> queryLeftPlayer = new ParseQuery<ParseObject>("MatchTest").WhereEqualTo("playerLeftId", ParseUser.CurrentUser.ObjectId);
        ParseQuery<ParseObject> queryRightPlayer = new ParseQuery<ParseObject>("MatchTest").WhereEqualTo("playerRightId", ParseUser.CurrentUser.ObjectId);
        ParseQuery<ParseObject> queryBothSides = queryLeftPlayer.Or(queryRightPlayer);
        return queryBothSides.FindAsync().ContinueWith(t =>
        {
            var matches = new List<Match>();
            IEnumerable<ParseObject> results = t.Result;
            foreach (ParseObject parseObject in results)
            {
                matches.Add(getMatchFromParseObject(parseObject));
            }
            return matches;
        });
    }

    private Match getMatchFromParseObject(ParseObject parseObject)
    {
        var match = new Match();
        // If the current user is the match's left player, show the right player on the button (the opponent)
        if (parseObject.Get<string>("playerLeftId") == ParseUser.CurrentUser.ObjectId)
        {
            match.friendId = parseObject.Get<string>("playerRightId");
            match.friendName = parseObject.Get<string>("playerRightName");
        }
        else
        {
            match.friendId = parseObject.Get<string>("playerLeftId");
            match.friendName = parseObject.Get<string>("playerLeftName");
        }
        match.matchId = parseObject.ObjectId;
        match.leftHealth = parseObject.Get<float>("playerLeftHealth");
        match.rightHealth = parseObject.Get<float>("playerRightHealth");
        return match;
    }

    #region Parse Stuff
    class CallInfo
    {
        public Function func;
        public object parameter;
        public CallInfo(Function Func, object Parameter)
        {
            func = Func;
            parameter = Parameter;
        }
        public void Execute()
        {
            func(parameter);
        }
    }

    public delegate void Function(object parameter);
    public delegate void Func();

    static List<CallInfo> calls = new List<CallInfo>();
    static List<Func> functions = new List<Func>();


    static System.Object callsLock = new System.Object();
    static System.Object functionsLock = new System.Object();

    public static void Call(Function Func, object Parameter)
    {
        lock (callsLock)
        {
            calls.Add(new CallInfo(Func, Parameter));
        }
    }
    public static void Call(Func func)
    {
        lock (functionsLock)
        {
            functions.Add(func);
        }
    }

    IEnumerator Executer()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.01f);

            while (calls.Count > 0)
            {
                calls[0].Execute();
                lock (callsLock)
                {
                    calls.RemoveAt(0);
                }
            }

            lock (functionsLock)
            {
                while (functions.Count > 0)
                {
                    if (functions != null)
                    {
                        if (functions[0] != null)
                        {
                            functions[0]();
                            functions.RemoveAt(0);
                        }
                    }
                }
            }
        }
    }
    #endregion
}
