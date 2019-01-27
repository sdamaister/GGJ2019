using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDirector : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3 mInitialPosition;
    private Quaternion mInitialRotation;

    public List<Transform> cameraLookTent;
    private Vector3 targetPosition;
    private Quaternion targetRotation;

    void Start()
    {
        targetPosition = mInitialPosition = transform.position;
        targetRotation = mInitialRotation = transform.rotation;
    }

    void Update() {
        transform.position = Vector3.Lerp(transform.position, targetPosition, 0.05f);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.05f);
    }

    // Update is called once per frame
    public void LookAtPlayer (int i) {
        if (i < 0) {
            targetPosition = mInitialPosition;
            targetRotation = mInitialRotation;
        } else {
            targetPosition = cameraLookTent[i].position;
            targetRotation = cameraLookTent[i].rotation;
        }
    }
}
