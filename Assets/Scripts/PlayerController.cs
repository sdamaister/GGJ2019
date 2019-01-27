using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;

[RequireComponent(typeof(PlayerLifeController))]
[RequireComponent(typeof(InputManager))]
public class PlayerController : MonoBehaviour
{
    private enum EPlayerState
    {
        eIdle,
        eAttacking,
        eDashing,
        eStunned,
        eDead
    }

    public GameObject stunIndicator;
    public int   mPlayerIdX     = 0;
    public float mSpeed         = 150.0f;
    public float MinSpeedFactor = 0.4f;
    public float MaxSpeedFactor = 1.0f;
    public float mAttatchOffset = 0.5f;
    public float AttackCooldownTime = 2.0f;
    public float StunCooldownTime = 3.0f;
    public float mDashForce       = 10.0f;
    public float mDashingFriction = 0.1f;
    public float mDashSpeedFinish = 5.0f;
    public float mAttackRotDeg = 45.0f;
    public float mAttackRotTime = 1.0f;

    private Rigidbody mRigidbody;
    private PlayerLifeController playerLife;
    private InputManager mInputManager;
    private PhysicMaterial mPhyMat;
    private Animator mAnimator;
    private Collider mCollider;

    private GameObject attackBox;
    private float cooldownRemainingTime = 0.0f;

    private float mAttackElapsedTime = 0.0f;

    private Pickable mCurrentPickup;

    private EPlayerState mCurrentState;

    public void PickObject(GameObject pickup)
    {
        pickup.transform.parent = transform;
        pickup.transform.localPosition = transform.forward * mAttatchOffset;
        pickup.transform.rotation = transform.rotation;
        pickup.GetComponent<Collider>().isTrigger = true;
        pickup.GetComponent<Rigidbody>().isKinematic = true;

        mCurrentPickup = pickup.GetComponent<Pickable>();
        Assert.IsNotNull(mCurrentPickup, "Object " + pickup.name + " doesn't have a 'Pickable' component");

        mAnimator.SetBool("holding", true);

        mCurrentPickup.OnPickedUp(this);
    }

    public void DropHeldObject()
    {
        if (mCurrentPickup != null)
        {
            mCurrentPickup.transform.parent = null;
            mCurrentPickup.OnDropped(this);

            mAnimator.SetBool("holding", false);
            mAnimator.SetBool("throwing", false);

            mCurrentPickup = null;
        }
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

        mCollider = GetComponent<Collider>();
        Assert.IsNotNull(mCollider);

        mPhyMat = mCollider.material;
        Assert.IsNotNull(mPhyMat);

        mAnimator = GetComponent<Animator>();
        Assert.IsNotNull(mPhyMat);

        playerLife.OnDie += Die;

        attackBox = transform.Find("AttackBox").gameObject;
        attackBox.SetActive(false);
    }

