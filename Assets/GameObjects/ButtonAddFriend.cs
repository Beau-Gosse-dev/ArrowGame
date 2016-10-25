using UnityEngine;
using UnityEngine.UI;

public class ButtonAddFriend : MonoBehaviour
{
    public Button button;
    public Image iconOfFriend;
    public string userIdOfFriend;
    public string usernameOfFriend;
    private NetworkManager _networkManager;

    void Awake()
    {
        if (NetworkManager.StartFromBeginingIfNotStartedYet())
        {
            return;
        }
        _networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
    }

    public void addFriend()
    {
        _networkManager.addFriend(userIdOfFriend, () =>
        {

            button.enabled = false;
            transform.FindChild("Image");
            var children = gameObject.GetComponentsInChildren(typeof(Image));
            foreach (var child in children)
            {
                if (child.name == "Image")
                {
                    ((Image)child).color = new Color(0, 1, 0);
                }
            }
        });
    }
}