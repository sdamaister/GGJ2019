using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupTrigger : MonoBehaviour
{
    public GameObject SpawnedFrom;
    public GameObject mPickupObject;

    private void OnDestroy()
    {
        FindObjectOfType<ItemSpawnController>().ItemPickedUp(SpawnedFrom);
    }
}
