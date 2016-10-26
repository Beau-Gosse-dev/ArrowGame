using UnityEngine;
using UnityEngine.UI;

public class RebuttalText : MonoBehaviour {

    private NetworkManager _networkManager;

    public void Awake()
    {
        if(NetworkManager.StartFromBeginingIfNotStartedYet())
        {
            return;
        }
        _networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
    }

    public void Enable()
    {
        //this.GetComponent<Text>().enabled = true;

        GameObject text = GameObject.Find("RebuttalText");
        text.GetComponent<Text>().enabled = true;
        _networkManager.levelDef.RebuttalTextEnabled = true;
    }

    public void setEnabled(bool enabled)
    {
        //this.GetComponent<Text>().enabled = false;
        GameObject text = GameObject.Find("RebuttalText");
        text.GetComponent<Text>().enabled = enabled;
        _networkManager.levelDef.RebuttalTextEnabled = enabled;
    }
}
