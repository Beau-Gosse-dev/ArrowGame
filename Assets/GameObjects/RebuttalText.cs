using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.Mangers;

public static class RebuttalText {
        
    public static void Enable()
    {
        //this.GetComponent<Text>().enabled = true;

        GameObject text = GameObject.Find("RebuttalText");
        text.GetComponent<Text>().enabled = true;
        LevelDefinition.RebuttleTextEnabled = true;
    }

    public static void setEnabled(bool enabled)
    {
        //this.GetComponent<Text>().enabled = false;
        GameObject text = GameObject.Find("RebuttalText");
        text.GetComponent<Text>().enabled = enabled;
        LevelDefinition.RebuttleTextEnabled = enabled;
    }
}
