using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Settings : MonoBehaviour {

    public Button LogoutButton;
    public Button BackButton;
    public Image PendingImage;
    public Text ButtonText;
    private NetworkManager _networkManager;
    public void Start()
    {
        _networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        BackButton.onClick.AddListener(() => { SceneManager.LoadScene("StartMenu"); });
        if (_networkManager.CurrentUser != null)
        {
            LogoutButton.enabled = true;
            ButtonText.text = "Logout";
        }
        else
        {
            LogoutButton.enabled = false;
            ButtonText.text = "Not Signed In";
        }
    }

    public void onLogoutClicked()
    {
        LogoutButton.enabled = false;
        PendingImage.enabled = true;
        if (_networkManager.CurrentUser == null)
        {
            SceneManager.LoadScene("CreateAccount");
        }
        else
        {
            _networkManager.CurrentUser.LogOut(() =>
            {
                PendingImage.enabled = false;
                LogoutButton.enabled = false;
                ButtonText.text = "Not Signed In";
                // TODO maybe show progress to make sure a user isn't doing anything when they think they are logged out
                // Handle errors if the logout didn't work.
            });
            
        }
    }
}
