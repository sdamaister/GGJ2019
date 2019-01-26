using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchItem : Pickable
{
    private readonly float CooldownTime = 10.0f;

    public float DropDistance = 1.1f;
    public float DropForce = 40.0f;

    private float Cooldown = 0.0f;
    private List<GameObject> Players;

    public override void OnPickedUp(PlayerController trigger)
    {
        GetComponent<SphereCollider>().enabled = true;
        GetComponent<CapsuleCollider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
    }

    public override void OnAction(PlayerController trigger)
    {
        trigger.DropHeldObject();
    }

    public override void OnDropped(PlayerController trigger)
    {
        Cooldown = CooldownTime;

        GetComponent<SphereCollider>().enabled = false;
        GetComponent<CapsuleCollider>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;

        transform.position = transform.position + (trigger.transform.right * DropDistance);
        GetComponent<Rigidbody>().AddForce(trigger.transform.forward * DropForce);

        DecrementLife();
    }

    void Start()
    {
        Players = new List<GameObject>();
    }

    void Update()
    {
        if (!IsBeingHeld())
        {
            Cooldown -= Time.deltaTime;

            if (Cooldown <= 0.0f)
            {
                Destroy(transform.gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerLifeController>().DecrementLife = false;
            Players.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerLifeController>().DecrementLife = true;
            Players.Remove(other.gameObject);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().PickObject(gameObject);
        }
    }


    private void OnDestroy()
    {
        DecrementLife();
    }

    private void DecrementLife()
    {
        foreach (GameObject player in Players)
        {
            player.GetComponent<PlayerLifeController>().DecrementLife = true;
        }

        Players.Clear();
    }
}
