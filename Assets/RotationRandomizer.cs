using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationRandomizer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
    }
}
