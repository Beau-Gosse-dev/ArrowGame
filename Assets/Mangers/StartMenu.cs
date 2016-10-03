using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Assets.Mangers;
using Assets.Networking;

public class StartMenu : MonoBehaviour
{
    public Button playHumanButton;
    public Button playComputerButton;
    public Button playFriendButton;
    public Button settingsButton;
    private NetworkManager _networkManager;

    void Awake()
    {
        if (NetworkManager.StartFromBeginingIfNotStartedYet())
        {
            return;
        }
        _networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
    }

    void Start ()
    {
        // TODO: Apparently you can't call any parse stuff until previous scene has finished. Maybe we need to wrap the Parse stuff in a outer scene that loads this one.
        _networkManager.CurrentUser = new GameUser();
    }

    public void onHumanClicked()
    {
        _networkManager.levelDef.gameType = GameType.Local;
        SceneManager.LoadScene("Human");
    }

    public void onComputerClicked(){
        _networkManager.levelDef.LevelDefinitionSetDefault();
        _networkManager.levelDef.gameType = GameType.Computer;
        _networkManager.levelDef.WallPosition = -1;
        SceneManager.LoadScene("Computer");
    }

    public void onSettingsClicked()
    {
        SceneManager.LoadScene("Settings");
    }

    public void onFriendClicked()
    {
        if(_networkManager.CurrentUser.IsLoggedIn)
        {
            SceneManager.LoadScene("CurrentMatches");
        }
        else
        {
            SceneManager.LoadScene("CreateAccount");
        }
    }
}
