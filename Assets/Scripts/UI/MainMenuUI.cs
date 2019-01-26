using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// UI.Text.text example
//
// A Space keypress changes the message shown on the screen.
// Two messages are used.
//
// Inside Awake a Canvas and Text are created.

public class MainMenuUI : MonoBehaviour
{
    private Text mText;

    void Awake()
    {
        Font lFont;
        lFont = (Font) Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");

        // Create Canvas GameObject.
        GameObject lCanvasGO = new GameObject();
        lCanvasGO.name = "Canvas";
        lCanvasGO.AddComponent<Canvas>();
        lCanvasGO.AddComponent<CanvasScaler>();
        lCanvasGO.AddComponent<GraphicRaycaster>();

        // Get canvas from the GameObject.
        Canvas lCanvas;
        lCanvas = lCanvasGO.GetComponent<Canvas>();
        lCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

        // Create the Text GameObject.
        GameObject lTextGO = new GameObject();
        lTextGO.transform.parent = lCanvasGO.transform;
        lTextGO.AddComponent<Text>();

        // Set Text component properties.
        mText = lTextGO.GetComponent<Text>();
        mText.font = lFont;
        mText.text = "Press any button to start";
        mText.fontSize = 48;
        mText.alignment = TextAnchor.MiddleCenter;

        // Provide Text position and size using RectTransform.
        RectTransform lRectTransform;
        lRectTransform = mText.GetComponent<RectTransform>();
        lRectTransform.localPosition = new Vector3(0, 0, 0);
        lRectTransform.sizeDelta = new Vector2(600, 200);
    }

    private void Update()
    {
        if (Input.anyKey)
        {
            SceneManager.LoadScene("JoinMenu", LoadSceneMode.Single);
        }
    }
}