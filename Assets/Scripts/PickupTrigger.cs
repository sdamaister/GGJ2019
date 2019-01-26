using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupTrigger : MonoBehaviour
{
    public GameObject SpawnedFrom;
    public GameObject mPickupObject;

    void Awake()
    {
        SphereCollider collider = gameObject.AddComponent<SphereCollider>();
        collider.isTrigger = true;
    }

    public void SetPickupObject(GameObject item)
    {
        mPickupObject = item;
        mPickupObject.GetComponent<Collider>().enabled = false;
    }
    
    private void OnDestroy()
    {
        mPickupObject.GetComponent<Collider>().enabled = true;
        ItemSpawnController isc = FindObjectOfType<ItemSpawnController>();
        if (SpawnedFrom != null && isc != null)
        {
            isc.ItemPickedUp(SpawnedFrom);
        }
    }
}
