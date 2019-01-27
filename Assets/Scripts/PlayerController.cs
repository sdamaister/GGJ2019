using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;

[RequireComponent(typeof(PlayerLifeController))]
[RequireComponent(typeof(InputManager))]
[RequireComponent(typeof(SoundController))]
public class PlayerController : MonoBehaviour
{
    private enum EPlayerState
    {
        eNone,
        eIdle,
        eAttacking,
        eDashing,
        eStunned,
        eDead,
        eComeOutOfTentAnim,
        eLookOtherFoxesAnim
    }

    public GameObject stunIndicator;

    // used for start animation
    public GameObject tent;

    public int   mPlayerIdX     = 0;
    public float mSpeed         = 150.0f;
    public float MinSpeedFactor = 0.4f;
    public float MaxSpeedFactor = 1.0f;
    public float mAttatchOffset = 0.5f;
    public float AttackCooldownTime = 0.8f;
    public float StunCooldownTime = 3.0f;
    public float mDashForce       = 10.0f;
    public float mDashingFriction = 0.1f;
    public float mDashSpeedFinish = 5.0f;
    public float mAttackRotDeg = 45.0f;
    public float mAttackRotTime = 0.2f;

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

    private EPlayerState mCurrentState = EPlayerState.eNone;

    private Vector3 mInitialPosition;
    private Quaternion mInitialRotation;
    private Vector3 mInsideTentPosition;
    private Quaternion mTargetRotation;

    // Anims
    private float mComeOutAnimDuration = 1f;

    private Renderer mRenderer;

    private SoundController soundController;
    private float nextMine = 0.0f;
    
    public void SetInputControllerIdx(int aIdx)
    {
        mInputManager.mControllerIdx = aIdx;
    }

