using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Assets.Mangers;

public class StartMenu : MonoBehaviour
{
    public Button playHumanButton;
    public Button playComputerButton;
    public Button playFriendButton;
    public Button settingsButton;

	void Start ()
    {
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
        var networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        if(networkManager.CurrentUser.IsLoggedIn)
        {
            SceneManager.LoadScene("CurrentMatches");
        }
        else
        {
            SceneManager.LoadScene("CreateAccount");
        }
    }
}
