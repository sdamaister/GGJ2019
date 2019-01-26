using UnityEngine;

public class StickItem : Pickable
{
    public float DropDistance = 1.0f;
    public float DropSpeed = 10.0f;

    private GameObject thrower;

    public override void OnPickedUp(PlayerController trigger)
    {
        Collider collider = GetComponent<Collider>();
        collider.enabled = true;
        collider.isTrigger = true;
        GetComponent<Rigidbody>().isKinematic = true;

        thrower = trigger.gameObject;
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

    public override void OnDemoBegin()
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        rigidbody.Sleep();

        GetComponent<Collider>().enabled = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject != thrower)
        {
            if (collision.gameObject.tag == "Player")
            {
                collision.gameObject.GetComponent<PlayerController>().Stun(GetComponent<Rigidbody>().velocity.normalized);
            }

            DoDestroy();
        }
    }
}
