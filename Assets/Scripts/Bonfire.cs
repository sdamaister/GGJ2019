using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Bonfire : MonoBehaviour
{
    public int mBonfireIdx = 0;
    private bool mHasWon;

    void Start() {
        mHasWon = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayerController lPlayerControllerComp = other.GetComponent<PlayerController>();
            Assert.IsNotNull(lPlayerControllerComp);
            if ( lPlayerControllerComp != null &&
                (lPlayerControllerComp.mPlayerIdX == mBonfireIdx) )
            {
                Debug.Log("Player " + mBonfireIdx + " enters bonfire!");

                Pickable lPickable = lPlayerControllerComp.GetCurrentPickup();
                if ((lPickable != null) && (lPickable.GetType() == typeof(TorchItem)))
                {
                    // Debug.Log("Player " + mBonfireIdx + " wins!");
                    mHasWon = true;
                }
            }
        }
    }

    public bool HasWon() {
        return mHasWon;
    }

    public void Reset() {
        mHasWon = false;
    }
}