    private void Update()
    {
        if (cooldownRemainingTime > 0.0f)
        {
            cooldownRemainingTime -= Time.deltaTime;
        }

        switch(mCurrentState)
        {
            case EPlayerState.eIdle:
                OnIdle();
                break;
            case EPlayerState.eAttacking:
                OnAttack();
                break;
            case EPlayerState.eDashing:
                OnDashing();
                break;
            case EPlayerState.eStunned:
                OnStun();
                break;
            case EPlayerState.eDead:
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (mCurrentState == EPlayerState.eIdle)
        {
            Vector3 lMovementVec = mInputManager.GetMoveVector();
            mRigidbody.velocity = (lMovementVec.magnitude > 0.0f) ? (lMovementVec * GetSpeed() * Time.deltaTime) :  Vector3.zero;

            if (mInputManager.IsDashPressed())
            {
                Dash();
            }
        }
    }
    
    private float GetSpeed()
    {
        // TODO: Maybe use a Bezier curve
        float speedFactor = Mathf.Lerp(MinSpeedFactor, MaxSpeedFactor, playerLife.Life * 0.01f);
        return speedFactor * mSpeed;
    }

    public Pickable GetCurrentPickup()
    {
        return mCurrentPickup;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Attack")
        {
            Stun(other.transform.forward);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Pickup" && mCurrentPickup == null && mCurrentState != EPlayerState.eStunned)
        {
            PickObject(other.GetComponent<PickupTrigger>().mPickupObject);

            Destroy(other.gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (transform.forward * 3.0f));
    }

    // State enter
    private void Idle()
    {
        mPhyMat.dynamicFriction = 0;
        mCurrentState = EPlayerState.eIdle;
    }

    private void Attack()
    {
        mCurrentState = EPlayerState.eAttacking;

        attackBox.SetActive(true);
        cooldownRemainingTime = AttackCooldownTime;

        mAnimator.SetBool("attacking", true);
        mAttackElapsedTime = 0.0f;

        mRigidbody.velocity = Vector3.zero;
    }

    private void Dash()
    {
        Vector3 lMoveDir = mRigidbody.velocity.normalized;
        mRigidbody.AddForce(transform.forward * mDashForce, ForceMode.Impulse);
        mPhyMat.dynamicFriction = mDashingFriction;

        mAnimator.SetBool("dashing", true);

        mCurrentState = EPlayerState.eDashing;
    }

    public void Stun(Vector3 direction)
    {
        DropHeldObject();
        mCurrentState = EPlayerState.eStunned;
        cooldownRemainingTime = StunCooldownTime;

        mRigidbody.AddForce(direction * 5.0f, ForceMode.Impulse);
        mPhyMat.dynamicFriction = 0.5f;
        stunIndicator.SetActive(true);

        mAnimator.SetBool("stunned", true);
        mAnimator.SetBool("walking", false);
        mAnimator.SetBool("dashing", false);
        mAnimator.SetBool("attacking", false);
        mAnimator.SetBool("throwing", false);
        mAnimator.SetBool("holding", false);
    }

    private void Die()
    {
        mCurrentState = EPlayerState.eDead;
        DropHeldObject();

        mPhyMat.dynamicFriction = 1.0f;
        mPhyMat.staticFriction = 1.0f;

        enabled = false;
    }

    // States
    private void OnDashing()
    {
        if (mRigidbody.velocity.magnitude <= mDashSpeedFinish)
        {
            mAnimator.SetBool("dashing", false);
            Idle();
        }
    }

    private void OnIdle()
    {
        Vector3 lLookVector = mInputManager.GetLookVector();
        if (lLookVector.magnitude > 0.0f)
        {
            transform.forward = lLookVector;
        }

        mAnimator.SetBool("walking", mRigidbody.velocity.magnitude > 0.1f);

        if (mInputManager.IsRightTriggerPressed())
        {
            if (mCurrentPickup != null)
            {
                mCurrentPickup.OnAction(this);
                mAnimator.SetBool("holding", false);
                mAnimator.SetBool("throwing", false);
            }
            else
            {
                Attack();
            }
        }
    }

    private void OnAttack()
    {
        if (cooldownRemainingTime <= 0.0f)
        {
            attackBox.SetActive(false);
            mAnimator.SetBool("throwing", false);
            mAnimator.SetBool("holding", false);
            mAnimator.SetBool("attacking", false);
            Idle();
        }
        else
        {
            mAttackElapsedTime += Time.deltaTime;
            if (mAttackElapsedTime < mAttackRotTime)
            {
                transform.Rotate(Vector3.right, mAttackRotDeg * (mAttackElapsedTime / mAttackRotTime) * Mathf.Deg2Rad);
            }
            else if (mAttackElapsedTime < (mAttackRotTime * 2.0f))
            {
                transform.Rotate(Vector3.right, -mAttackRotDeg * ( (mAttackElapsedTime - mAttackRotTime) / mAttackRotTime) * Mathf.Deg2Rad);
            }
        }
    }

    private void OnStun()
    {
        if (cooldownRemainingTime <= 0)
        {
            mAnimator.SetBool("stunned", false);
            Idle();
            stunIndicator.SetActive(false);
        }
    }
}
