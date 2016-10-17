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
        if(_networkManager.IsLoggedInWithUsernamePassword)
        {
            SceneManager.LoadScene("CurrentMatches");
        }
        else
        {
            SceneManager.LoadScene("CreateAccount");
        }
    }
}
