using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;

[RequireComponent(typeof(PlayerLifeController))]
public class PlayerController : MonoBehaviour
{
    public float mSpeed = 1.0f;
    public float MinSpeedFactor = 0.4f;
    public float MaxSpeedFactor = 1.0f;
    public float mAttatchOffset = 0.5f;

    private Rigidbody mRigidbody;
    private PlayerLifeController playerLife;
    private bool enableMovement = true;

    private GameObject mCurrentPickup;

    private float mJoystickMovementEpsilon = 0.1f;
    private float mJoystickLookEpsilon     = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        mRigidbody = GetComponent<Rigidbody>();
        Assert.IsNotNull(mRigidbody);

        playerLife = GetComponent<PlayerLifeController>();
        Assert.IsNotNull(playerLife);

        playerLife.OnDie += OnPlayerDied;
    }

    private void Update()
    {
        float lLookHorizontal = Input.GetAxis("Mouse X");
        float lLookVertical   = Input.GetAxis("Mouse Y");

        if (enableMovement && ( ((Mathf.Abs(lLookHorizontal) >= mJoystickLookEpsilon)) || (Mathf.Abs(lLookVertical) >= mJoystickLookEpsilon)))
        {
            transform.forward = Vector3.Normalize(new Vector3(lLookHorizontal, 0.0f, lLookVertical));
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float lMoveHorizontal = Input.GetAxis("Horizontal");
        float lMoveVertical = Input.GetAxis("Vertical");

        if (enableMovement && ((Mathf.Abs(lMoveHorizontal) >= mJoystickMovementEpsilon) || (Mathf.Abs(lMoveVertical) >= mJoystickMovementEpsilon)))
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
            mCurrentPickup.transform.parent        = transform;
            mCurrentPickup.transform.localPosition = transform.forward * mAttatchOffset;
            mCurrentPickup.transform.rotation      = transform.rotation;

            mCurrentPickup.GetComponent<Collider>().enabled = false;
            mCurrentPickup.GetComponent<Rigidbody>().isKinematic = true;
        }

        Destroy(other.gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (transform.forward * 3.0f));
    }
}
