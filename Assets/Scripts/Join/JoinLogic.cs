using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JoinLogic : MonoBehaviour
{
    public Renderer[] mSelectors;
    public int[] mSelectedControllers;
    public int mMinPlayersToStart = 2;
    public TutorialMenuUI TutorialUi;

    private int mIdx = 0;
    private bool playersReady = false;


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < mSelectedControllers.Length; ++i)
        {
            mSelectedControllers[i] = -1;
        }

        DontDestroyOnLoad(this.gameObject);

        TutorialUi.enabled = false;
        TutorialUi.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (playersReady)
        {
            return;
        }

        CheckPlayer(0);
        CheckPlayer(1);
        CheckPlayer(2);
        CheckPlayer(3);

        if (mIdx >= mMinPlayersToStart && Input.GetButtonDown("Start"))
        {
            playersReady = true;
            TutorialUi.gameObject.SetActive(true);
        }
    }

    private bool IsControllerUsed(int aIdx)
    {
        for (int i = 0; i < mSelectedControllers.Length; ++i)
        {
            if (mSelectedControllers[i] == aIdx)
            {
                return true;
            }
        }

        return false;
    }

    private void SelectPlayer(int aIdx)
    {
        mSelectedControllers[mIdx] = aIdx;
        mSelectors[mIdx].enabled = true;

        ++mIdx;
    }

    private void CheckPlayer(int aIdx)
    {
        if (!IsControllerUsed(aIdx) && Input.GetButtonDown("AButtton " + aIdx))
        {
            SelectPlayer(aIdx);
        }
    }
}
