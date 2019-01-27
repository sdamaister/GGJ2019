using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialMenuUI : MonoBehaviour
{
    private float countdown = 20.0f;
    
    // Update is called once per frame
    void Update()
    {
        countdown -= Time.deltaTime;

        if (countdown <= 0.0f)
        {
            LoadLevel();
        }
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
    }
}
