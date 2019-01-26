using UnityEngine;

public class StickItem : Pickable
{
    public float DropDistance = 1.0f;
    public float DropSpeed = 10.0f;

    public override void OnPickedUp(PlayerController trigger)
    {
        GetComponent<Collider>().isTrigger = true;
        GetComponent<Rigidbody>().isKinematic = true;
    }

    public override void OnAction(PlayerController trigger)
    {
        trigger.DropHeldObject();

        transform.position = transform.position + (trigger.transform.forward * DropDistance);

        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = trigger.transform.forward * DropSpeed;
        rigidbody.AddTorque(Vector3.forward * 10);
        rigidbody.useGravity = false;
    }

    public override void OnDropped(PlayerController trigger)
    {
        GetComponent<Collider>().isTrigger = false;
        GetComponent<Rigidbody>().isKinematic = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().Stun();
        }

        Destroy(gameObject);
    }
}
