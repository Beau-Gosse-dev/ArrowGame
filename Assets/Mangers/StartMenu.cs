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

    void Start ()
    {
        _networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        // TODO: Apparently you can't call any parse stuff until previous scene has finished. Maybe we need to wrap the Parse stuff in a outer scene that loads this one.
        _networkManager.CurrentUser = new GameUser();
    }

    public void onHumanClicked()
    {
        LevelDefinition.gameType = GameType.Local;
        SceneManager.LoadScene("Human");
    }

    public void onComputerClicked(){
        LevelDefinition.LevelDefinitionSetDefault();
        LevelDefinition.gameType = GameType.Computer;
        LevelDefinition.WallPosition = -1;
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
