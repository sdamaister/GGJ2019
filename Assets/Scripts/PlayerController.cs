using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;

[RequireComponent(typeof(PlayerLifeController))]
public class PlayerController : MonoBehaviour
{
    public float   mSpeed = 1.0f;
    public float   MinSpeedFactor = 0.4f;
    public float   MaxSpeedFactor = 1.0f;
    public Vector3 mAttatchOffset = new Vector3(0.5f, 0.0f, 0.0f);

    private Rigidbody mRigidbody;
    private PlayerLifeController playerLife;
    private bool enableMovement = true;

    private GameObject mCurrentPickup;


    // Start is called before the first frame update
    void Start()
    {
        mRigidbody = GetComponent<Rigidbody>();
        Assert.IsNotNull(mRigidbody);

        playerLife = GetComponent<PlayerLifeController>();
        Assert.IsNotNull(playerLife);

        playerLife.OnDie += OnPlayerDied;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float lMoveHorizontal = Input.GetAxis("Horizontal");
        float lMoveVertical = Input.GetAxis("Vertical");

        if (enableMovement && ((lMoveHorizontal) != 0.0f || (lMoveVertical != 0.0f)))
        {
            mRigidbody.velocity = Vector3.Normalize(new Vector3(lMoveHorizontal, 0.0f, lMoveVertical)) *
                                  GetSpeed() * Time.deltaTime;
        }
        else
        {
            mRigidbody.velocity = Vector3.zero;
        }
    }
    
    private float GetSpeed()
    {
        // TODO: Maybe use a Bezier curve
        float speedFactor = Mathf.Lerp(MinSpeedFactor, MaxSpeedFactor, playerLife.Life * 0.01f);
        return speedFactor * mSpeed;
    }

    private void OnPlayerDied()
    {
        enableMovement = false;
        // TODO: Throw held objects
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Pickup")
        {
            mCurrentPickup = Instantiate(other.GetComponent<PickupTrigger>().mPickupObject, transform);
            mCurrentPickup.transform.parent = transform;
            mCurrentPickup.transform.localPosition = mAttatchOffset;
            mCurrentPickup.GetComponent<Collider>().enabled = false;
            mCurrentPickup.GetComponent<Rigidbody>().isKinematic = true;
        }

        Destroy(other.gameObject);
    }
}
