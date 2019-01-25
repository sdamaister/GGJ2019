using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemSpawnController : MonoBehaviour
{
    public float MinDelay = 10.0f;
    public float MaxDelay = 20.0f;
    public GameObject SpawneableObject;

    private GameObject[] SpawnPoints;
    private GameObject LastSpawn = null;
    private float NextSpawnTs;

    // Start is called before the first frame update
    void Start()
    {
        NextSpawnTs = Random.Range(MinDelay, MaxDelay);

        SpawnPoints = GameObject.FindGameObjectsWithTag("spawn");
    }

    // Update is called once per frame
    void Update()
    {
        NextSpawnTs -= Time.deltaTime;

        if (NextSpawnTs <= 0.0f)
        {
            SpawnObject(SelectNextSpawnPoint());
            NextSpawnTs = Random.Range(MinDelay, MaxDelay);
        }
    }

    private GameObject SelectNextSpawnPoint()
    {
        GameObject selected = null;
        do
        {
            int which = Random.Range(0, SpawnPoints.Length);
            selected = SpawnPoints[which];
        } while (selected == LastSpawn && (SpawnPoints.Length > 1) );

        LastSpawn = selected;
        return selected;
    }

    private void SpawnObject(GameObject where)
    {
        Instantiate(SpawneableObject, where.transform.position, Quaternion.identity);
    }
}
