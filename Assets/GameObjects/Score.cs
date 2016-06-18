using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Score : MonoBehaviour {

    private int scoreLeft;
    private int scoreRight;
    public int ScoreLeft
    {
        get
        {
            return scoreLeft;
        }
        set
        {
            scoreLeft = value;

            // Update the score text on the display
            GetComponent<Text>().text = scoreLeft + " : " + scoreRight;
        }
    }

    public int ScoreRight
    {
        get
        {
            return scoreRight;
        }
        set
        {
            scoreRight = value;

            // Update the score text on the display
            GetComponent<Text>().text = scoreLeft + " : " + scoreRight;
        }
    }
}
