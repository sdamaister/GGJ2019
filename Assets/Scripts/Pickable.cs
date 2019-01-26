using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pickable : MonoBehaviour
{
    public ItemSpawnController Spawner;

    private bool held = false;

    public virtual void OnPickedUp(PlayerController trigger)
    {
        held = true;
    }
    public virtual void OnAction(PlayerController trigger) {}

    public virtual void OnDropped(PlayerController trigger)
    {
        held = false;
    }

    public virtual void DoDestroy()
    {
        Spawner.ItemDestroyed(gameObject);
    }

    public abstract void OnDemoBegin();

    protected bool IsBeingHeld()
    {
        return held;
    }
}
