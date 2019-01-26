using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private enum EButtonState
    {
        eNone,
        eDown,
        ePressed,
        eUp
    }

    public int  mControllerIdx = 0;
    public bool mOrientWithMovement = true;

    private float mJoystickMovementEpsilon = 0.1f;
    private float mJoystickLookEpsilon = 0.05f;
    private EButtonState rightTriggerState = EButtonState.eNone;

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
        return rightTriggerState == EButtonState.eDown;
    }

    public bool IsDashPressed()
    {
        return Input.GetButtonDown("Dash " + mControllerIdx);
    }

    private void Update()
    {
        if (Input.GetAxis("TriggerR " + mControllerIdx) > 0.5f)
        {
            if (rightTriggerState == EButtonState.eNone)
            {
                rightTriggerState = EButtonState.eDown;
            }
            else if (rightTriggerState == EButtonState.eDown)
            {
                rightTriggerState = EButtonState.ePressed;
            }
        }
        else
        {
            if (rightTriggerState == EButtonState.ePressed || rightTriggerState == EButtonState.eDown)
            {
                rightTriggerState = EButtonState.eUp;
            }
            else if (rightTriggerState == EButtonState.eUp)
            {
                rightTriggerState = EButtonState.eNone;
            }
        }
    }
}
