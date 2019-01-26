﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchItem : Pickable
{
    private readonly float CooldownTime = 10.0f;

    public float DropDistance = 0.2f;
    public float DropForce = 40.0f;

    private float Cooldown = 0.0f;
    private List<GameObject> Players;
    private bool enableCooldown = false;

    public override void OnPickedUp(PlayerController trigger)
    {
        base.OnPickedUp(trigger);

        GetComponent<SphereCollider>().enabled = true;
        GetComponent<CapsuleCollider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
    }

    public override void OnAction(PlayerController trigger)
    {
        base.OnAction(trigger);

        trigger.DropHeldObject();
    }

    public override void OnDropped(PlayerController trigger)
    {
        base.OnDropped(trigger);

        Cooldown = CooldownTime;

        GetComponent<SphereCollider>().enabled = false;
        GetComponent<CapsuleCollider>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;

        float y = transform.position.y;
        Vector3 newPosition = trigger.transform.position + (trigger.transform.forward * DropDistance);
        newPosition.y = y;
        transform.position = newPosition;
        GetComponent<Rigidbody>().AddForce(trigger.transform.forward * DropForce);

        DecrementLife();

        enableCooldown = true;
    }

    public override void OnDemoBegin()
    {
        enableCooldown = false;

        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        rigidbody.Sleep();

        rigidbody.isKinematic = true;

        GetComponent<CapsuleCollider>().enabled = false;
    }

    void Start()
    {
        Players = new List<GameObject>();
    }

    void Update()
    {
        if (!IsBeingHeld() && enableCooldown)
        {
            Cooldown -= Time.deltaTime;

            if (Cooldown <= 0.0f)
            {
                enableCooldown = false;
                DecrementLife();
                DoDestroy();
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
            collision.gameObject.GetComponent<PlayerController>().TryPickObject(gameObject);
        }
    }


    private void OnDestroy()
    {
        
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
