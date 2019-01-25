using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.UIElements;
using UnityEngine;

public class PlayerLifeController : MonoBehaviour
{
    public float Life = 100.0f;
    public bool DecrementLife = true;
    public float LifeDecrementFactor = 1.0f;

    // Update is called once per frame
    void Update()
    {
        if (DecrementLife)
        {
            Life -= LifeDecrementFactor * Time.deltaTime;

            if (Life <= 0.0f)
            {
                DecrementLife = false;
                // TODO: Die
            }
        }
    }
}
