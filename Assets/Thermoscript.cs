using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thermoscript : MonoBehaviour
{

    public GameObject player;
    private Vector3 offset;
    public GameObject sprite;

    // Start is called before the first frame update
    void Start()
    {
        offset = (transform.position - player.transform.position) - sprite.transform.forward * 4f;
    }

    void LateUpdate() {
        transform.position = player.transform.position + offset;
    }
}
