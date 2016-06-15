using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour {
    	
	// Update is called once per frame
	void Update () {
        DrawQuad(new Rect(new Vector2(3, 3), new Vector2(3, 3)), Color.red);
    }

    void DrawQuad(Rect position, Color color)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        GUI.skin.box.normal.background = texture;
        GUI.Box(position, GUIContent.none);
    }

}
