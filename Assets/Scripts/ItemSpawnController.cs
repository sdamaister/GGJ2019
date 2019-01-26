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

    private GameObject LastSpawn = null;
    private float NextSpawnTs;
    private List<GameObject> EmptySpawnPoints;

    // Start is called before the first frame update
    void Start()
    {
        NextSpawnTs = Random.Range(MinDelay, MaxDelay);

        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("spawn");
        EmptySpawnPoints = new List<GameObject>(spawnPoints);
    }

    // Update is called once per frame
    void Update()
    {
        NextSpawnTs -= Time.deltaTime;

        if (NextSpawnTs <= 0.0f)
        {
            if (EmptySpawnPoints.Count > 0)
            {
                SpawnObject(SelectNextSpawnPoint());
                NextSpawnTs = Random.Range(MinDelay, MaxDelay);
            }
        }
    }

    public void ItemPickedUp(GameObject from)
    {
        EmptySpawnPoints.Add(from);

        if (EmptySpawnPoints.Count == 1)
        {
            NextSpawnTs = Random.Range(MinDelay, MaxDelay);
        }
    }

    private GameObject SelectNextSpawnPoint()
    {
        GameObject selected = null;
        do
        {
            int which = Random.Range(0, EmptySpawnPoints.Count);
            selected = EmptySpawnPoints[which];
        } while (selected == LastSpawn && (EmptySpawnPoints.Count > 1) );

        EmptySpawnPoints.Remove(selected);
        LastSpawn = selected;
        return selected;
    }

    private void SpawnObject(GameObject where)
    {
        GameObject spawned = Instantiate(SpawneableObject, where.transform.position, Quaternion.identity);
        spawned.GetComponent<PickupTrigger>().SpawnedFrom = where;
    }
}
