﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreateAccount : MonoBehaviour
{
    public Button SignInButton;
    public Button SignUpButton;
    public InputField UserNameField;
    public InputField PasswordField;
    public Text ErrorText;
    private NetworkManager _networkManager;

    // Use this for initialization
    void Start ()
    {
        _networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        SignInButton.onClick.AddListener(SignInButtonBehavior);
        SignUpButton.onClick.AddListener(SignUpButtonBehavior);

        ErrorText.enabled = false;
    }

	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            LevelManager.quitPlaying();
        }
    }

    public void SignUpButtonBehavior()
    {
        _networkManager.signUpAsync(UserNameField.text, PasswordField.text, (string errorMessage) =>
        {
            if (errorMessage == null)
            {
                NetworkManager.Call(HideErrorText);
                NetworkManager.Call(LoadCreateMatch);
            }
            else
            {
                NetworkManager.Call(ShowErrorText, errorMessage);
            }
        });
    }

    public void SignInButtonBehavior()
    {
        _networkManager.signInAsync(UserNameField.text, PasswordField.text, (string errorMessage) =>
        {
            if (errorMessage == null)
            {
                NetworkManager.Call(HideErrorText);
                NetworkManager.Call(LoadCreateMatch);
            }
            else
            {
                NetworkManager.Call(ShowErrorText, errorMessage);
            }
        });
    }

    private void LoadFriendSearch()
    {
        SceneManager.LoadScene("FriendSearch");
    }

    private void LoadCreateMatch()
    {
        SceneManager.LoadScene("CreateMatch");
    }

    public void ShowErrorText(object error)
    {
        Debug.Log("ShowErrorText: " + (string)error);
        ErrorText.enabled = true;
        ErrorText.text = (string)error;
    }
    private void HideErrorText()
    {
        if (ErrorText != null && ErrorText.enabled)
        {
            ErrorText.enabled = false;
        }
    }
}
