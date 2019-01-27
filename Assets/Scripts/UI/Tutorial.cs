using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    Texture mTexture;

    // Start is called before the first frame update
    void Awake()
    {
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

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
