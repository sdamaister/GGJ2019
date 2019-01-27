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
    private List<int> results;
    private int mCurrentRound = 0;

    private EGameState mGameState;
    private float mGameStateClock;
    // Start is called before the first frame update
    void Start()
    {
        mGameState = EGameState.eGameCelebration;
        mGameStateClock = 0.4f;

        results = new List<int>();
        NewRound();
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
                    NewRound();
                }

                foreach (Bonfire tent in tents) {
                    if (tent.HasWon()) {
                        int playerId = tent.mBonfireIdx;
                        Debug.Log("Player " + playerId + " wins!");
                        results.Add(playerId);
                        mGameState = EGameState.eGameCelebration;
                        mGameStateClock = 3f;

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
                if (clockIsDone) {
                    NewRound();
                }
                break;
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
        mGameStateClock = 3f;
    }
    
    private void StartRound() {
        mGameState = EGameState.eGameRunning;
        foreach (PlayerController player in players) {
            player.ReadyToPlay();
        }
    }
}
