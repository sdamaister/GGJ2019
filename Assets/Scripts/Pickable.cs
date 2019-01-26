using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pickable : MonoBehaviour
{
    public virtual void OnPickedUp(PlayerController trigger) {}
    public virtual void OnAction(PlayerController trigger) {}
    public virtual void OnDropped(PlayerController trigger) {}

    protected bool IsBeingHeld()
    {
        return transform.parent != null && transform.parent.tag == "Player";
    }
}
