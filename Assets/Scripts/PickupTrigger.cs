using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupTrigger : MonoBehaviour
{
    public GameObject SpawnedFrom;
    public GameObject mPickupObject;

    private void OnDestroy()
    {
        ItemSpawnController isc = FindObjectOfType<ItemSpawnController>();
        if (SpawnedFrom != null && isc != null)
        {
            isc.ItemPickedUp(SpawnedFrom);
        }
    }
}
