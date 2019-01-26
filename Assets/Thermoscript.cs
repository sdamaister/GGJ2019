using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thermoscript : MonoBehaviour
{

    public GameObject player;
    private PlayerLifeController lifeController;
    private Vector3 offset;
    public MeshRenderer sprite;

    // Start is called before the first frame update
    void Start()
    {
        offset = (transform.position - player.transform.position) - sprite.transform.forward * 4f;
        lifeController = player.GetComponent<PlayerLifeController>();
    }

    void LateUpdate() {
        transform.position = player.transform.position + offset;
        float percentage = lifeController.GetLifePercent();
        int spriteIndex = (int) Mathf.Max(0, Mathf.Min(23, Mathf.Floor(percentage * 24f)));
        string spriteName = "Thermometer/thermometerr" + spriteIndex;
        Texture texture = (Texture) Resources.Load(spriteName);
        sprite.material.mainTexture = texture;
    }
}
