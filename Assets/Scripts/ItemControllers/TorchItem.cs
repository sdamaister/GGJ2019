using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchItem : Pickable
{
    private readonly float CooldownTime = 10.0f;
    
    private float Cooldown = 0.0f;
    private List<GameObject> Players;

    public override void OnPickedUp(PlayerController trigger)
    {
        GetComponent<Collider>().isTrigger = true;
        GetComponent<Rigidbody>().isKinematic = true;
    }

    public override void OnAction(PlayerController trigger)
    {
        // TODO: Drop object?
    }

    public override void OnDropped(PlayerController trigger)
    {
        Cooldown = CooldownTime;

        GetComponent<Collider>().isTrigger = false;
        GetComponent<Rigidbody>().isKinematic = false;
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

    private void OnDestroy()
    {
        foreach (GameObject player in Players)
        {
            player.GetComponent<PlayerLifeController>().DecrementLife = true;
        }
    }


}
