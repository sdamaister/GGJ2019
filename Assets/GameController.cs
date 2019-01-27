using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    private enum EGameState {
        eGameReadyAnim,
        eGameRunning,
        eGameCelebration
    }

    public List<PlayerController> players;
    public List<Bonfire> tents;
    public List<GameObject> thermos;
    public GameOverlayUI ui;
    public CameraDirector camera;
    private List<int> results;
    private int mCurrentRound = 0;

    private EGameState mGameState;
    private float mGameStateClock;

    private bool gameFinished = false;
    private int gameWinner = -1;
    // Start is called before the first frame update
    void Start()
    {
        mGameState = EGameState.eGameCelebration;
        mGameStateClock = 0.0f;

        for (int i = 0; i < players.Count; ++i)
        {
            players[i].enabled = false;
        }

        GameObject lGameObject = GameObject.FindWithTag("JoinLogic");
        if (lGameObject != null)
        {
            JoinLogic logic = lGameObject.GetComponent<JoinLogic>();

            if (logic != null)
            {
                for (int i = 0; i < logic.mSelectedControllers.Length; ++i)
                {
                    if (logic.mSelectedControllers[i] != -1)
                    {
                        players[i].gameObject.SetActive(true);
                        players[i].SetInputControllerIdx(logic.mSelectedControllers[i]);
                    }
                    else
                    {
                        players[i].gameObject.SetActive(false);
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < players.Count; ++i)
            {
                players[i].gameObject.SetActive(true);
            }
        }

        results = new List<int>();

        foreach (GameObject thermo in thermos) {
            thermo.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (mGameStateClock > 0) {
            mGameStateClock -= Time.deltaTime;
        }
        bool clockIsDone = (mGameStateClock <= 0);

        switch(mGameState) {
            case EGameState.eGameReadyAnim:
                if (clockIsDone) {
                    StartRound();
                }
                break;

            case EGameState.eGameRunning:
                bool allDead = true;
                foreach (PlayerController player in players) {
                    if (!player.IsDead()) {
                        allDead = false;
                        break;
                    }
                }
                if (allDead) {
                    Debug.Log("Round ends in draw!");
                    results.Add(-1);
                    GameEnd();
                }

                foreach (Bonfire tent in tents) {
                    if (tent.HasWon()) {
                        int playerId = tent.mBonfireIdx;
                        Debug.Log("Player " + playerId + " wins!");
                        results.Add(playerId);
                        GameEnd();
                        camera.LookAtPlayer(playerId);
                        // make other players stop
                        /*
                        foreach (PlayerController player in players) {
                            if (player.mPlayerIdX != playerId) {
                                player.Die();
                            }
                        }
                        */
                    }
                }
                break;
            
            case EGameState.eGameCelebration:
                // zoom in into winning player
                // make it bounce, or whatever
                if (clockIsDone && !gameFinished) {
                    NewRound();
                }
                break;
        }
    }

    private void GameEnd () {
        mGameState = EGameState.eGameCelebration;
        mGameStateClock = 3f;
        foreach (GameObject thermo in thermos) {
            thermo.SetActive(false);
        }

        int[] playerWins = new int[4];
        playerWins[0] = 0;
        playerWins[1] = 0;
        playerWins[2] = 0;
        playerWins[3] = 0;
        foreach (int result in results) {
            if (result >= 0) {
                playerWins[result]++;
            }
        }
        for (int i = 0; i < 4; i++) {
            if (playerWins[i] >= 4) {
                Debug.Log("Player " + i + " has won 4 rounds, and wins the game!");
                gameFinished = true;
                gameWinner = i;
                ui.ShowVictory();
                break;
            }
        }

        for (int i = 0; i < players.Count; ++i)
        {
            if (players[i].gameObject.activeInHierarchy)
            {
                players[i].StopWalking();
                players[i].enabled = false;
            }
        }
    }

    private void NewRound () {
        mCurrentRound++;

        // tell round show thing to show current round
        Debug.Log("Round " + mCurrentRound + ", fight!");
        foreach (PlayerController player in players) {
            player.Reset();
            player.DoStartAnim();
        }
        foreach (Bonfire tent in tents) {
            tent.Reset();
        }
        mGameState = EGameState.eGameReadyAnim; 
        camera.LookAtPlayer(-1);
        mGameStateClock = 4f;
        ui.ShowRound(mCurrentRound, results);
    }
    
    private void StartRound() {
        for (int i = 0; i < players.Count; ++i)
        {
            if (players[i].gameObject.activeInHierarchy)
            {
                players[i].enabled = true;
            }
        }

        mGameState = EGameState.eGameRunning;
        foreach (PlayerController player in players) {
            player.ReadyToPlay();
        }
        foreach (GameObject thermo in thermos) {
            thermo.SetActive(true);
        }
    }

    private void GameReset() {
        camera.LookAtPlayer(-1);
        mCurrentRound = 0;
        mGameState = EGameState.eGameCelebration;
        mGameStateClock = 0.0f;
        results = new List<int>();
        foreach (GameObject thermo in thermos) {
            thermo.SetActive(false);
        }
    }
}
