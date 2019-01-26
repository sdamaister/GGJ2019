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
        mPickupObject.GetComponent<Pickable>().OnDemoBegin();
    }
    
    private void OnDestroy()
    {
        Pickable pickable = mPickupObject.GetComponent<Pickable>();
        if (SpawnedFrom != null && pickable.Spawner != null)
        {
            pickable.Spawner.ItemPickedUp(SpawnedFrom);
        }
    }
}
