using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.SceneManagement;
using Assets.Networking;
using System.Threading.Tasks;
using System;
using Assets.Mangers;
using Newtonsoft.Json;
using Assets.GameObjects;

using PlayFab;
using PlayFab.ClientModels;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

class NetworkManager : MonoBehaviour
{
    public LevelDefinition levelDef;

    private static bool hasStarted = false;

    public string PlayFabId;

    public GameUser CurrentUser { get; set; }

    public bool IsLoggedInWithUsernamePassword { get; private set; }

    public static bool StartFromBeginingIfNotStartedYet()
    {
        if (!hasStarted)
        {
            SceneManager.LoadScene("PersistentObjectInit");
            return true;
        }
        return false;
    }

    public void addFriend(string userIdOfFriend)
    {
        var request = new AddFriendRequest()
        {
            FriendPlayFabId = userIdOfFriend
        };

        PlayFabClientAPI.AddFriend(request, (result) =>
        {
            Debug.Log("Successfully added friend: " + userIdOfFriend);
        }, (error) => LogError(error, "AddFriend"));          
    }

    void Awake()
    {
        if (!hasStarted)
        {
            // For running on the main thread
            calls = new List<CallInfo>();
            functions = new List<Func>();
            StartCoroutine(Executer());

            // This NetworkManager object will persist until the game is closed
            DontDestroyOnLoad(gameObject);

            // TODO: Apparently you can't call any parse stuff until this scene has finished. Maybe we need to wrap the Parse stuff in a outer scene that loads this one.
            //CurrentUser = new GameUser();
            CurrentUser = new GameUser();
            
            //levelDef = new LevelDefinition();

            // Now that the network manager is ready, head to the menu
            SceneManager.LoadScene("StartMenu");

            // Set the static playfab ID so we can make calls with PlayFab
            PlayFabSettings.TitleId = "8DA7";

            hasStarted = true;
        }

        PlayFabLoginThisDevice();
    }    

