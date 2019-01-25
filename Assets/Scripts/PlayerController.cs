using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float mSpeed = 1.0f;

    private Rigidbody mRigidbody;

    // Start is called before the first frame update
    void Start()
    {
        mRigidbody = GetComponent<Rigidbody>();
        Assert.IsNotNull(mRigidbody);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float lMoveHorizontal = Input.GetAxis("Horizontal");
        float lMoveVertical   = Input.GetAxis("Vertical");

        if ( (lMoveHorizontal) != 0.0f || (lMoveVertical != 0.0f) )
        {
            mRigidbody.velocity = Vector3.Normalize( new Vector3(lMoveHorizontal, 0.0f, lMoveVertical) ) * mSpeed * Time.deltaTime;
        }
        else
        {
            mRigidbody.velocity = Vector3.zero;
        }
    }
}