    public void TryPickObject(GameObject pickup)
    {
        if (mCurrentPickup == null && mCurrentState != EPlayerState.eStunned)
        {
            PickObject(pickup);
        }
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
    void Awake()
    {
        mRigidbody = GetComponent<Rigidbody>();
        Assert.IsNotNull(mRigidbody);

        playerLife = GetComponent<PlayerLifeController>();
        Assert.IsNotNull(playerLife);

        mInputManager = GetComponent<InputManager>();
        Assert.IsNotNull(mInputManager);
        mInputManager.enabled = false;

        mCollider = GetComponent<Collider>();
        Assert.IsNotNull(mCollider);

        mPhyMat = mCollider.material;
        Assert.IsNotNull(mPhyMat);

        mAnimator = GetComponent<Animator>();
        Assert.IsNotNull(mPhyMat);

        Transform lTransform = transform.Find("FoxPivot/Foxy_/IK Chain002/Foxy/Body");
        if (lTransform != null)
        {
            mRenderer = lTransform.gameObject.GetComponent<Renderer>();
            Assert.IsNotNull(mRenderer);
        }

        playerLife.OnDie += Die;

        attackBox = transform.Find("AttackBox").gameObject;
        attackBox.SetActive(false);

        mInitialPosition = transform.position - transform.forward * 0.4f;
        mInitialRotation = transform.rotation;
        mInsideTentPosition = tent.transform.position + tent.transform.forward * 0.3f;

        soundController = GetComponent<SoundController>();
        Assert.IsNotNull(soundController);
    }

    private void Update()
    {
        if (cooldownRemainingTime > 0.0f)
        {
            cooldownRemainingTime -= Time.deltaTime;
        }

        switch(mCurrentState)
        {
            case EPlayerState.eNone:
                break;
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
            case EPlayerState.eComeOutOfTentAnim:
                float timeFactorRotation =  (mComeOutAnimDuration - cooldownRemainingTime + 0.1f)/(mComeOutAnimDuration);
                timeFactorRotation = Mathf.Clamp(timeFactorRotation, 0f, 1f);

                float timeFactorPosition = (mComeOutAnimDuration - cooldownRemainingTime) / mComeOutAnimDuration;
                timeFactorPosition = Mathf.Clamp(timeFactorPosition, 0f, 1f);

                transform.Find("FoxPivot").localRotation = Quaternion.Lerp(Quaternion.Euler(40, 0, 0), Quaternion.Euler(0, 0, 0), timeFactorRotation);
                transform.position = Vector3.Lerp(mInsideTentPosition, mInitialPosition, timeFactorPosition);
                if (cooldownRemainingTime <= 0) {
                    LookAtOtherFox();
                }
                break;

            case EPlayerState.eLookOtherFoxesAnim:
                transform.rotation = Quaternion.Lerp(transform.rotation, mTargetRotation, 0.2f);
                if (cooldownRemainingTime <= 0) {
                    LookAtOtherFox();
                }
                break;
            default:
                break;
        }
        
        // rendering stuff
        mRenderer.material.SetFloat("_FreezeCoef", playerLife.GetLifePercent());
        mRenderer.material.SetFloat("_Stunned", (mCurrentState == EPlayerState.eStunned) ? 1.0f : 0.0f);

        if (nextMine <= 0.0f && mCurrentState != EPlayerState.eDead)
        {
            nextMine = soundController.PlayMine();
            nextMine += Random.Range(5.0f, 15.0f);
        }
        nextMine -= Time.deltaTime;
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

    private void PickObject(GameObject pickup)
    {
        Transform socket = transform.Find("FoxPivot/Foxy_/Bone001/Bone002/Bone006/Bone027/Bone028/Bone029/Hand_R_Fire");
        pickup.transform.parent = socket;
        pickup.transform.localPosition = (transform.forward + transform.up) * mAttatchOffset;
        pickup.transform.rotation = transform.rotation;
        pickup.GetComponent<Collider>().isTrigger = true;
        pickup.GetComponent<Rigidbody>().isKinematic = true;

        mCurrentPickup = pickup.GetComponent<Pickable>();
        Assert.IsNotNull(mCurrentPickup, "Object " + pickup.name + " doesn't have a 'Pickable' component");

        mAnimator.SetBool("holding", true);

        mCurrentPickup.OnPickedUp(this);

        if (pickup.tag == "Torch")
        {
            soundController.PlayGrabTorch();
        }
        else if (pickup.tag == "Stick")
        {
            soundController.PlayGrabStick();
        }
    }

    // State enter
    public void Idle()
    {
        mPhyMat.staticFriction = 0f;
        mPhyMat.dynamicFriction = 0f;
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

        soundController.PlayThrow();
    }

    private void Dash()
    {
        return; // until proved useful gameplay wise
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

        soundController.PlayStun();
    }

    public void Die()
    {
        mCurrentState = EPlayerState.eDead;
        DropHeldObject();

        mPhyMat.dynamicFriction = 1.0f;
        mPhyMat.staticFriction = 1.0f;

        enabled = false;

        soundController.PlayDieOfHeat();
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

                if (mCurrentPickup && mCurrentPickup.tag == "Stick")
                {
                    soundController.PlayThrow();
                }
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

    public void Reset()
    {
        if (playerLife == null) return;
        playerLife.Reset();
        cooldownRemainingTime = 0.0f;
        if (mCurrentPickup != null) {
            Debug.Log("Destroying pickup!");
            mCurrentPickup.DoDestroy();
            mCurrentPickup = null;
        }
        transform.position = mInitialPosition;
        transform.rotation = mInitialRotation;

        nextMine = Random.Range(5.0f, 15.0f);

        enabled = true;
        mCurrentState = EPlayerState.eNone;
    }

    public void DoStartAnim() {
        mInputManager.enabled = false;
        mPhyMat.dynamicFriction = 1.0f;
        mPhyMat.staticFriction = 1.0f;
        mCurrentState = EPlayerState.eComeOutOfTentAnim;

        transform.position = mInsideTentPosition;
        transform.Find("FoxPivot").localRotation = Quaternion.Euler(40, 0, 0);
        Debug.Log("DoStartAnim " + mPlayerIdX);
        cooldownRemainingTime = mComeOutAnimDuration + Random.Range(1f, 1.5f);

        playerLife.DecrementLife = false;
    }

    public void LookAtOtherFox() {
        mCurrentState = EPlayerState.eLookOtherFoxesAnim;
        mTargetRotation = mInitialRotation * Quaternion.Euler(0, 45 * (Random.Range(0f, 1f) > 0.5f ? 1f : -1f), 0);
        cooldownRemainingTime = 0.5f;
    }

    public bool IsDead() {
        return (EPlayerState.eDead == mCurrentState);
    }

    public void ReadyToPlay() {
        playerLife.DecrementLife = true;
        transform.rotation = mInitialRotation;
        Idle();
        mInputManager.enabled = true;
        enabled = true;
    }
}
