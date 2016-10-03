using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Parse;
using UnityEngine.SceneManagement;
using Assets.Networking;
using System.Threading.Tasks;
using System;
using Assets.Mangers;


class NetworkManager : MonoBehaviour
{
    #region One-time Creation

    public LevelDefinition levelDef;

    private static bool hasStarted = false;

    public static bool StartFromBeginingIfNotStartedYet()
    {
        if (!hasStarted)
        {
            SceneManager.LoadScene("PersistentObjectInit");
            return true;
        }
        return false;
    }

    public GameUser CurrentUser { get; set; }

    void Awake()
    {
        // For running on the main thread
        calls = new List<CallInfo>();
        functions = new List<Func>();
        StartCoroutine(Executer());
        
        // This NetworkManager object will persist until the game is closed
        DontDestroyOnLoad(gameObject);

        // TODO: Apparently you can't call any parse stuff until this scene has finished. Maybe we need to wrap the Parse stuff in a outer scene that loads this one.
        //CurrentUser = new GameUser();
        
        levelDef = new LevelDefinition();

        // Now that the network manager is ready, head to the menu
        SceneManager.LoadScene("StartMenu");

        hasStarted = true;
    }
    #endregion

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            LevelManager.quitPlaying();
        }
    }

    public void loadFriends(Action<string, string> loadFriends)
    {
        //
        // Populate friend buttons for creating a new match
        // 
        ParseUser currentUser = ParseUser.CurrentUser;
        try
        {
            IList<string> friends = currentUser.Get<IList<string>>("friends");
            foreach (string friend in friends)
            {
                ParseUser.Query.GetAsync((string)friend).ContinueWith(t =>
                {
                    NetworkManager.Call(() => loadFriends(t.Result.Username, t.Result.ObjectId));
                });
            }
        }
        catch (KeyNotFoundException)
        {
            // This user hasn't added friends yet. Do nothing. 
        }
    }

    public void createMatch(string userIdOfFriend, string usernameOfFriend)
    {
        this.levelDef.LevelDefinitionSetDefault();

        ParseObject matchParseObject = this.levelDef.getParseObject(
            ParseUser.CurrentUser.ObjectId,
            ParseUser.CurrentUser.Username,
            userIdOfFriend,
            usernameOfFriend
            );

        matchParseObject.SaveAsync();
    }

    public void loadMatch(string matchId)
    {
        ParseQuery<ParseObject> query = ParseObject.GetQuery("MatchTest");
        query.GetAsync(matchId).ContinueWith(t =>
        {
            ParseObject match = t.Result;
            NetworkManager.Call(() =>
            {
                levelDef.initializeFromParseObject(match);
            });
        });
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

    /// <summary>
    /// Sign in with the username and password, also provide an action to do with the result of the sign in.
    /// </summary>
    /// <param name="username">Username to sign in with</param>
    /// <param name="password">Password to sign in with</param>
    /// <param name="processSignInResult">An action that takes a string to process after sign in. The string is null if the sign in was successful</param>
    public void signUpAsync(string username, string password, Action<string> processSignInResult)
    {
        ParseUser user = new ParseUser()
        {
            Username = username,
            Password = password
            //,Email = null
        };

        // other fields can be set just like with ParseObject
        //user["phone"] = "650-555-0000";

        user.SignUpAsync().ContinueWith(t =>
        {
            string signInResultMessage = null;
            if (t.IsCanceled)
            {
                Debug.Log("Canceled Sign Up");
            }
            if (t.IsFaulted)
            {
                Debug.Log("Sign up faulted");

                foreach (var ex in t.Exception.InnerExceptions)
                {
                    ParseException parseException = (ParseException)ex;
                    signInResultMessage = parseException.Message;
                    Debug.Log("Error message " + signInResultMessage);
                    Debug.Log("Error code: " + parseException.Code);
                }
            }
            else
            {
                // Login was successful.
                Debug.Log("Sign up success");
            }
            processSignInResult(signInResultMessage);
        });
    }

    /// <summary>
    /// Sign in with the username and password, also provide an action to do with the result of the sign in.
    /// </summary>
    /// <param name="username">Username to sign in with</param>
    /// <param name="password">Password to sign in with</param>
    /// <param name="processSignInResult">An action that takes a string to process after sign in. The string is null if the sign in was successful</param>
    public void signInAsync(string username, string password, Action<string> processSignInResult)
    {
        // TODO save actual username as lowercase so that we don't have dups, but also save display name with cases.
        ParseUser.LogInAsync(username, password).ContinueWith(t =>
        {
            string signInResultMessage = null;
            if (t.IsCanceled)
            {
                Debug.Log("Canceled Sign In");
            }
            if (t.IsFaulted)
            {
                Debug.Log("Sign in faulted");
                foreach (var ex in t.Exception.InnerExceptions)
                {
                    ParseException parseException = (ParseException)ex;
                    signInResultMessage = parseException.Message;
                    Debug.Log("Error message " + signInResultMessage);
                    Debug.Log("Error code: " + parseException.Code);
                }
            }
            else
            {
                // Login was successful.
                Debug.Log("Sign in success");
            }

            processSignInResult(signInResultMessage);
        });
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
