using UnityEngine;

public class StickItem : Pickable
{
    public override void OnPickedUp(PlayerController trigger)
    {
        GetComponent<Collider>().isTrigger = true;
        GetComponent<Rigidbody>().isKinematic = true;
    }

    public override void OnAction(PlayerController trigger)
    {
        // TODO: Throw it
    }

    public override void OnDropped(PlayerController trigger)
    {
        GetComponent<Collider>().isTrigger = false;
        GetComponent<Rigidbody>().isKinematic = false;
    }
}
