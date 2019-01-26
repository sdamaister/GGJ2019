using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;

[RequireComponent(typeof(PlayerLifeController))]
[RequireComponent(typeof(InputManager))]
public class PlayerController : MonoBehaviour
{
    public int   mPlayerIdX     = 0;
    public float mSpeed         = 150.0f;
    public float MinSpeedFactor = 0.4f;
    public float MaxSpeedFactor = 1.0f;
    public float mAttatchOffset = 0.5f;
    public float DropDistance = 1.1f;
    public float DropForce = 40.0f;

    private Rigidbody mRigidbody;
    private PlayerLifeController playerLife;
    private InputManager mInputManager;
    private bool enableMovement = true;

    private Pickable mCurrentPickup;

    public void PickObject(GameObject pickup)
    {
        pickup.transform.parent = transform;
        pickup.transform.localPosition = transform.forward * mAttatchOffset;
        pickup.transform.rotation = transform.rotation;
        pickup.GetComponent<Collider>().isTrigger = true;
        pickup.GetComponent<Rigidbody>().isKinematic = true;

        mCurrentPickup = pickup.GetComponent<Pickable>();
        Assert.IsNotNull(mCurrentPickup, "Object " + pickup.name + " doesn't have a 'Pickable' component");

        mCurrentPickup.OnPickedUp(this);
    }

    public void DropHeldObject()
    {
        mCurrentPickup.OnDropped(this);
        mCurrentPickup.transform.parent = null;

        mCurrentPickup.transform.position = transform.position + (transform.right * DropDistance);
        mCurrentPickup.GetComponent<Rigidbody>().AddForce(transform.forward * DropForce);
        mCurrentPickup = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        mRigidbody = GetComponent<Rigidbody>();
        Assert.IsNotNull(mRigidbody);

        playerLife = GetComponent<PlayerLifeController>();
        Assert.IsNotNull(playerLife);

        mInputManager = GetComponent<InputManager>();
        Assert.IsNotNull(mInputManager);

        playerLife.OnDie += OnPlayerDied;
    }

    private void Update()
    {
        if (enableMovement)
        {
            Vector3 lLookVector = mInputManager.GetLookVector();
            if (lLookVector.magnitude > 0.0f)
            {
                transform.forward = lLookVector;
            }

            if (mInputManager.IsRightTriggerPressed() && mCurrentPickup != null)
            {
                mCurrentPickup.OnAction(this);
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (enableMovement)
        {
            Vector3 lMovementVec = mInputManager.GetMoveVector();
            mRigidbody.velocity = (lMovementVec.magnitude > 0.0f) ? (lMovementVec * GetSpeed() * Time.deltaTime) :  Vector3.zero;
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

    public Pickable GetCurrentPickup()
    {
        return mCurrentPickup;
    }

    private void PickObject(GameObject pickup)
    {
        pickup.transform.parent = transform;
        pickup.transform.localPosition = transform.forward * mAttatchOffset;
        pickup.transform.rotation      = transform.rotation;
        pickup.GetComponent<Collider>().isTrigger = true;
        pickup.GetComponent<Rigidbody>().isKinematic = true;

        mCurrentPickup = pickup.GetComponent<Pickable>();
        Assert.IsNotNull(mCurrentPickup, "Object " + pickup.name + " doesn't have a 'Pickable' component");

        mCurrentPickup.OnPickedUp(this);
        DropHeldObject();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Pickup")
        {
            GameObject pickedGameObject = Instantiate(other.GetComponent<PickupTrigger>().mPickupObject, transform);
            PickObject(pickedGameObject);

            Destroy(other.gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (transform.forward * 3.0f));
    }
}
