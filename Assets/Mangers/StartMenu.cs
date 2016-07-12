using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Parse;
using UnityEngine.SceneManagement;
using Assets.Mangers;

public class StartMenu : MonoBehaviour
{
    public Button playHumanButton;
    public Button playComputerButton;
    public Button playFriendButton;
    public Button settingsButton;

	// Use this for initialization
	void Start ()
    {
        //playHumanButton = GetComponent<Button>();
        //playComputerButton = GetComponent<Button>();
        //playFriendButton = GetComponent<Button>();
        //settingsButton = GetComponent<Button>();
    } // End Start

    public void onHumanClicked()
    {
        LevelDefinition.gameType = GameType.Local;
        SceneManager.LoadScene("Human");
    } // End onHumanClicked    
    public void onComputerClicked(){
        LevelDefinition.LevelDefinitionSetDefault();
        LevelDefinition.gameType = GameType.Computer;
        LevelDefinition.WallPosition = -1;
        SceneManager.LoadScene("Computer");
    } // End onHumanClicked

    public void onSettingsClicked()
    {
        SceneManager.LoadScene("Settings");
    } // End onSettingsClicked

    public void onFriendClicked()
    {
        if(ParseUser.CurrentUser != null)
        {
            SceneManager.LoadScene("CurrentMatches");
        }
        else
        {
            SceneManager.LoadScene("CreateAccount");
        }
    } // End onFriendClicked
}