    void Start()
    {
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            LevelManager.quitPlaying();
        }
    }

    public void SaveLevelDefinitionToServer()
    {
        //currentMatch["isPlayerLeftTurn"] = levelDef.IsPlayerLeftTurn;
        //currentMatch["playerDistanceFromCenter"] = levelDef.PlayerDistanceFromCenter;
        //currentMatch["playerLeftHealth"] = levelDef.PlayerLeftHealth;
        //currentMatch["playerRightHealth"] = levelDef.PlayerRightHealth;
        //currentMatch["wallHeight"] = levelDef.WallHeight;
        //currentMatch["wallPosition"] = levelDef.WallPosition;
        //currentMatch["gameState"] = levelDef.gameState.ToString();
        //currentMatch["gameType"] = levelDef.gameType.ToString();

        //currentMatch["LastShotStartX"] = levelDef.LastShotStartX;
        //currentMatch["LastShotStartY"] = levelDef.LastShotStartY;
        //currentMatch["LastShotEndX"] = levelDef.LastShotEndX;
        //currentMatch["LastShotEndtY"] = levelDef.LastShotEndY;

        //currentMatch["RebuttalTextEnabled"] = levelDef.RebuttalTextEnabled;

        //currentMatch["ShotArrows"] = JsonConvert.SerializeObject(
        //    levelDef.ShotArrows,
        //    Formatting.Indented,
        //    new JsonSerializerSettings
        //    {
        //        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        //    }
        //);

        //currentMatch.SaveAsync().ContinueWith(t =>
        //{
        //    Debug.Log("Canceled: " + t.IsCanceled);
        //    Debug.Log("IsCompleted: " + t.IsCompleted);
        //    Debug.Log("IsFaulted: " + t.IsFaulted);
        //    Debug.Log("Exception: " + t.Exception.ToString());
        //    Debug.Log("InnerException: " + t.Exception.InnerException.ToString());
        //});
    }

    public void loadFriends(Action<string, string> loadFriends)
    {
        PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest(), (result) =>
        {
            foreach (var friend in result.Friends)
            {
                CallOnMainThread(() => loadFriends(friend.Username, friend.FriendPlayFabId));
            }
        }, (error) => LogError(error, "GetFriendsList"));
    }

    public void searchFriends(string searchString, Action<IEnumerable<ButtonAddFriendContent>> populateResults)
    {
        // Search all 4 fields 
        var userNameRequest = new GetAccountInfoRequest() { Username = searchString };
        var emailRequest = new GetAccountInfoRequest() { Email = searchString };
        var idRequest = new GetAccountInfoRequest() { PlayFabId = searchString };
        var displayNameRequest = new GetAccountInfoRequest() { TitleDisplayName = searchString };

        PlayFabClientAPI.GetAccountInfo(userNameRequest, (result) =>
        {
            populateResults(new List<ButtonAddFriendContent>() { new ButtonAddFriendContent(result.AccountInfo.PlayFabId, result.AccountInfo.Username) });
        }, (error) => LogError(error, "GetAccountInfo(userNameRequest"));

        PlayFabClientAPI.GetAccountInfo(emailRequest, (result) =>
        {
            populateResults(new List<ButtonAddFriendContent>() { new ButtonAddFriendContent(result.AccountInfo.PlayFabId, result.AccountInfo.Username) });
        }, (error) => LogError(error, "GetAccountInfo(emailRequest"));

        PlayFabClientAPI.GetAccountInfo(idRequest, (result) =>
        {
            populateResults(new List<ButtonAddFriendContent>() { new ButtonAddFriendContent(result.AccountInfo.PlayFabId, result.AccountInfo.Username) });
        }, (error) => LogError(error, "GetAccountInfo(idRequest"));

        PlayFabClientAPI.GetAccountInfo(displayNameRequest, (result) =>
        {
            populateResults(new List<ButtonAddFriendContent>() { new ButtonAddFriendContent(result.AccountInfo.PlayFabId, result.AccountInfo.Username) });
        }, (error) => LogError(error, "GetAccountInfo(displayNameRequest"));
    }

    private string getGroupId(string friendId)
    {
        List<string> friendAndSelfId = new List<string>() { friendId, CurrentUser.UserId };
        friendAndSelfId.Sort();
        return string.Concat(friendAndSelfId[0], '-', friendAndSelfId[1]);
    }

    public void createMatch(string userIdOfFriend, string usernameOfFriend)
    {
        var createGroupRequest = new CreateSharedGroupRequest()
        {
            SharedGroupId = getGroupId(userIdOfFriend)
        };

        PlayFabClientAPI.CreateSharedGroup(createGroupRequest,
            (creatResult) =>
            {
                Debug.Log("Success: CreateSharedGroup");

                AddSharedGroupMembersRequest addToGroupRequest = new AddSharedGroupMembersRequest()
                {
                    PlayFabIds = new List<string>() { userIdOfFriend },
                    SharedGroupId = creatResult.SharedGroupId                    
                };

                PlayFabClientAPI.AddSharedGroupMembers(addToGroupRequest, 
                    (addResult) =>
                    {
                        Debug.Log("Success: AddSharedGroupMembers");

                        var levelDefinition = new LevelDefinition(CurrentUser.UserId, CurrentUser.UserName, userIdOfFriend, usernameOfFriend);
                        levelDefinition.LevelDefinitionSetDefault();

                        UpdateSharedGroupDataRequest updateDataRequest = new UpdateSharedGroupDataRequest()
                        {
                            Data = new Dictionary<string, string>() { { "LevelDefinition", JsonConvert.SerializeObject(levelDefinition).ToString() } },
                            SharedGroupId = addToGroupRequest.SharedGroupId,
                            Permission = UserDataPermission.Private // TODO: does this work?
                        };

                        PlayFabClientAPI.UpdateSharedGroupData(updateDataRequest, 
                            (result) =>
                            {
                                Debug.Log("Success: UpdateSharedGroupData");
                            }, (error) => LogError(error, "UpdateSharedGroupData")
                        );
                    }, (error) => LogError(error, "AddSharedGroupMembers")
                );
            }, (error) => LogError(error, "CreateSharedGroup")
        );
    }

    public void loadMatch(LevelDefinition levelDefinition)
    {
        NetworkManager.CallOnMainThread(() =>
        {
            levelDef = levelDefinition;

            SceneManager.LoadScene("Friend");
        });
    }

    public void GetMatchesAsync(Action<List<LevelDefinition>> doWithMatches)
    {
        //// TODO: Can this function be cleaned up?
        //ParseQuery<ParseObject> queryLeftPlayer = new ParseQuery<ParseObject>("MatchTest").WhereEqualTo("playerLeftId", ParseUser.CurrentUser.ObjectId);
        //ParseQuery<ParseObject> queryRightPlayer = new ParseQuery<ParseObject>("MatchTest").WhereEqualTo("playerRightId", ParseUser.CurrentUser.ObjectId);
        //ParseQuery<ParseObject> queryBothSides = queryLeftPlayer.Or(queryRightPlayer);
        //return queryBothSides.FindAsync().ContinueWith(t =>
        //{
        //    var matches = new List<Match>();
        //    IEnumerable<ParseObject> results = t.Result;
        //    foreach (ParseObject parseObject in results)
        //    {
        //        matches.Add(getMatchFromParseObject(parseObject));
        //    }
        //    return matches;
        //});

        /// OLD PARSE ABOVE

        PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest(), (result) =>
        {
            foreach (var friend in result.Friends)
            {
                var groupRequest = new GetSharedGroupDataRequest() { SharedGroupId = getGroupId(friend.FriendPlayFabId) };
                PlayFabClientAPI.GetSharedGroupData(groupRequest, (groupResult) =>
                {
                    var matches = new List<LevelDefinition>();
                    // TODO, don't hard code string
                    if (groupResult.Data != null && groupResult.Data.ContainsKey("LevelDefinition"))
                    {
                        var levelDef = JsonConvert.DeserializeObject<LevelDefinition>(groupResult.Data["LevelDefinition"].Value);
                        matches.Add(levelDef);
                    }

                    doWithMatches(matches);
                }, (error) => LogError(error, "GetSharedGroupData"));
            }
        }, (error) => LogError(error, "GetFriendsList"));
    }

    public void LogOut(Func handleLogOutResult, Func handleError)
    {
        var unlinkRequest = new UnlinkCustomIDRequest()
        {
            CustomId = UniqueDeviceGuid.GetValue().ToString()
        };

        PlayFabClientAPI.UnlinkCustomID(unlinkRequest, 
            (result) => 
            {
                CallOnMainThread(handleLogOutResult);
                IsLoggedInWithUsernamePassword = false;
            }, 
            (error) =>
            {
                CallOnMainThread(handleError);
            });
    }

    public void InitializeUserName(string username, string userId)
    {
        CurrentUser.UserName = username;
        CurrentUser.UserId = userId;
    }

    /// <summary>
    /// Sign in with the username and password, also provide an action to do with the result of the sign in.
    /// </summary>
    /// <param name="username">Username to sign in with</param>
    /// <param name="password">Password to sign in with</param>
    /// <param name="processSignInResult">An action that takes a string to process after sign in. The string is null if the sign in was successful</param>
    public void signUpAsync(string username, string password, Action processSuccess, Action<string> processFailure)
    {
        var registerPlayFabUserRequest = new RegisterPlayFabUserRequest()
        {
            TitleId = "8DA7",
            Username = username,
            Password = password,
            RequireBothUsernameAndEmail = false
        };

        PlayFabClientAPI.RegisterPlayFabUser(registerPlayFabUserRequest, (result) => 
        {
            Debug.Log("PlayFab Registered username: " + result.Username);

            // Save this newly registered user to this device ID so they will auto login next time
            var requestToLink = new LinkCustomIDRequest()
            {
                CustomId = UniqueDeviceGuid.GetValue().ToString(),
                ForceLink = true
            };
            PlayFabClientAPI.LinkCustomID(requestToLink, (linkResult) =>
            {
                IsLoggedInWithUsernamePassword = true;
                processSuccess();
            }, (error) => 
            {
                processFailure(error.ErrorMessage);
            });
        },
        (error) => { processFailure(error.ErrorMessage); });
    }

    /// <summary>
    /// Sign in with the username and password, also provide an action to do with the result of the sign in.
    /// </summary>
    /// <param name="username">Username to sign in with</param>
    /// <param name="password">Password to sign in with</param>
    /// <param name="processSignInResult">An action that takes a string to process after sign in. The string is null if the sign in was successful</param>
    public void signInAsync(string username, string password, Action processSuccess, Action<string> processError)
    {
        LoginWithPlayFabRequest request = new LoginWithPlayFabRequest
        {
            Username = username,
            Password = password,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams() { GetUserAccountInfo = true }
        };

        // We are signing in, so the current deviceID doesn't have an associated email/password
        // When we sign in, we need to reset the "unique" device ID to the ID of users who just logged in.
        
        PlayFabClientAPI.LoginWithPlayFab(request, (result) => 
        {
            Debug.Log("PlayFab Logged in username: " + request.Username);
            InitializeUserName(username, result.PlayFabId);
            processSuccess();
            IsLoggedInWithUsernamePassword = true;

            // Save this newly registered user to this device ID so they will auto login next time
            var requestToLink = new LinkCustomIDRequest()
            {
                CustomId = UniqueDeviceGuid.GetValue().ToString(),
                ForceLink = true
            };
            PlayFabClientAPI.LinkCustomID(requestToLink, (linkResult) =>
            {
                IsLoggedInWithUsernamePassword = true;
                processSuccess();
            }, (error) =>
            {
                processError(error.ErrorMessage);
            });

        }, (error) => 
        {
            processError(error.ErrorMessage);
        });
    }

    private void LogError(PlayFabError error, string location = "")
    {
        Debug.Log(location + " PlayFab Error: " + error.ErrorMessage + " Details: " + error.ErrorDetails);
    }
    
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

    public static void CallOnMainThread(Function Func, object Parameter)
    {
        lock (callsLock)
        {
            calls.Add(new CallInfo(Func, Parameter));
        }
    }
    public static void CallOnMainThread(Func func)
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

    void PlayFabLoginThisDevice()
    {
        LoginWithCustomIDRequest request = new LoginWithCustomIDRequest()
        {
            TitleId = "8DA7",
            CreateAccount = true,
            CustomId = UniqueDeviceGuid.GetValue().ToString(),
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams() { GetUserAccountInfo = true }
        };

        PlayFabClientAPI.LoginWithCustomID(request, 
            (result) => 
            {
                string username = null;
                if(result.InfoResultPayload != null && result.InfoResultPayload.AccountInfo != null && !string.IsNullOrEmpty(result.InfoResultPayload.AccountInfo.Username))
                {
                    username = result.InfoResultPayload.AccountInfo.Username;
                    IsLoggedInWithUsernamePassword = true;
                }
                else
                {
                    username = result.PlayFabId;
                    IsLoggedInWithUsernamePassword = false;
                }

                Debug.Log("Logged in user: " + username);
                InitializeUserName(username, result.PlayFabId);
            }, (error) => LogError(error, "LoginWithCustomID"));
    }
}

public static class UniqueDeviceGuid
{
    public static Guid GetValue()
    {
        Guid guid;

        if (File.Exists(Application.persistentDataPath + "/UniqueDeviceGuid.dat"))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/UniqueDeviceGuid.dat", FileMode.Open);
            guid = (Guid)binaryFormatter.Deserialize(file);
            binaryFormatter.Serialize(file, guid);
            file.Close();
        }
        else
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/UniqueDeviceGuid.dat");
            guid = Guid.NewGuid();
            binaryFormatter.Serialize(file, guid);
            file.Close();
        }

        return guid;
    }
    public static void UpdateValueOnDisk(string id)
    {
        Guid guid = new Guid(id);
        FileStream file;
        if (File.Exists(Application.persistentDataPath + "/UniqueDeviceGuid.dat"))
        {
            file = File.Open(Application.persistentDataPath + "/UniqueDeviceGuid.dat", FileMode.Open);
        }
        else
        {
            file = File.Create(Application.persistentDataPath + "/UniqueDeviceGuid.dat");
        }

        BinaryFormatter binaryFormatter = new BinaryFormatter();        
        binaryFormatter.Serialize(file, guid);
        file.Close();
    }
}

