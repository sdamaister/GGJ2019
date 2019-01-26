using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private enum EButtonState
    {
        eUnPressed,
        ePressed,
    }

    public int  mControllerIdx = 0;
    public bool mOrientWithMovement = true;

    private float mJoystickMovementEpsilon = 0.1f;
    private float mJoystickLookEpsilon = 0.05f;

    public Vector3 GetMoveVector()
    {
        float lMoveHorizontal = Input.GetAxis("Horizontal " + mControllerIdx);
        float lMoveVertical   = Input.GetAxis("Vertical "   + mControllerIdx);

        if (((Mathf.Abs(lMoveHorizontal) >= mJoystickMovementEpsilon) || (Mathf.Abs(lMoveVertical) >= mJoystickMovementEpsilon)))
        {
            return Vector3.Normalize(new Vector3(lMoveHorizontal, 0.0f, lMoveVertical));
        }

        return Vector3.zero;
    }

    public Vector3 GetLookVector()
    {
        if (mOrientWithMovement)
        {
            return GetMoveVector();
        }

        float lLookHorizontal = Input.GetAxis("RightH " + mControllerIdx);
        float lLookVertical   = Input.GetAxis("RightV " + mControllerIdx);

        if (((Mathf.Abs(lLookHorizontal) >= mJoystickLookEpsilon)) || (Mathf.Abs(lLookVertical) >= mJoystickLookEpsilon))
        {
            return Vector3.Normalize(new Vector3(lLookHorizontal, 0.0f, lLookVertical));
        }

        return Vector3.zero;
    }

    public bool IsRightTriggerPressed()
    {
        return Input.GetAxis("TriggerR " + mControllerIdx) > 0.5f;
    }

    public bool IsDashPressed()
    {
        return Input.GetButtonDown("Dash " + mControllerIdx);
    }
}
