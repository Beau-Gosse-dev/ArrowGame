using UnityEngine;
using Facebook.Unity;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using System;
using Parse;

class NetworkManager : MonoBehaviour
{
    //# region Constants and configuration values
    //private const string CognitoIdentityPoolId = "us-east-1:6188f484-165e-4575-91e5-4ad1cc59fd30";

    //private const string PlayersDatasetName = "KnownPlayers";
    //private const string MatchesDatasetName = "Matches";

    // Needed only when building for Android
    //private const string AndroidPlatformApplicationArn = null;
    //private const string GoogleConsoleProjectId = null;

    // Needed only when building for iOS
    //private const string IOSPlatformApplicationArn = null;

    // By default, we use the Region Endpoint specified in the
    // AWSSDK/src/Core/Resource/awsconfig.xml file. If you are using the same region for all of
    // your services, just change the region value in awsconfig.xml. Otherwise, you can
    // replace the null values below with the correct region endpoints,
    // i.e. RegionEndpoint.USEast1.
    //private static readonly RegionEndpoint _cognitoRegion = RegionEndpoint.USEast1;
    //private RegionEndpoint CognitoRegion { get { return _cognitoRegion != null ? _cognitoRegion : AWSConfigs.RegionEndpoint; } }
    //#endregion

    //#region AWS Clients, Managers, and Contexts, And Info
    //private CognitoAWSCredentials _credentials;

    //private CognitoAWSCredentials Credentials
    //{
    //    get
    //    {
    //        if (_credentials == null)
    //            _credentials = new CognitoAWSCredentials(CognitoIdentityPoolId, CognitoRegion);
    //        return _credentials;
    //    }
    //}

    //private CognitoSyncManager _cognitoSyncManager;

    //private CognitoSyncManager CognitoSyncManager
    //{
    //    get
    //    {
    //        if (_cognitoSyncManager == null)
    //        {
    //            _cognitoSyncManager = new CognitoSyncManager(Credentials, new AmazonCognitoSyncConfig { RegionEndpoint = CognitoRegion });
    //        }
    //        return _cognitoSyncManager;
    //    }
    //}
    //#endregion

    #region One-time Creation
    public static NetworkManager Instance { get; private set; }
    void Awake()
    {
        // For running on the main thread
        calls = new List<CallInfo>();
        functions = new List<Func>();
        StartCoroutine(Executer());

        Instance = this;
        // This NetworkManager object will persist until the game is closed
        DontDestroyOnLoad(gameObject);

        // Now that the network manager is ready, head to the menu
        Application.LoadLevel("StartMenu");
    }
    #endregion

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            LevelManager.quitPlaying();
        }
    }

    //# region Using Amazon Cognito Sync
    //// Save the game state to CognitoSync's local storage.
    //public void SaveGameStateLocal(GameState gameState)
    //{
    //    // Add our own PlayerInfo, to the players dataset, in the
    //    // form of key: player's id, value: player's name
    //    CognitoSyncManager.WipeData();
    //    using (Dataset playersDataset = CognitoSyncManager.OpenOrCreateDataset(PlayersDatasetName))
    //    {

    //        Debug.Log("TRYING TO PUT DATAset");
    //        if (gameState.Self != null)
    //        {
    //            playersDataset.Put(gameState.Self.Id, gameState.Self.Name);

    //            Debug.Log("PUT dataset");
    //        }
    //    }
    //}

    //// Synchronize the locally saved data with Cognito
    //public void SynchronizeLocalDataAsync()
    //{
    //    Debug.Log("Starting aysnc" );
    //    Dataset playersDataset = CognitoSyncManager.OpenOrCreateDataset(PlayersDatasetName);

    //    Debug.Log("After openOrCreateDataset");
    //    playersDataset.OnSyncFailure += delegate
    //    {
    //        Debug.LogWarning("Failed to sync Friends, but they will still be saved locally.");
    //        playersDataset.Dispose();
    //    };
    //    playersDataset.OnSyncSuccess += delegate { playersDataset.Dispose();
    //        Debug.LogWarning("Synced Friends remotely");
    //    };
    //    playersDataset.Synchronize();

    //    Debug.Log("After synchronize");
    //}

    //// Extract a dictionary with keys: player Id and values: PlayerInfo objects from the
    //// key-value pairs in the dataset.
    //private static Dictionary<string, GameState.PlayerInfo> PlayersDatasetToDict(Dataset playersDataset, string selfId, out GameState.PlayerInfo self)
    //{
    //    IDictionary<string, string> friendsStringDict = playersDataset.GetAll();
    //    Dictionary<string, GameState.PlayerInfo> friendsDict = new Dictionary<string, GameState.PlayerInfo>();
    //    self = null;
    //    foreach (string friendId in friendsStringDict.Keys)
    //    {
    //        if (friendId == selfId)
    //        {
    //            self = new GameState.PlayerInfo(friendId, friendsStringDict[friendId]);
    //        }
    //        else
    //        {
    //            friendsDict.Add(friendId, new GameState.PlayerInfo(friendId, friendsStringDict[friendId]));
    //        }
    //    }

    //    if (string.IsNullOrEmpty(selfId))
    //    {
    //        self = null;
    //    }
    //    else if (self == null)
    //    {
    //        self = new GameState.PlayerInfo(selfId, "anonymous");
    //    }
    //    else
    //    {
    //        if (string.IsNullOrEmpty(self.Id))
    //        {
    //            self = new GameState.PlayerInfo(selfId, self.Name);
    //        }
    //        if (string.IsNullOrEmpty(self.Name))
    //        {
    //            self = new GameState.PlayerInfo(self.Id, "anonymous");
    //        }
    //    }
    //    return friendsDict;
    //}

    //// Use the Facebook sdk to log in with Facebook credentials
    //public void LogInToFacebookAsync()
    //{
    //    if (!FB.IsInitialized)
    //    {
    //        FB.Init(() =>
    //        {
    //            FB.LogInWithReadPermissions(null, FacebookLoginCallback);
    //        });
    //    }
    //    else
    //    {
    //        FB.LogInWithReadPermissions(null, FacebookLoginCallback);
    //    }
    //}

    //// Attch the Facebook Login token to our Cognito Identity.
    //private void FacebookLoginCallback(ILoginResult result)
    //{
    //    if (result.Error != null || !FB.IsLoggedIn)
    //    {
    //        Debug.LogError(result.Error);
    //    }
    //    else
    //    {
    //        Debug.Log("Adding login to credentials");
    //        Credentials.AddLogin("graph.facebook.com", result.AccessToken.TokenString);
    //        SaveGameStateLocal(new GameState(new GameState.PlayerInfo("testIdentifier", "testName")));
    //        SynchronizeLocalDataAsync();
    //    }
    //    //GameManager.Instance.Load();
    //}        

    //// Attch the Facebook Login token to our Cognito Identity.
    //public void testLogin()
    //{
    //    Debug.Log("Adding login to credentials");
    //    Credentials.AddLogin("graph.facebook.com", "asfgasf");         
    //    Debug.Log("Added login to credentials");
    //    SaveGameStateLocal(new GameState(new GameState.PlayerInfo("testIdentifier", "testName")));
    //    Debug.Log("Saved Local");
    //    SynchronizeLocalDataAsync();
    //    Debug.Log("After Async Call");
    //}
    //#endregion




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
